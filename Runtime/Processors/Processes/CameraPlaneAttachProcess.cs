using UnityEngine;

namespace ChainBehaviors.Processes
{
    [ExecuteInEditMode]
    public class CameraPlaneAttachProcess : BaseProcess
    {
        [SerializeField]
        private Camera _camera = null;

        [SerializeField, Range(0, 1)]
        private float _ratioDistance = 0.01f;

        [SerializeField]
        private Vector3 _scaleFactor = Vector3.one;


        public override void UpdateProcess(float deltaTime)
        {
            if (_camera == null)
            {
                return;
            }

            float distance = Mathf.Lerp(_camera.nearClipPlane, _camera.farClipPlane, _ratioDistance);
            Transform cameraTrans = _camera.transform;
            Vector3 position = cameraTrans.position + cameraTrans.forward * distance;
            Quaternion rotation = Quaternion.LookRotation(cameraTrans.forward, cameraTrans.up);

            // https://docs.unity3d.com/Manual/FrustumSizeAtDistance.html
            float frustumHeight = 2.0f * distance * Mathf.Tan(_camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
            float frustumWidth = frustumHeight * _camera.aspect;

            Vector3 scale = new Vector3(frustumWidth, frustumHeight, 1);
            scale.Scale(_scaleFactor);

            this.transform.SetPositionAndRotation(position, rotation);
            this.transform.localScale = scale;
        }
    }
}
