#if USE_DARKTABLE_SAVWAV
using Darktable;
using UnityEngine;
using System.IO;
using AUE;
using ChainBehaviors.Utils;

namespace ChainBehaviors.Audio
{
    /// <summary>
    /// Convert audio raw samples (float[]) to WAV data (byte[])
    /// </summary>
    [AddComponentMenu(CBConstants.ModuleAudioPath + nameof(AudioAmplitudesToWaveData))]
    public class AudioAmplitudesToWaveData : BaseMethod
    {
        [SerializeField]
        private AUEEvent<byte[]> _processed = null;

        public void Process(float[] data)
        {
            Trace();
            _processed.Invoke(ToWavData(data));
        }

        public static byte[] ToWavData(float[] data)
        {
            byte[] wavData = new byte[data.Length * 2];
            ToWavData(data, wavData, 0);
            return wavData;
        }

        public static void ToWavData(float[] data, byte[] output, uint dstOffset)
        {
            ToWavData(data, 0, output, dstOffset, (uint) data.Length);
        }

        public static void ToWavData(float[] data, uint srcOffset, Stream output, uint dstOffset, uint dataCount)
        {
            output.Seek(dstOffset, SeekOrigin.Begin);
            ToWavData(data, srcOffset, output, dataCount);
        }

        public static void ToWavData(float[] data, uint srcOffset, Stream output, uint dataCount)
        {
            for (uint i = srcOffset; i < dataCount; ++i)
            {
                short value = (short)(data[i] * SavWav.RescaleFactor);
                output.WriteByte((byte)(value >> 0));
                output.WriteByte((byte)(value >> 8));
            }
        }

        public static void ToWavData(float[] data, uint srcOffset, byte[] output, uint dstOffset, uint dataCount)
        {
            for (uint i = srcOffset; i < dataCount; ++i)
            {
                short value = (short)(data[i] * SavWav.RescaleFactor);
                output[dstOffset + i * 2] = (byte)(value >> 0);
                output[dstOffset + i * 2 + 1] = (byte)(value >> 8);
            }
        }

        public unsafe static void ToWavData(float* data, uint srcOffset, byte[] output, uint dstOffset, uint dataCount)
        {
            for (uint i = srcOffset; i < dataCount; ++i)
            {
                short value = (short)(data[i] * SavWav.RescaleFactor);
                output[dstOffset + i * 2] = (byte)(value >> 0);
                output[dstOffset + i * 2 + 1] = (byte)(value >> 8);
            }
        }
    }
}
#endif