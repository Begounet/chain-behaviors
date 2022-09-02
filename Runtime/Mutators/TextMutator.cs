using Sirenix.OdinInspector;
using System;
using System.IO;
using System.Text;
using UnityEngine;
using AUE;

namespace ChainBehaviors.Mutators
{
    public class TextMutator : MonoBehaviour
    {
        [Flags]
        public enum EPathMask
        {
            None = 0x00,
            Directory = 0x01,
            Filename = 0x02,
            Extension = 0x04,
            All = Directory | Filename | Extension
        }

        public enum ETruncateMode
        {
            None,
            Truncate,
            Ellipsis
        }

#if UNITY_EDITOR
        [ShowInInspector, CustomValueDrawer("DrawPreviewText")]
        private string PreviewText;
#endif

        [SerializeField]
        private EPathMask _pathMask = EPathMask.None;


        [SerializeField]
        private ETruncateMode _truncateMode = ETruncateMode.None;

        [SerializeField, Indent, Min(1), ShowIf("@_truncateMode != ETruncateMode.None")]
        private int _numMaxCharacters = 10;

        [SerializeField]
        private AUEEvent<string> _mutated = null;


        public void MutateText(string text)
        {
            _mutated.Invoke(InternalMutateText(text));
        }

        private string InternalMutateText(string text)
        {
            if (_pathMask != EPathMask.None)
            {
                ApplyPathMask(ref text);
            }
            if (_truncateMode != ETruncateMode.None)
            {
                ApplyTruncate(ref text);
            }

            return text;
        }

        private void ApplyPathMask(ref string text)
        {

            StringBuilder sb = new StringBuilder();
            if (_pathMask.HasFlag(EPathMask.Directory))
            {
                try
                {
                    string directory = Path.GetDirectoryName(text);
                    sb.Append(directory);
                }
                catch { }
            }
            if (_pathMask.HasFlag(EPathMask.Filename))
            {
                if (sb.Length > 0)
                {
                    sb.Append("/");
                }

                try
                {
                    string filename = Path.GetFileNameWithoutExtension(text);
                    sb.Append(filename);
                }
                catch { }
            }
            if (_pathMask.HasFlag(EPathMask.Extension))
            {
                try
                {
                    string extension = Path.GetExtension(text);
                    sb.Append(extension);
                }
                catch { }
            }

            text = sb.ToString();
        }

        private void ApplyTruncate(ref string text)
        {
            if (text.Length <= _numMaxCharacters)
            {
                return;
            }

            switch (_truncateMode)
            {
                case ETruncateMode.Truncate:
                    text = text.Remove(_numMaxCharacters);
                    break;

                case ETruncateMode.Ellipsis:
                    text = text.Remove(Mathf.Max(0, _numMaxCharacters - 3)) + "...";
                    break;

                default:
                case ETruncateMode.None:
                    break;
            }
        }

#if UNITY_EDITOR
        private string DrawPreviewText(string text, GUIContent label)
        {
            text = UnityEditor.EditorGUILayout.TextField(label, text);
            UnityEditor.EditorGUI.BeginDisabledGroup(true);
            {
                UnityEditor.EditorGUILayout.LabelField("Preview result", InternalMutateText(text));
            }
            UnityEditor.EditorGUI.EndDisabledGroup();
            return text;
        }
#endif
    }
}