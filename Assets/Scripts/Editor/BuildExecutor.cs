using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Djn.Builds {
    public static class BuildExecutor {
        public static void Build(BuildData data) {
            Debug.Log("Building application.");

            if(data == null) throw new ArgumentNullException();
            if(!data.IsValid) throw new ArgumentException();

            // Get build destination.
            var ext = "";
            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android) {
                ext = "apk";
            } else {
                ext = "exe";
            }
            var dstPath = EditorUtility.SaveFilePanel("Select Build Destination", "", data.Name + "." + ext, ext);
            if (string.IsNullOrEmpty(dstPath)) {
                Debug.Log("Build cancelled.");
                return;
            }

            if (!BuildPlayer(data, dstPath)) {
                Debug.LogWarning("Build player did not complete.");
                return;
            }

            var directoryPath = Path.GetDirectoryName(dstPath);
            var streamingPath = Path.Combine(
                directoryPath,
                Path.GetFileNameWithoutExtension(dstPath));
            streamingPath += "_Data";
            streamingPath = Path.Combine(streamingPath, "StreamingAssets", "BuildDataAssetBundle");
            if(Directory.Exists(streamingPath)) {
                Directory.Delete(streamingPath, true);
            }
            Directory.CreateDirectory(streamingPath);

            BuildAssetBundle(data, streamingPath);
      
            System.Diagnostics.Process.Start("explorer.exe", directoryPath);
        }

        private static bool BuildPlayer(BuildData data, string dstPath) {
            // build with scene list and build options.
            var buildOptions = BuildOptions.None;
            if (EditorUserBuildSettings.development) buildOptions |= BuildOptions.Development;
            if (EditorUserBuildSettings.development && EditorUserBuildSettings.allowDebugging) buildOptions |= BuildOptions.AllowDebugging;

            var buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.locationPathName = dstPath;
            buildPlayerOptions.options = buildOptions;
            buildPlayerOptions.scenes = data.CompleteSceneList.Select(x => x.Path).ToArray();
            buildPlayerOptions.target = EditorUserBuildSettings.activeBuildTarget;
      
            var report = BuildPipeline.BuildPlayer(buildPlayerOptions);

            if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded) return true;
            return false;
        }

        public static void BuildAssetBundle(BuildData data, string dstDir) {
            var bundleName = "builddata";

            //TODO: validate that build data and it's levels are real files.

            // Temporarily clear old BuildData bundle objects.
            // TODO: This will build all assets with an assetbundle name.
            // We should clear ALL bundle names so that we only build the builddata bundle.
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