using AUE;
using UnityEngine;

namespace ChainBehaviors.Extractors
{
    public class ExtractAudioClipName : MonoBehaviour
    {
        [SerializeField]
        private string _defaultText = string.Empty;
        public string DefaultText 
        {
            get => _defaultText;
            set => _defaultText = value;
        }

        [SerializeField]
        private AUEEvent<string> _onExtracted = null;


        public void Extract(AudioClip clip)
        {
            string clipName;
            if (clip != null)
            {
                clipName = clip.name;
            }
            else
            {
                clipName = _defaultText;
            }
            _onExtracted.Invoke(clipName);
        }
    }
}