using ChainBehaviors.Utils;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace ChainBehaviors.Processes
{
    /// <summary>
    /// Applies transform modification each frame
    /// </summary>
    [AddComponentMenu(CBConstants.ModuleProcessors + "Transform Modifier Process")]
    public class TransformModifierProcess : BaseProcess
    {
        [Flags]
        public enum EBitmaskAxis
        {
            X = 1 << 1,
            Y = 1 << 2,
            Z = 1 << 3,
        }

        [System.Serializable]
        public class ProjectionSettings
        {
            [SerializeField, ToggleGroup("_enabled", "ProjectionSettings")]
            private bool _enabled = false;
            public bool Enabled => _enabled;

            [SerializeField]
            [ToggleGroup("_enabled")]
            [EnumToggleButtons]
            private EBitmaskAxis _axis = EBitmaskAxis.X;
            public EBitmaskAxis Axis => _axis;
        }

        [Flags]
        public enum ETransformMode
        {
            Position = 0x01,
            Rotation = 0x02
        }

        [SerializeField, Tooltip("The transform to change. Properties that won't be changed will use these ones.")]
        private TransformReference _source = null;

        [SerializeField, Tooltip("The transform to use to calculate the new one.")]
        private TransformReference _target = null;

        [SerializeField, Tooltip("If empty, the source will be used as result")]
        private TransformReference _result = null;

        [SerializeField]
        private ETransformMode _transformMode = ETransformMode.Position | ETransformMode.Rotation;

        [SerializeField, InlineProperty, HideLabel]
        private ProjectionSettings _projection = null;

        private Transform SourceTrans => _source.Value;
        private Transform TargetTrans => _target.Value;
        private Transform ResultTrans => ((_result != null && _result.IsValueDefined && _result.Value != null) ? _result.Value : SourceTrans);

        public override void UpdateProcess(float deltaTime)
        {
            if (!AreReferencesValid())
            {
                return;
            }

            if (_transformMode.HasFlag(ETransformMode.Position))
            {
                Vector3 resultPos = TargetTrans.position;
                if (_projection.Enabled)
                {
                    ApplyProjection(ref resultPos);
                }

                ResultTrans.position = resultPos;
            }
            else
            {
                ResultTrans.position = SourceTrans.position;
            }

            if (_transformMode.HasFlag(ETransformMode.Rotation))
            {
                ResultTrans.rotation = TargetTrans.rotation;
            }
            else
            {
                ResultTrans.rotation = SourceTrans.rotation;
            }
        }

        private void ApplyProjection(ref Vector3 resultPos)
        {
            resultPos = SourceTrans.position;

            Vector3 projectionPosition = TargetTrans.position;

            var axis = _projection.Axis;
            if (axis.HasFlag(EBitmaskAxis.X))
            {
                resultPos.x = projectionPosition.x;
            }
            else if (axis.HasFlag(EBitmaskAxis.Y))
            {
                resultPos.y = projectionPosition.y;
            }
            else if (axis.HasFlag(EBitmaskAxis.Z))
            {
                resultPos.z = projectionPosition.z;
            }
        }

        private bool AreReferencesValid()
            => (_source != null && _source.IsValueDefined && _source.Value != null) &&
            (_target != null && _target.IsValueDefined && _target.Value != null);
    }
}