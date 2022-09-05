using AUE;
using ChainBehaviors.Utils;
using UnityEngine;

namespace ChainBehaviors.Extractors
{
    [AddComponentMenu(CBConstants.ModuleExtractorsPath + "Extract Audio Clip Name")]
    public class ExtractAudioClipName : BaseMethod
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
            Trace(("clip", clip), ("name", clipName));
            _onExtracted.Invoke(clipName);
        }
    }
}