using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using AppTools;
using AppTools.Audio;
using Cysharp.Threading.Tasks;
using ScriptableObjectArchitecture;
using Sirenix.OdinInspector;
using UnityEngine;
using ChainBehaviors.Utils;

namespace ChainBehaviors.Audio.Visualizer
{
    /// <summary>
    /// Generate a texture representing an AudioClip waveform
    /// </summary>
    [AddComponentMenu(CBConstants.ModuleAudioPath + "Audio Clip Waveform Visualizer")]
    public class AudioClipWaveformVisualizer : BaseMethod
    {
        private delegate float SampleHeightDelegate(float[] audioBuffer, 
            int audioBufferStartOffset,
            int sampleWindow,
            int audioSampleIdx, int sampleStep, int sampleCount);

        private const string OutputWaveformTextureName = "audio-waveform";

        public enum EVisualizationMode
        {
            Average,
            Means
        }

        public enum ESampleWindowMode
        {
            Auto,
            Custom
        }

        [SerializeField]
        private bool _autoStart = false;

        [SerializeReference, SerializedInterface]
        private IAudioDataSource _audioDataSource = null;
        public IAudioDataSource AudioDataSource { get => _audioDataSource; set => _audioDataSource = value; }

        [SerializeField]
        private int _clipLoadBufferSize = 44100;

        [SerializeField]
        private ESampleWindowMode _sampleWindowMode = ESampleWindowMode.Auto;

        [SerializeField, EnableIf(nameof(_sampleWindowMode), ESampleWindowMode.Custom)]
        private int _sampleWindow = 1024;

        [SerializeField, ShowIf(nameof(_sampleWindowMode), ESampleWindowMode.Auto)]
        [Tooltip("Width on the waveform of one sample window")]
        private int _waveformSampleWinWidth = 2;

        [SerializeField]
        private int _waveformWidth = 512;

        [SerializeField]
        private int _waveformHeight = 64;

        [SerializeField, Min(0)]
        private int _waveformSampleSpaceWidth = 1;

        [SerializeField, Tooltip("Max time allowed for calculus in one frame")]
        private float _timeBudgetSeconds = 0.003f;

        [SerializeField, Tooltip("The minimum value to display. For aesthetic purposes.")]
        private float _minWaveformSampleHeight = 0.0f;

        [SerializeField]
        private bool _useParallelization = false;

        [SerializeField]
        private EVisualizationMode _visualizationMode = EVisualizationMode.Means;

        [SerializeField]
        private bool _isOutputTextureReadable = false;

        [SerializeField]
        private Texture2DUnityEvent _waveformGenerated = null;

        private CancellationTokenSource _cancelTknSrc = null;


        private void Start()
        {
            if (_autoStart)
            {
                GenerateWaveform();
            }
        }

        private void OnDisable() => CancelWaveformGeneration();
        private void OnDestroy() => CancelWaveformGeneration();

        public void GenerateWaveform(AudioClip clip)
            => GenerateWaveform(new AudioClipDataSource(clip));

        public void GenerateWaveform(IAudioDataSource audioDataSource)
        {
            _audioDataSource = audioDataSource;
            if (_audioDataSource != null && _audioDataSource.AudioClip != null)
            {
                GenerateWaveform();
            }
        }

        public void GenerateWaveform()
        {
            if (_audioDataSource == null)
            {
                throw new InvalidOperationException("Audio data source must not be null");
            }

            Trace(("audio data source", _audioDataSource.AudioClip.name));

            CancelWaveformGeneration();
            _cancelTknSrc = new CancellationTokenSource();

            var sw = new Stopwatch();
            int startFramecount = Time.frameCount;
            sw.Start();
            GenerateWaveformAsync()
                .ContinueWith(texture2D =>
                {
                    sw.Stop();
                    //Debug.Log($"Time : {sw.ElapsedMilliseconds}ms (on {Time.frameCount - startFramecount} frames)");
                    if (texture2D != null)
                    {
                        _waveformGenerated.Invoke(texture2D);
                    }
                })
                .AttachExternalCancellation(_cancelTknSrc.Token)
                .SuppressCancellationThrow()
                .Forget((ex) => throw ex);
        }

        private async UniTask<Texture2D> GenerateWaveformAsync()
        {
            float[] sampleHeights = await GetSampleHeights();
            if (_cancelTknSrc.IsCancellationRequested || sampleHeights == null)
            {
                return null;
            }

            byte[] pixels = new byte[_waveformWidth * _waveformHeight];
            await SamplesHeightToPixels(sampleHeights, pixels);
            if (_cancelTknSrc.IsCancellationRequested)
            {
                return null;
            }

            Texture2D waveformTex =
                new Texture2D(_waveformWidth, _waveformHeight, TextureFormat.R8, mipChain: false);
            waveformTex.SetPixelData(pixels, 0, 0);
            waveformTex.name = OutputWaveformTextureName;
            waveformTex.filterMode = FilterMode.Point;
            waveformTex.wrapMode = TextureWrapMode.Clamp;
            waveformTex.Apply(updateMipmaps: false, makeNoLongerReadable: !_isOutputTextureReadable);

            return waveformTex;
        }

