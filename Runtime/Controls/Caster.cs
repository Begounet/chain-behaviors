#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
using System;
using UnityEngine;
using AUE;

namespace ChainBehaviors
{
    /// <summary>
    /// Cast a value to another type
    /// </summary>
    [AddComponentMenu(CBConstants.ModuleControlsPath + "Caster")]
    public class Caster : BaseMethod
    {
        public enum Type
        {
            Bool,
            Int,
            Float,
            String,
            AudioClip,
            Collider,
            Camera,
            Texture,
            Object,
        }

        [SerializeField]
        private Type _type = Type.Bool;

        [SerializeField, LabelText("Casted"), ShowIf("_type", Type.Bool)]
        private AUEEvent<bool> _boolCast = null;

        [SerializeField, LabelText("Casted"), ShowIf("_type", Type.Int)]
        private AUEEvent<int> _intCast = null;

        [SerializeField, LabelText("Casted"), ShowIf("_type", Type.Float)]
        private AUEEvent<float> _floatCast = null;

        [SerializeField, LabelText("Casted"), ShowIf("_type", Type.String)]
        private AUEEvent<string> _stringCast = null;

        [SerializeField, LabelText("Casted"), ShowIf("_type", Type.AudioClip)]
        private AUEEvent<AudioClip> _audioClipCast = null;

        [SerializeField, LabelText("Casted"), ShowIf("_type", Type.Collider)]
        private AUEEvent<Collider> _colliderCast = null;

        [SerializeField, LabelText("Casted"), ShowIf("_type", Type.Camera)]
        private AUEEvent<Camera> _cameraCast = null;

        [SerializeField, LabelText("Casted"), ShowIf("_type", Type.Texture)]
        private AUEEvent<Texture> _textureCast = null;

        [SerializeField, LabelText("Casted"), ShowIf("_type", Type.Object)]
        private AUEEvent<object> _objectCast = null;

        public void Cast(object obj) => InternCast(obj);
        public void Cast(bool obj) => InternCast(obj);
        public void Cast(int obj) => InternCast(obj);
        public void Cast(float obj) => InternCast(obj);
        public void Cast(string obj) => InternCast(obj);
        public void Cast(AudioClip obj) => InternCast(obj);
        public void Cast(Collider obj) => InternCast(obj);
        public void Cast(Camera obj) => InternCast(obj);
        public void Cast(Texture obj) => InternCast(obj);

        private void InternCast(object obj)
        {
            Trace(("source", obj), ("type", _type));

            switch (_type)
            {
                case Type.Object: _objectCast.Invoke(obj); break;
                case Type.Bool: _boolCast.Invoke(Convert.ToBoolean(obj)); break;
                case Type.Int: _intCast.Invoke(Convert.ToInt32(obj)); break;
                case Type.Float: _floatCast.Invoke(Convert.ToSingle(obj)); break;
                case Type.String: _stringCast.Invoke(Convert.ToString(obj)); break;
                case Type.AudioClip: _audioClipCast.Invoke(obj as AudioClip); break;
                case Type.Collider: _colliderCast.Invoke(obj as Collider); break;
                case Type.Camera: _cameraCast.Invoke(obj as Camera); break;
                case Type.Texture: _textureCast.Invoke(obj as Texture); break;

                default:
                    throw new Exception($"Unsupported type {_type}");
            }
        }
    }
}
#endif