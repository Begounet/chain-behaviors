#if USE_DARKTABLE_SAVWAV
using UnityEngine;
using System.IO;
using Cysharp.Threading.Tasks;
using AUE;
using ChainBehaviors.IO;
using AppTools;
using AppTools.Audio;
using ChainBehaviors.Utils;

namespace ChainBehaviors.Audio
{
    /// <summary>
    /// Convert audio samples file to WAV data
    /// </summary>
    [AddComponentMenu(CBConstants.ModuleAudioPath + "Audio Samples File To WAV data"))]
    public class AudioSamplesFileToWaveData : BaseMethod
    {
        [SerializeReference]
        private IStream _outputStream;

        [SerializeField]
        private bool _generateHeader = true;

        [SerializeField, Tooltip("Expressed in kilobytes")]
        private BufferSize _fileStreamBlockSize = new BufferSize(128, BufferSize.EScale.Kb);

        [SerializeField, Tooltip("Expressed in kilobytes")]
        private BufferSize _blockSize = new BufferSize(32, BufferSize.EScale.Kb);

        [SerializeField]
        private AUEEvent _processed = null;


        public void ConvertFromFile(string filePath)
        {
            Trace(("file path", filePath));

            var fileStream = new FileStream(filePath,
                FileMode.Open, FileAccess.Read, FileShare.ReadWrite,
                _fileStreamBlockSize.BytesSizeAsInt,
                FileOptions.SequentialScan | FileOptions.Asynchronous);

            ConvertAsync(fileStream)
                .ContinueWith(() =>
                {
                    fileStream.Close();
                    _processed.Invoke();
                })
                .Forget((ex) => throw ex);
        }

        private async UniTask ConvertAsync(Stream audioStream)
        {
            var header = AudioSampleFileHeader.CreateFromStream(audioStream);

            long dataLength = header.NumSamples;
            if (_generateHeader)
            {
                dataLength += SavWav.HeaderSize;
            }
            var outputStream = _outputStream.CreateStream(dataLength);

            if (_generateHeader)
            {
                SavWav.InsertHeader(outputStream, (uint)audioStream.Length, (uint)header.Frequency, header.NumChannels, (uint)header.NumSamples);
            }

            int blockSizeAsBytes = _blockSize.BytesSizeAsInt;
            byte[] blockBuffer = new byte[blockSizeAsBytes * sizeof(float)];
            byte[] wavBuffer = new byte[blockSizeAsBytes * sizeof(short)];

            int count;
            uint offset = 0;
            while ((count = Mathf.Min(blockBuffer.Length, (int)(audioStream.Length - audioStream.Position))) > 0)
            {
                uint sampleCount = ((uint)count / sizeof(float));
                uint wavSampleCount = ((uint)count / sizeof(short));

                await audioStream.ReadAsync(blockBuffer, 0, count);

                // Reinterpret byte array as float array. Prevents a float[] allocation.
                unsafe
                {
                    fixed (byte* blockBufferPtr = &blockBuffer[0])
                    {
                        float* samplesBuffer = (float*)blockBufferPtr;
                        AudioAmplitudesToWaveData.ToWavData(samplesBuffer, 0, wavBuffer, 0, sampleCount);
                    }
                }

                // Write all the buffer in one time
                await outputStream.WriteAsync(wavBuffer, 0, (int)wavSampleCount);
                offset += (uint)count;
            }

            outputStream.Close();
        }

        public void ConvertFromAudioDataSource(IAudioDataSource audioDataSource)
        {
            ConvertFromAudioDataSourceAsync(audioDataSource)
                .ContinueWith(_processed.Invoke)
                .Forget((ex) => throw ex);
        }

        private async UniTask ConvertFromAudioDataSourceAsync(IAudioDataSource audioDataSource)
        {
            long dataLength = audioDataSource.Samples;
            if (_generateHeader)
            {
                dataLength += SavWav.HeaderSize;
            }
            var outputStream = _outputStream.CreateStream(dataLength);

            if (_generateHeader)
            {
                SavWav.InsertHeader(outputStream, (uint)dataLength, (uint)audioDataSource.Frequency, (ushort) audioDataSource.Channels, (uint)audioDataSource.Samples);
            }

            int blockSizeAsBytes = _blockSize.BytesSizeAsInt;
            float[] blockBuffer = new float[blockSizeAsBytes];
            byte[] wavBuffer = new byte[blockSizeAsBytes * sizeof(short)];

            uint offset = 0;
            while (offset < audioDataSource.Samples)
            {
                uint readSampleCount = (uint) Mathf.Min(blockBuffer.Length, audioDataSource.Samples - offset);
                uint readSampleCountAsBytes = readSampleCount * sizeof(ushort);

                if (!audioDataSource.GetData(blockBuffer, (int) offset))
                {
                    throw new System.ArgumentException($"Cannot read audio data from parameter {nameof(audioDataSource)}");
                }

                // Reinterpret byte array as float array. Prevents a float[] allocation.
                unsafe
                {
                    fixed (float* samplesBuffer = &blockBuffer[0])
                    {
                        AudioAmplitudesToWaveData.ToWavData(samplesBuffer, 0, wavBuffer, 0, readSampleCount);
                    }
                }

                // Write all the buffer in one time
                await outputStream.WriteAsync(wavBuffer, 0, (int)readSampleCountAsBytes);
                offset += readSampleCount;
            }

            outputStream.Close();
        }
    }
}
#endif