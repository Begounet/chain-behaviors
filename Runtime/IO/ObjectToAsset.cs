#pragma warning disable 414 // The field 'ObjectToAsset._outputAsset' is assigned but its value is never used

using AppTools.FileManagement;
using UnityEngine;
using ChainBehaviors.Utils;
#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif

namespace ChainBehaviors.IO
{
    /// <summary>
    /// Allow to save an UnityObject as an asset into the project.
    /// So it is only useful in edit mode!
    /// </summary>
    /// <example>
    /// You could use it to save an AudioClip made by a microphone capture so you can re-use it again.
    /// </example>
    [AddComponentMenu(CBConstants.ModuleIOPath + "Object To Asset")]
    public class ObjectToAsset : BaseMethod
    {
        [SerializeField]
        private AppAssetFilePath _outputAsset = null;


        public void Save(Object obj)
        {
#if UNITY_EDITOR
            string dir = Path.GetDirectoryName(_outputAsset.AbsoluteFilePath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            // If it is an original asset...
            string originalObjPath = AssetDatabase.GetAssetPath(obj);
            if (string.IsNullOrEmpty(originalObjPath))
            {
                TraceCustomMethodName("create asset", ("object", obj), ("destination path", _outputAsset.RelativeFilePath));
                AssetDatabase.CreateAsset(obj, _outputAsset.RelativeFilePath);
            }
            // ... otherwise, copy the asset from the source
            else
            {
                TraceCustomMethodName("copy asset", ("object", obj), ("from asset path", originalObjPath), ("destination path", _outputAsset.RelativeFilePath));
                AssetDatabase.CopyAsset(originalObjPath, _outputAsset.RelativeFilePath);
            }
#endif
        }
    }
}