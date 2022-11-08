#if USE_XR_MANAGEMENT

using AUE;
using ChainBehaviors.Utils;
using UnityEngine;
using UnityEngine.XR.Management;

namespace ChainBehaviors.XR
{
    [AddComponentMenu(CBConstants.ModuleXRPath + "VR Mode Watcher")]
    public class VRModeWatcher : BaseMethod
    {
        [SerializeField]
        private AUEEvent<bool> _isVREnabled = null;

        public bool IsVREnabled => (XRGeneralSettings.Instance?.InitManagerOnStart ?? false);

        void Start()
        {
            Trace(("Is VR enabled?", IsVREnabled));
            _isVREnabled.Invoke(IsVREnabled);
        }
    }
}

#endif