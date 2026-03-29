using UnityEngine;
using System.IO;

namespace UnityEditor.Build.Pipeline.Utilities
{
    /// <summary>
    /// Static class containing per project build settings.
    /// </summary>
    public static class ScriptableBuildPipeline
    {
        [System.Serializable]
        internal class Settings
        {
            public bool useBuildCacheServer = false;
            public string cacheServerHost = "";
            public int cacheServerPort = 8126;
            public bool threadedArchiving = true;
            public bool logCacheMiss = false;
            public bool logAssetWarnings = true;
            public bool slimWriteResults = true;
            public int maximumCacheSize = 20;
            public bool useDetailedBuildLog = false;
#if UNITY_2021_1_OR_NEWER
            public bool useV2Hasher = true;
#elif UNITY_2020_1_OR_NEWER
            public bool useV2Hasher = false;
#endif
            public int fileIDHashSeed = 0;
            public int prefabPackedHeaderSize = 2;
        }

        internal static Settings s_Settings = new Settings();

        /// <summary>
        /// Flag to determine if the Build Cache Server is to be used.
        /// </summary>
        public static bool UseBuildCacheServer
        {
            get => s_Settings.useBuildCacheServer;
            set => CompareAndSet(ref s_Settings.useBuildCacheServer, value);
        }

        /// <summary>
        /// The host of the Build Cache Server.
        /// </summary>
        public static string CacheServerHost
        {
            get => s_Settings.cacheServerHost;
            set => CompareAndSet(ref s_Settings.cacheServerHost, value);
        }

        /// <summary>
        /// The port number for the Build Cache Server.
        /// </summary>
        public static int CacheServerPort
        {
            get => s_Settings.cacheServerPort;
            set => CompareAndSet(ref s_Settings.cacheServerPort, value);
        }

        /// <summary>
        /// Thread the archiving and compress build stage.
        /// </summary>
        public static bool threadedArchiving
        {
            get => s_Settings.threadedArchiving;
            set => CompareAndSet(ref s_Settings.threadedArchiving, value);
        }

        /// <summary>
        /// Log a warning on build cache misses. Warning will contain which asset and dependency caused the miss.
        /// </summary>
        public static bool logCacheMiss
        {
            get => s_Settings.logCacheMiss;
            set => CompareAndSet(ref s_Settings.logCacheMiss, value);
        }

        /// <summary>
        /// Log a warning on invalid asset references.
        /// </summary>
        public static bool logAssetWarnings
        {
            get => s_Settings.logAssetWarnings;
            set => CompareAndSet(ref s_Settings.logAssetWarnings, value);
        }

        /// <summary>
        /// Reduces the caching of WriteResults data down to the bare minimum for improved cache performance.
        /// </summary>
        public static bool slimWriteResults
        {
            get => s_Settings.slimWriteResults;
            set => CompareAndSet(ref s_Settings.slimWriteResults, value);
        }

        /// <summary>
        /// The size of the Build Cache folder will be kept below this maximum value when possible.
        /// </summary>
        public static int maximumCacheSize
        {
            get => s_Settings.maximumCacheSize;
            set => CompareAndSet(ref s_Settings.maximumCacheSize, value);
        }

        /// <summary>
        /// Set this to true to write more detailed event information in the build log. Set to false to only write basic event information.
        /// </summary>
        public static bool useDetailedBuildLog
        {
            get => s_Settings.useDetailedBuildLog;
            set => CompareAndSet(ref s_Settings.useDetailedBuildLog, value);
        }

        /// <summary>
        /// Set this to true to use the same hasher as Asset Database V2. This hasher improves build cache performance, but invalidates the existing build cache.
        /// </summary>
#if UNITY_2020_1_OR_NEWER
        public static bool useV2Hasher
        {
            get => s_Settings.useV2Hasher;
            set => CompareAndSet(ref s_Settings.useV2Hasher, value);
        }
#endif

        // Internal as we don't want to allow setting these via API. We want to ensure they are saved to json, and checked in to the project version control.
        internal static int fileIDHashSeed
        {
            get => s_Settings.fileIDHashSeed;
            set => CompareAndSet(ref s_Settings.fileIDHashSeed, value);
        }

        // Internal as we don't want to allow setting these via API. We want to ensure they are saved to json, and checked in to the project version control.
        internal static int prefabPackedHeaderSize
        {
            get => Mathf.Clamp(s_Settings.prefabPackedHeaderSize, 1, 4);
            set => CompareAndSet(ref s_Settings.prefabPackedHeaderSize, Mathf.Clamp(value, 1, 4));
        }

        static void CompareAndSet<T>(ref T property, T value)
        {
            if (property.Equals(value))
                return;

            property = value;
            SaveSettings();
        }

        internal const string kSettingPath = "ProjectSettings/ScriptableBuildPipeline.json";
        internal const string kLogAssetWarningsKey = "ScriptableBuildPipeline.LogAssetWarnings";

        internal static void LoadSettings()
        {
            // Load new settings from Json
            if (File.Exists(kSettingPath))
            {
                // Existing projects should keep previous defaults that are now settings
                s_Settings.prefabPackedHeaderSize = 4;

                var json = File.ReadAllText(kSettingPath);
                EditorJsonUtility.FromJsonOverwrite(json, s_Settings);

                ApplyLogAssetWarningsSetting();
            }
        }

        internal static void SaveSettings()
        {
            var json = EditorJsonUtility.ToJson(s_Settings, true);
            File.WriteAllText(kSettingPath, json);

            ApplyLogAssetWarningsSetting();
        }

        internal static void ApplyLogAssetWarningsSetting()
        {
            // Save setting in EditorPrefs so that it can be accessed from engine code
            EditorPrefs.SetBool(kLogAssetWarningsKey, s_Settings.logAssetWarnings);
        }

        static ScriptableBuildPipeline()
        {
            LoadSettings();
        }
    }
}
