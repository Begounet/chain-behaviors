using Sirenix.OdinInspector;
using System.IO;
using UnityEngine;
using ChainBehaviors.IO;
using AUE;
using AppTools;
using AppTools.Audio;
using static AppTools.FileManagement.AppDirectoryPath;
using ChainBehaviors.Utils;

namespace ChainBehaviors.Audio
{
    /// <summary>
    /// Capture the audio from an <see cref="AudioListener"/>.
    /// Create a temporary file and output everything into it.
    /// When capture is completed, create an AudioClip that streams data directly from this temporary file.
    /// It is important to tell the AudioListener to release the temporary file to release resource
    /// and delete the temporary file.
    /// These files may be very big (uncompressed audio) so it is really important.
    /// </summary>
    /// <remarks>
    /// Binary file layout:
    /// 4b: num samples
    /// 1b: num channels
    /// 4b: frequency
    /// Xb: float[] containing sample data
    /// </remarks>
    [RequireComponent(typeof(AudioListener))]
    [AddComponentMenu(CBConstants.ModuleAudioPath + "Audio Listener Capture")]
    public class AudioListenerCaptureController : BaseMethod
    {
        [SerializeField]
        private FileStreamCreator _captureStreamDesc = new FileStreamCreator()
        {
            FileMode = FileMode.Create,
            FileAccess = FileAccess.Write,
            FilePathBuilder = new AppFilePathBuilder()
            {
                FilePath = new AppTools.FileManagement.AppFilePath(EPathSource.TemporaryCachePath, "audio-listener-samples", ".dat")
            }
        };

        [SerializeField]
        private bool _autoStartCapture = true;

        [SerializeField]
        private AUEEvent _onRecordingStarted = null;

        [SerializeField, Tooltip("Send the path to the recorded file")]
        private AUEEvent<string> _onRecordingCompleted = null;

        private BinaryWriter _binaryWriter;
        private FileStream _captureStream;
        private int _audioSampleRate = 0;
        private volatile bool _isRecording = false;
        private object _recordLock = new object();

        private void Awake() => _audioSampleRate = AudioSettings.outputSampleRate;

        private void Start()
        {
            if (_autoStartCapture)
            {
                StartCapture();
            }
        }

        [Button, FoldoutGroup("Editor Actions"), DisableInEditorMode]
        public void StartCapture()
        {
            if (_isRecording)
            {
                return;
            }

            TraceCustomMethodName("Start Capture");

            _onRecordingStarted.Invoke();

            _captureStream = _captureStreamDesc.CreateFileStream();
            _binaryWriter = new BinaryWriter(_captureStream);
            _isRecording = true;

            // Skip header for now
            _binaryWriter.Seek(AudioSampleFileHeader.Size, SeekOrigin.Begin);
        }

        [Button, FoldoutGroup("Editor Actions"), DisableInEditorMode]
        public void StopCapture()
        {
            if (!_isRecording)
            {
                return;
            }

            TraceCustomMethodName("Stop Capture");

            _isRecording = false;
            CompleteCapture();
        }

        private void OnDestroy()
        {
            if (!_isRecording)
            {
                return;
            }

            _isRecording = false;
            lock (_recordLock)
            {
                _captureStream?.Dispose();
            }
        }

        private void CompleteCapture()
        {
            if (_captureStream == null)
            {
                return;
            }

            lock (_recordLock)
            {
                int fileSize = (int)_captureStream.Length;
                InsertAudioSampleHeader(fileSize);
                TraceCustomMethodName("Complete Capture ~ Start", ("audio file source length", fileSize));
                _captureStream.Close();
            }
            _onRecordingCompleted.Invoke(_captureStream.Name);
        }

        private void InsertAudioSampleHeader(int fileSize)
        {
            int numChannels = 2;
            int numFloatSamples = fileSize / sizeof(float);

            _binaryWriter.Seek(0, SeekOrigin.Begin);
            var header = new AudioSampleFileHeader()
            {
                NumSamples = numFloatSamples,
                Frequency = _audioSampleRate,
                NumChannels = (byte)numChannels
            };
            header.Write(_binaryWriter);
        }

        private void OnAudioFilterRead(float[] data, int channels)
        {
            if (!_isRecording)
            {
                return;
            }

            lock (_recordLock)
            {
                MemoryArrayConverter.WriteInStream(data, _captureStream);
            }
        }
    }
}