﻿using AUE;
using ChainBehaviors.Proxy;
using ChainBehaviors.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace ChainBehaviors.Observers
{
    /// <summary>
    /// Observes the play state changes in an <see cref="AudioSource"/>
    /// </summary>
    [AddComponentMenu(CBConstants.ModuleObservers + "Audio Source State Observer")]
    public class AudioSourceStateObserver : BaseMethod
    {
        [SerializeField]
        private UnityObjectProxyable<AudioSource> _source = null;

        [SerializeField]
        private AUEEvent _onPlay = null;

        [SerializeField, Tooltip("Called when audio source is stopped or paused"), FormerlySerializedAs("_onStoppedAUE")]
        private AUEEvent _onStopped = null;

        private bool _wasPlaying = false;

        void Update()
        {
            bool isPlaying = _source.Value.isPlaying;
            if (!_wasPlaying && isPlaying)
            {
                TraceCustomMethodName("On Audio Play", ("source", _source));
                _onPlay?.Invoke();
            }

            if (_wasPlaying && !isPlaying)
            {
                TraceCustomMethodName("On Audio Stopped", ("source", _source));
                _onStopped?.Invoke();
            }

            _wasPlaying = isPlaying;
        }
    }
}