        private async UniTask<float[]> GetSampleHeights()
        {
            TimeoutYielder ty = new TimeoutYielder(_timeBudgetSeconds);

            int audioBufferLength = _clipLoadBufferSize * _audioDataSource.Channels;
            float[] audioBuffer = new float[audioBufferLength];
            int sampleHeightIdx = 0;

            _sampleWindow = DetermineSampleWindow();
            if (_sampleWindow <= 0)
            {
                return null;
            }
            
            int sampleWindow = _sampleWindow;
            int windowStep = _sampleWindow * _audioDataSource.Channels;
            int countSampleHeights = CountSampleHeights(audioBufferLength, windowStep);

            // If there is too much sample to calculate, relative to the final texture width...
            if (countSampleHeights > _waveformWidth)
            {
                // ... limit the number of sample to calculate by increasing the sample window size
                sampleWindow = Mathf.CeilToInt((float) _audioDataSource.Samples / _waveformWidth);
                windowStep = sampleWindow * _audioDataSource.Channels;
                countSampleHeights = CountSampleHeights(audioBufferLength, windowStep);
            }

            float[] sampleHeights = new float[countSampleHeights];
            int numSampleHeightsPerAudioBuffer = Mathf.CeilToInt((float) audioBufferLength / windowStep);
            SampleHeightDelegate getSampleHeightWindow = FindSampleHeightsMethod();

            int sampleStep = _audioDataSource.Channels;
            int sampleCount = _audioDataSource.Samples * sampleStep;

            for (int sampleIdx = 0;
                sampleIdx < _audioDataSource.Samples && !_cancelTknSrc.IsCancellationRequested;
                sampleIdx += audioBufferLength)
            {
                // Load audio buffer data
                if (!_audioDataSource.GetData(audioBuffer, sampleIdx))
                {
                    throw new Exception($"Could not load clip data");
                }

                if (_useParallelization)
                {
                    Parallel.For(0, numSampleHeightsPerAudioBuffer, idx =>
                    {
                        int audioBufferIdx = idx * windowStep;
                        sampleHeights[sampleHeightIdx + idx] =
                            getSampleHeightWindow(audioBuffer, audioBufferIdx, sampleWindow, sampleIdx, sampleStep,
                                sampleCount);
                    });
                    sampleHeightIdx += numSampleHeightsPerAudioBuffer;
                }
                else
                {
                    // Get sample height window from the loaded clip buffer, multiple times if necessary
                    // aka. for each sample window available...
                    for (int audioBufferIdx = 0; audioBufferIdx < audioBufferLength; audioBufferIdx += windowStep)
                    {
                        sampleHeights[sampleHeightIdx] =
                            getSampleHeightWindow(audioBuffer, audioBufferIdx, sampleWindow, sampleIdx, sampleStep,
                                sampleCount);

                        ++sampleHeightIdx;
                    }
                }

                await ty.Yield();
            }

            await UniTask.RunOnThreadPool(() => NormalizeSampleHeights(sampleHeights))
                .AttachExternalCancellation(_cancelTknSrc.Token)
                .SuppressCancellationThrow();

            return sampleHeights;
        }

