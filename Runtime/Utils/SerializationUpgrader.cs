using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ChainBehaviors.Utils
{
    public static class SerializationUpgrader
    {
        /// <summary>
        /// Force reserialization on all scenes and prefabs of the project.
        /// </summary>
        public static void ReserializeScenesAndPrefabs()
        {
#if UNITY_EDITOR
            var allInstances = AssetDatabase.FindAssets("t:scene")
                .Concat(AssetDatabase.FindAssets("t:prefab"))
                .Select(AssetDatabase.GUIDToAssetPath)
                .ToArray();
            AssetDatabase.ForceReserializeAssets(allInstances, ForceReserializeAssetsOptions.ReserializeAssets);
#endif
        }
    }
}
