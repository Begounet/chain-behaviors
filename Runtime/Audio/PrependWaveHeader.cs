#if USE_DARKTABLE_SAVWAV
using Darktable;
using Utilities.Tools;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AUE;

namespace ChainBehaviors.Audio
{
    public class PrependWaveHeader : MonoBehaviour
    {
        [SerializeField]
        private bool _useAudioOutputFrequency = false;

        [SerializeField, Min(0), HideIf("_useAudioOutputFrequency")]
        private int _frequency = 44100;

        [SerializeField, Range(1, 2)]
        private int _channels = 2;


        [SerializeField]
        private AUEEvent<byte[]> _processed = null;


        public void Process(IEnumerable<byte> data)
        {
            int dataCount = data.Count();
            byte[] wavData = new byte[SavWav.HeaderSize + dataCount];
            var dataEnumerator = data.GetEnumerator();
            int index = 0;
            while (dataEnumerator.MoveNext())
            {
                wavData[SavWav.HeaderSize + index] = dataEnumerator.Current;
                ++index;
            }

            int frequency = _useAudioOutputFrequency ? AudioSettings.outputSampleRate : _frequency;

            SavWav.InsertHeader(wavData, (uint)frequency, (ushort)_channels, (uint)dataCount);
            _processed.Invoke(wavData);
        }
    }
}
#endif