        private int DetermineSampleWindow()
        {
            switch (_sampleWindowMode)
            {
                case ESampleWindowMode.Auto:
                    int numSampleAndSpaces = Mathf.FloorToInt((float) _waveformWidth / (_waveformSampleWinWidth + _waveformSampleSpaceWidth));
                    return Mathf.FloorToInt((float) _audioDataSource.Samples / numSampleAndSpaces);
                case ESampleWindowMode.Custom:
                    return _sampleWindow;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private SampleHeightDelegate FindSampleHeightsMethod()
        {
            return _visualizationMode == EVisualizationMode.Means
                ? GetSampleHeightsWindowMeans
                : GetSampleHeightsWindowAverage;
        }

        private int CountSampleHeights(int audioBufferLength, int windowStep)
        {
            int numWindowInAudioBuffer = Mathf.CeilToInt((float) audioBufferLength / windowStep);
            int numAudioBufferInClip = Mathf.CeilToInt((float) _audioDataSource.Samples / audioBufferLength);
            int numSampleHeights = Mathf.CeilToInt(numAudioBufferInClip * numWindowInAudioBuffer);
            return numSampleHeights;
        }

        private void NormalizeSampleHeights(float[] sampleHeights)
        {
            float peakValue = sampleHeights.Max();
            if (_useParallelization)
            {
                Parallel.For(0, sampleHeights.Length, idx => { sampleHeights[idx] /= peakValue; });
            }
            else
            {
                for (int i = 0; i < sampleHeights.Length; ++i)
                {
                    sampleHeights[i] /= peakValue;
                }
            }
        }

        /// <returns>False if cancelled</returns>
        private async UniTask<bool> SamplesHeightToPixels(float[] sampleHeights, byte[] pixels)
        {
            int totalSpaceWidth = Mathf.Min(_waveformWidth, Mathf.CeilToInt(_waveformSampleSpaceWidth * (sampleHeights.Length - 2)));
            int totalSamplesWidth = Mathf.Max(0, _waveformWidth - totalSpaceWidth);
            int spaceWidth = _waveformSampleSpaceWidth;
            int sampleWidth = Mathf.CeilToInt((float)totalSamplesWidth / (sampleHeights.Length - 1));
            float sampleIndexFactor = (1.0f / (sampleWidth + spaceWidth));
            if (_useParallelization)
            {
                Parallel.For(0, _waveformWidth * _waveformHeight, pixelIdx =>
                {
                    int x = pixelIdx % _waveformWidth;
                    int y = pixelIdx / _waveformWidth;
                    SetPixelValue(pixels, sampleHeights, sampleIndexFactor, sampleWidth, spaceWidth,
                        pixelIdx, x, y);
                });
            }
            else
            {
                return !await UniTask.RunOnThreadPool(() =>
                    {
                        for (int y = 0; y < _waveformHeight; ++y)
                        {
                            for (int x = 0; x < _waveformWidth; ++x)
                            {
                                int pixelIdx = y * _waveformWidth + x;
                                SetPixelValue(pixels, sampleHeights, sampleIndexFactor,
                                    sampleWidth, spaceWidth,
                                    pixelIdx, x, y);
                            }
                        }
                    }).AttachExternalCancellation(_cancelTknSrc.Token)
                    .SuppressCancellationThrow();
            }
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetPixelValue(byte[] pixels, float[] sampleHeights, float sampleIndexFactor,
            int sampleWidth, int spaceWidth, int pixelIdx, int x, int y)
        {
            int sampleIndex = Mathf.FloorToInt(x * sampleIndexFactor);
            sampleIndex = (sampleIndex < sampleHeights.Length ? sampleIndex : sampleHeights.Length - 1);
            
            float sampleValue = sampleHeights[sampleIndex];
            sampleValue = (sampleValue > _minWaveformSampleHeight ? sampleValue : _minWaveformSampleHeight);
            sampleValue = (sampleIndex > sampleHeights.Length - 1 ? 0.5f : sampleValue);

            bool isFilled = (sampleValue > (float) y / _waveformHeight);
            bool isSpace = (x % (sampleWidth + spaceWidth) >= sampleWidth);
            pixels[pixelIdx] = (isFilled && !isSpace ? byte.MaxValue : byte.MinValue);
        }

        private static float GetSampleHeightsWindowAverage(float[] audioBuffer, int audioBufferStartOffset,
            int sampleWindow,
            int audioSampleIdx, int sampleStep, int sampleCount)
        {
            float acc = 0;
            int count = 0;

            for (int sampleIdx = 0; sampleIdx < sampleWindow * sampleStep; sampleIdx += sampleStep)
            {
                int totalSampleIndex = audioSampleIdx + audioBufferStartOffset + sampleIdx;

                // Ensure that neither we don't go out of the buffer, nor after the end of the audio clip 
                if (audioBufferStartOffset + sampleIdx >= audioBuffer.Length || totalSampleIndex > sampleCount)
                {
                    break;
                }

                float sampleData = audioBuffer[audioBufferStartOffset + sampleIdx];
                float sampleValue = (sampleData > 0 ? sampleData : -sampleData);
                acc = sampleValue;
                ++count;
            }

            return acc / count;
        }

        private static float GetSampleHeightsWindowMeans(float[] audioBuffer, int audioBufferStartOffset,
            int sampleWindow,
            int audioSampleIdx, int sampleStep, int sampleCount)
        {
            float acc = 0;
            int count = 0;

            for (int sampleIdx = 0; sampleIdx < sampleWindow * sampleStep; sampleIdx += sampleStep)
            {
                int totalSampleIndex = audioSampleIdx + audioBufferStartOffset + sampleIdx;

                // Ensure that neither we don't go out of the buffer, nor after the end of the audio clip 
                if (audioBufferStartOffset + sampleIdx >= audioBuffer.Length || totalSampleIndex > sampleCount)
                {
                    break;
                }

                float sampleData = audioBuffer[audioBufferStartOffset + sampleIdx];
                acc = sampleData * sampleData;
                ++count;
            }

            return Mathf.Sqrt(acc / count);
        }

        private void CancelWaveformGeneration()
        {
            _cancelTknSrc?.Cancel();
        }
    }
}