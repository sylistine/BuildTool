using System.IO;
using UnityEngine;
using Djn.Builds;

namespace Djn {
    /// <summary>
    /// Class takes care of build data asset loading. Serves as a one-stop-shop for accessing info about levels currently built into the app.
    /// </summary>
    public static class Application {
        private static bool _inited = false;
        private static AssetBundle _buildDataBundle;
        private static BuildData _buildData;

        public static BuildData BuildData {
            get {
                if(!_inited) throw new System.Exception();
                return _buildData;
            }
        }

        public static void Init() {
            if(_inited) return;

            var buildDataBundlePath = Path.Combine(
              UnityEngine.Application.streamingAssetsPath,
              "BuildDataAssetBundle",
              "builddata");
            var buildDataBundle = AssetBundle.LoadFromFile(buildDataBundlePath);
            if(buildDataBundle == null) {
                _inited = true;
                Teardown();
                buildDataBundle = AssetBundle.LoadFromFile(buildDataBundlePath);
                if(buildDataBundle == null) {
                    Debug.LogError("Unable to load build data.");
                    return;
                }
            }
            
            var allBuildData = buildDataBundle.LoadAllAssets<BuildData>();
            if(allBuildData.Length == 0) {
                Debug.LogError("Build data bundle does not contain build data.");
                buildDataBundle.Unload(true);
                return;
            } else if (allBuildData.Length > 1) {
                Debug.LogError("Build data bundle contains more than one build data.");
                buildDataBundle.Unload(true);
                return;
            }

            // All green.
            _buildDataBundle = buildDataBundle;
            _buildData = allBuildData[0];

            _inited = true;
        }

        public static void Teardown() {
            if(!_inited) return;

            try {
                _buildDataBundle.Unload(true);
            } catch (MissingReferenceException) {
                Debug.LogWarning("Trying to unload the build data bundle, but it has already been unloaded.");
            }
            _inited = false;
        }
    }
}
