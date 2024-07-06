using UnityEngine;
using UnityEngine.Rendering;

namespace FrostEngine
{
    public sealed class EnvironmentInformationWindow : ScrollableDebuggerWindowBase
    {
        private RootModules _mRootModule = null;

        private ResourceModule _mResourceModule = null;

        public override void Initialize(params object[] args)
        {
            _mRootModule = ModuleSystem.GetModule<RootModules>();
            if (_mRootModule == null)
            {
                Debug.LogError("Base component is invalid.");
                return;
            }

            _mResourceModule = ModuleSystem.GetModule<ResourceModule>();
            if (_mResourceModule == null)
            {
                Debug.LogError("Resource component is invalid.");
                return;
            }
        }

        protected override void OnDrawScrollableWindow()
        {
            GUILayout.Label("<b>Environment Information</b>");
            GUILayout.BeginVertical("box");
            {
                DrawItem("Product Name", Application.productName);
                DrawItem("Company Name", Application.companyName);
#if UNITY_5_6_OR_NEWER
                DrawItem("Game Identifier", Application.identifier);
#else
                    DrawItem("Game Identifier", Application.bundleIdentifier);
#endif
                DrawItem("Game Framework Version", Version.GameFrameworkVersion);
                DrawItem("Game Version", StringUtils.Format("{} ({})", Version.GameVersion, Version.InternalGameVersion));
                DrawItem("Resource Version", (string.IsNullOrEmpty(_mResourceModule.ApplicableGameVersion) ? 
                    "Unknown" : StringUtils.Format("{} ({})", _mResourceModule.ApplicableGameVersion, _mResourceModule.InternalResourceVersion)));
                DrawItem("Application Version", Application.version);
                DrawItem("Unity Version", Application.unityVersion);
                DrawItem("Platform", Application.platform.ToString());
                DrawItem("System Language", Application.systemLanguage.ToString());
                DrawItem("Cloud Project Id", Application.cloudProjectId);
                DrawItem("Build Guid", Application.buildGUID);
                DrawItem("Target Frame Rate", Application.targetFrameRate.ToString());
                DrawItem("Internet Reachability", Application.internetReachability.ToString());
                DrawItem("Background Loading Priority", Application.backgroundLoadingPriority.ToString());
                DrawItem("Is Playing", Application.isPlaying.ToString());
                DrawItem("Splash Screen Is Finished", SplashScreen.isFinished.ToString());
                DrawItem("Run In Background", Application.runInBackground.ToString());
                DrawItem("Install Name", Application.installerName);
                DrawItem("Install Mode", Application.installMode.ToString());
                DrawItem("Sandbox Type", Application.sandboxType.ToString());
                DrawItem("Is Mobile Platform", Application.isMobilePlatform.ToString());
                DrawItem("Is Console Platform", Application.isConsolePlatform.ToString());
                DrawItem("Is Editor", Application.isEditor.ToString());
                DrawItem("Is Focused", Application.isFocused.ToString());
                DrawItem("Is Batch Mode", Application.isBatchMode.ToString());
            }
            GUILayout.EndVertical();
        }
    }
}