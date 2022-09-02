using UnityEngine;
using AUE;
using ChainBehaviors.Proxy;

namespace ChainBehaviors.Processes
{
    public class ExtractAudioClipTimeProcess : BaseProcess
    {
        [SerializeField]
        private UnityObjectProxyable<AudioSource> _source = null;
        public AudioSource Source
        {
            get => _source;
            set => _source.SetDirectValue(value);
        }

        [SerializeField]
        private bool _isTimeNormalized = true;

        [SerializeField]
        private AUEEvent<float> _timeUpdated = null;


        public override void UpdateProcess(float deltaTime)
        {
            TraceCustomMethodName("Update Process", ("deltaTime", deltaTime));

            var audioSource = _source.Value;
            if (audioSource == null || audioSource.clip == null)
            {
                return;
            }

            /// Based on <see cref="AudioSource.timeSamples"/> because <see cref="AudioSource.time"/> is not reliable when not playing.

            int timeSamples = audioSource.timeSamples;
            float time = ((float)timeSamples / audioSource.clip.samples) * audioSource.clip.length;
            if (_isTimeNormalized)
            {
                time /= audioSource.clip.length;
            }

            _timeUpdated?.Invoke(time);
        }
    }
}