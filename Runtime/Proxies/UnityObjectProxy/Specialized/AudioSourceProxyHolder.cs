using AUE;
using UnityEngine;
using ChainBehaviors.Utils;

namespace ChainBehaviors.Proxy
{
    [AddComponentMenu(CBConstants.ModuleObjectProxyPath + "Audio Source Proxy Holder")]
    public class AudioSourceProxyHolder : MonoBehaviour, IProxyObjectHolder<AudioSource>
    {
        [SerializeField]
        private AudioSource _audioSource = null;
        public AudioSource AudioSource
        { 
            get => _audioSource;
            set
            {
                _audioSource = value;
                NotifyAudioSourceSet();
            }
        }

        public AudioSource Proxy { get => AudioSource; set => AudioSource = value; }

        public AudioClip AudioClip 
        { 
            get => _audioSource != null ? _audioSource.clip : null;
            set
            {
                Safe.Execute(_audioSource, () => _audioSource.clip = value);
            }
        }

        /// <summary>
        /// Is based on <see cref="AudioSource.timeSamples"/> because <see cref="AudioSource.time"/> is not reliable when not playing.
        /// </summary>
        public float time
        {
            get
            {
                if (_audioSource == null || _audioSource.clip == null)
                {
                    return 0.0f;
                }
                return ((float)_audioSource.timeSamples / _audioSource.clip.samples) * _audioSource.clip.length;
            }
            set
            {
                if (_audioSource != null)
                {
                    _audioSource.timeSamples = Mathf.FloorToInt((value / _audioSource.clip.length) * _audioSource.clip.samples);
                    _audioSource.time = value;
                }
            }
        }

        public float length 
        { 
            get
            {
                if (_audioSource == null || _audioSource.clip == null)
                {
                    return 0.0f;
                }
                return _audioSource.clip.length;
            }
        }

        public float volume 
        {
            get => Safe.Get(_audioSource, 0.0f, () => _audioSource.volume);
            set => Safe.Execute(_audioSource, () => _audioSource.volume = value);
        }

        public bool mute
        {
            get => Safe.Get(_audioSource, false, () => _audioSource.mute);
            set => Safe.Execute(_audioSource, () => _audioSource.mute = value);
        }

        [SerializeField]
        private AUEEvent<AudioSource> _onAudioSourceSet;

        void OnValidate()
        {
            NotifyAudioSourceSet();
        }

        private void Start()
        {
            NotifyAudioSourceSet();
        }

        public void Play() => Safe.Execute(_audioSource, _audioSource.Play);
        public void Pause() => Safe.Execute(_audioSource, _audioSource.Pause);
        public void Resume()
        {
            Safe.Execute(_audioSource, (audio) =>
            {
                // Try unpause
                audio.UnPause();
                if (!audio.isPlaying)
                {
                    // If it fails, try to play from start
                    audio.timeSamples = 0;
                    audio.Play();
                }
            });
        }
        public void Stop() => Safe.Execute(_audioSource, _audioSource.Stop);

        public void NormalizedSeek(float t)
        {
            Safe.Execute(_audioSource, () =>
            {
                if (_audioSource.clip != null)
                {
                    t = Mathf.Clamp(t, 0, 0.99f); // Because 1 is an invalid seek position
                    _audioSource.timeSamples = Mathf.FloorToInt(t * _audioSource.clip.samples);
                }
            });
        }

        private void NotifyAudioSourceSet()
        {
            _onAudioSourceSet?.Invoke(_audioSource);
        }
    }
}
