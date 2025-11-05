using System.IO;
using UnityEngine;

namespace IOToolkit
{
    public static class IOToolkitUtil
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        static void IOToolkitInitialize()
        {
            try
            {
                IOSettings.SetIOConfigPath(ConfigPath);
                IOSettings.SetIOLogDir(LogDir);
            }
            catch (System.Exception) { }
        }

        public static string ConfigPath
        {
            get
            {
#if UNITY_EDITOR
                return Path.Combine(Application.dataPath, "Plugins/IOToolkit/Config/IODevice.xml");
#elif UNITY_64 && UNITY_STANDALONE_WIN
                return Path.Combine(Application.dataPath, $"Plugins/x86_64/Config/IODevice.xml");
#elif !UNITY_64 && UNITY_STANDALONE_WIN
                return Path.Combine(Application.dataPath, $"Plugins/x86/Config/IODevice.xml");
#endif
            }
        }

        public static string LogDir
        {
            get
            {
                var _appDir = Path.GetDirectoryName(Application.dataPath);
#if UNITY_EDITOR
                return Path.Combine(_appDir, "Logs/IOToolkit");
#elif UNITY_STANDALONE_WIN
                return Path.Combine(_appDir, "Logs");
#endif
            }
        }
    }
}
