using UnityEngine;

namespace UnityEditor.Build.Pipeline.Utilities
{
    /// <summary>
    /// Static class containing per project build settings.
    /// </summary>
    public static class ScriptableBuildPipeline
    {
        public static bool UseBuildCacheServer;
        public static string CacheServerHost = "";
        public static int CacheServerPort = 8126;
        public static bool threadedArchiving = true;
        public static bool logCacheMiss;
        public static bool logAssetWarnings = true;
        public static bool slimWriteResults = true;
        public static int maximumCacheSize = 20;
        public static bool useDetailedBuildLog;
#if UNITY_2021_1_OR_NEWER
        public static bool useV2Hasher = true;
#elif UNITY_2020_1_OR_NEWER
        public static bool useV2Hasher;
#endif
        internal static int fileIDHashSeed;
        internal static int prefabPackedHeaderSize = 2;

        static ScriptableBuildPipeline()
        {
            EditorPrefs.SetBool("ScriptableBuildPipeline.LogAssetWarnings", logAssetWarnings);
        }
    }
}
