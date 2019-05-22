using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Djn.Builds {
    public static class BuildExecutor {
        public static void Build(BuildData data) {
            if(data == null) throw new ArgumentNullException();
            if(!data.IsValid) throw new ArgumentException();

            // Get build destination.
            var dstPath = EditorUtility.SaveFilePanel("Select Build Destination", "", data.Name + ".exe", "exe");
            if (string.IsNullOrEmpty(dstPath)) {
                return;
            }
            
            BuildPlayer(data, dstPath);

            var streamingPath = Path.Combine(
                Path.GetDirectoryName(dstPath),
                Path.GetFileNameWithoutExtension(dstPath));
            streamingPath += "_Data";
            streamingPath = Path.Combine(streamingPath, "StreamingAssets");
            if(!Directory.Exists(streamingPath)) {
                Directory.CreateDirectory(streamingPath);
            }

            BuildAssetBundle(data, streamingPath);
        }

        private static void BuildPlayer(BuildData data, string dstPath) {
            Debug.Log("Building player.");
            // build with scene list and build options.
            var buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.locationPathName = dstPath;
            // TODO: Should options be a popup after "Build" is clicked in a target?
            buildPlayerOptions.options = BuildOptions.Development | BuildOptions.AllowDebugging;
            buildPlayerOptions.scenes = data.CompleteSceneList.Select(x => x.Path).ToArray();
            buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
            BuildPipeline.BuildPlayer(buildPlayerOptions);
        }

        public static void BuildAssetBundle(BuildData data, string dstDir) {
            Debug.Log("Building asset bundles.");
            var bundleName = "builddata";

            //TODO: validate that build data and it's levels are real files.

            // Temporarily clear old BuildData bundle objects.
            var originalBundleAssetPaths = AssetDatabase.GetAssetPathsFromAssetBundle(bundleName);
            foreach(var assetPath in originalBundleAssetPaths) {
                var assetImporter = AssetImporter.GetAtPath(assetPath);
                assetImporter.assetBundleName = "";
                assetImporter.SaveAndReimport();
            }

            // Set current build data bundle names.
            var buildDataPaths = new HashSet<string>();
            var pathToOriginalBundleName = new Dictionary<string, string>();
            buildDataPaths.Add(AssetDatabase.GetAssetPath(data));
            foreach(var levelData in data.LevelDatas) {
                buildDataPaths.Add(AssetDatabase.GetAssetPath(levelData));
            }
            foreach(var path in buildDataPaths) {
                var assetImporter = AssetImporter.GetAtPath(path);
                pathToOriginalBundleName.Add(path, assetImporter.assetBundleName);
                assetImporter.assetBundleName = bundleName;
                assetImporter.SaveAndReimport();
            }

            // Build the bundle.
            BuildPipeline.BuildAssetBundles(
                dstDir,
                BuildAssetBundleOptions.None,
                BuildTarget.StandaloneWindows64);

            // Undo all build data bundle changes.
            foreach(var path in buildDataPaths) {
                var assetImporter = AssetImporter.GetAtPath(path);
                assetImporter.assetBundleName = pathToOriginalBundleName[path];
                assetImporter.SaveAndReimport();
            }
            foreach(var assetPath in originalBundleAssetPaths) {
                var assetImporter = AssetImporter.GetAtPath(assetPath);
                assetImporter.assetBundleName = bundleName;
                assetImporter.SaveAndReimport();
            }
        }
    }
}