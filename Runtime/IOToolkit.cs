using UNIHper;
using UnityEngine;
using UnityEngine.Events;
using System.IO;

#if ENABLE_UNIHper
using UniRx;
#endif

namespace IOToolkit
{
    public class IOToolkit : MonoBehaviour
    {
        private static IOToolkit instance = null;
        public static IOToolkit Instance => AutoBuild();
        public UnityEvent OnUpdateEvent = new UnityEvent();

        // 自动加载
        public static IOToolkit AutoBuild()
        {
            if (instance == null)
            {
                IODeviceController.Load();
#if UNITY_2023_1_OR_NEWER
                instance = FindFirstObjectByType<IOToolkit>();
#else
                instance = FindObjectOfType<IOToolkit>();
#endif
                if (instance == null)
                {
                    instance = new GameObject("IOToolkit").AddComponent<IOToolkit>();
                    DontDestroyOnLoad(instance.gameObject);
                }
                instance.gameObject.name = "__IOToolkit";
            }
            return instance;
        }

        void Start()
        {
            CreateShortcuts();
        }

        public static void CreateShortcuts()
        {
#if ENABLE_UNIHper && !UNITY_EDITOR
            Managements.Framework
                .OnInitializedAsObservable()
                .Subscribe(_ =>
                {
                    var shortcutDir = PathUtils.GetProjectPath();
#if UNITY_64
                    var _targetDir = PathUtils.GetPluginsPath("x86_64");
#else
                    var _targetDir = PathUtils.GetPluginsPath("x86");
#endif
                    Managements.Framework.CreateRelativeShortcut(
                        "IOToolkit",
                        shortcutDir,
                        _targetDir,
                        PathUtils.GetPluginsPath(),
                        "调试IOToolkit"
                    );
                });
#endif
        }

        private void Update()
        {
            IODeviceController.Update();
            OnUpdateEvent.Invoke();
        }

        private void OnDestroy()
        {
            IODeviceController.Unload();
            OnUpdateEvent.RemoveAllListeners();
        }
    }
}
