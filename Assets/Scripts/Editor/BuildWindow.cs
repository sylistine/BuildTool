﻿using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Djn.Builds {
    public class BuildWindow : EditorWindow {
        private const string NewBuildDataPath = "Assets/Settings/BuildData/BuildData.asset";
        private const string DefaultLevelPath = "Assets/Settings/BuildData/DefaultStartupScene.asset";
        //private float lineHeight = 12f;
        //private float headerHeight = 14f;

        [MenuItem("Window/Build")]
        public static void ShowWindow() {
            var window = EditorWindow.GetWindow<BuildWindow>();
            window.titleContent = new GUIContent("Build");
        }

        private Vector2 _scrollPosition;
        private int _currentBuildDataIdx = 0;
        private BuildData[] _buildData;

        private void Awake() {
            if(!AssetDatabase.IsValidFolder("Assets/Settings"))
                AssetDatabase.CreateFolder("Assets", "Settings");
            if(!AssetDatabase.IsValidFolder("Assets/Settings/BuildData"))
                AssetDatabase.CreateFolder("Assets/Settings", "BuildData");
            if (!AssetDatabase.IsValidFolder("Assets/Settings/BuildData/Editor"))
                AssetDatabase.CreateFolder("Assets/Settings/BuildData", "Editor");

            // Create default level asset if it doesn't exist.
            if (AssetDatabase.LoadAssetAtPath<LevelData>(DefaultLevelPath) == null) {
                var newLevel = CreateInstance<LevelData>();
                AssetDatabase.CreateAsset(newLevel, DefaultLevelPath);
                AssetDatabase.SaveAssets();
                var serializedLevel = new SerializedObject(newLevel);
                serializedLevel.FindProperty("_name").stringValue = "Default Startup Level";
                serializedLevel.ApplyModifiedProperties();
            }
        }

        private void OnGUI() {
            RefreshBuildDataReferences();

            // Setup styles and positions.
            var sidebarPosition = new Rect(-1f, -1f, 242f, position.height + 1f);
            var contentPosition = new Rect(sidebarPosition.width - 1f, -1f, position.width - 239f, position.height + 1f);
            
            SidebarGUI(sidebarPosition);

            if (_buildData.Length > 0) {
                BuildDataGUI(contentPosition, _buildData[_currentBuildDataIdx]);
            } else {
                BuildDataGUI(contentPosition, null);
            }
        }

        private void SidebarGUI(Rect position) {
            // TODO: scroll view.
            var sidebarStyle = new GUIStyle("box");
            GUI.Box(position, GUIContent.none, sidebarStyle);

            var paddedPosition = position;
            paddedPosition.width -= 4f;
            paddedPosition.height -= 4f;
            paddedPosition.x += 2f;
            paddedPosition.y += 2f;


            var headerStyle = new GUIStyle();
            headerStyle.fontSize = 14;
            headerStyle.normal.textColor = Color.white;

            var headerPadding = 6f;

            // Platform options.
            var insertPosition = paddedPosition;

            insertPosition.height = 20f;
            EditorGUI.LabelField(insertPosition, new GUIContent("Target Platform"), headerStyle);
            insertPosition.y += insertPosition.height;

            insertPosition.height = EditorGUIUtility.singleLineHeight;
            var buildTarget = (BuildTarget)EditorGUI.EnumPopup(insertPosition, EditorUserBuildSettings.activeBuildTarget);
            insertPosition.y += insertPosition.height;
            if (buildTarget != EditorUserBuildSettings.activeBuildTarget) {
                if (EditorUtility.DisplayDialog("Change Target Platform", "Switch platforms?", "Confirm", "Cancel")) {
                    BuildTargetGroup targetGroup;
                    switch(buildTarget) {
                      case BuildTarget.Android:
                        targetGroup = BuildTargetGroup.Android;
                        break;
                      default:
                        targetGroup = BuildTargetGroup.Standalone;
                        break;
                    }
                    if (EditorUserBuildSettings.SwitchActiveBuildTarget(targetGroup, buildTarget)) {
                        EditorUserBuildSettings.selectedBuildTargetGroup = targetGroup;
                    }
                }
            }


            insertPosition.y += headerPadding;
            insertPosition.height = 20f;
            EditorGUI.LabelField(insertPosition, new GUIContent("Build Options"), headerStyle);
            insertPosition.y += insertPosition.height;

            insertPosition.height = EditorGUIUtility.singleLineHeight;
            EditorUserBuildSettings.development = EditorGUI.Toggle(
                insertPosition, new GUIContent("Development Build"), EditorUserBuildSettings.development);
            insertPosition.y += insertPosition.height;

            EditorGUI.BeginDisabledGroup(!EditorUserBuildSettings.development);
            EditorUserBuildSettings.allowDebugging = EditorGUI.Toggle(
                insertPosition, new GUIContent("Script Debugging"), EditorUserBuildSettings.allowDebugging);
            EditorGUI.EndDisabledGroup();
            insertPosition.y += insertPosition.height;

            // Build target list.
            insertPosition.y += headerPadding;
            insertPosition.height = 20f;
            EditorGUI.LabelField(insertPosition, new GUIContent("Build Targets"), headerStyle);
            insertPosition.y += insertPosition.height;

            var newBuildDataButtonPosition = insertPosition;
            newBuildDataButtonPosition.height = EditorGUIUtility.singleLineHeight;
            if (GUI.Button(newBuildDataButtonPosition, new GUIContent("+"))) {
                CreateNewBuildData();
            }

            var buildItemPosition = newBuildDataButtonPosition;
            buildItemPosition.width -= EditorGUIUtility.singleLineHeight;
            var buildItemDeleterPosition = newBuildDataButtonPosition;
            buildItemDeleterPosition.width = EditorGUIUtility.singleLineHeight;
            buildItemDeleterPosition.x += buildItemPosition.width;
            for(var i = 0; i < _buildData.Length; ++i) {
                buildItemPosition.y += EditorGUIUtility.singleLineHeight;
                buildItemDeleterPosition.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.BeginDisabledGroup(i == _currentBuildDataIdx);
                if (GUI.Button(buildItemPosition, new GUIContent(_buildData[i].Name))) {
                    _currentBuildDataIdx = i;
                }
                EditorGUI.EndDisabledGroup();
                if (GUI.Button(buildItemDeleterPosition, new GUIContent("×"))) {
                    // TODO: Confirm window.
                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(_buildData[i]));
                    _currentBuildDataIdx = 0;
                }
            }
        }

        private void BuildDataGUI(Rect position, BuildData content) {
            if (content == null) {
                EditorGUI.LabelField(position, new GUIContent("No build data selected."));
                return;
            }

            position.width -= 6f;
            position.height -= 4f;
            position.y += 2f;
            position.x += 2f;
            var contentPosition = position;
            contentPosition.height -= EditorGUIUtility.singleLineHeight;
            var buildControlsRect = position;
            buildControlsRect.height = EditorGUIUtility.singleLineHeight;
            buildControlsRect.y += contentPosition.height;

            _scrollPosition = GUI.BeginScrollView(position, _scrollPosition, contentPosition);
            var buildDataVisualizer = new BuildDataVisualizer(content);
            buildDataVisualizer.OnGUI(contentPosition);
            GUI.EndScrollView();

            // TODO: Draw menubar elsewhere?
            var buttonRect = buildControlsRect;
            buttonRect.width = EditorGUIUtility.labelWidth;
            if(GUI.Button(buttonRect, new GUIContent("Build..."))) {
                if (!content.IsValid) {
                    // Catch missing scene asset references
                    // I.e:  gaps in subscene arrays.
                    EditorUtility.DisplayDialog("Unable to build player.",
                        "Unable to build " + content.Name + ".\nMake sure all scene references are valid.",
                        "Confirm");
                } else {
                    BuildExecutor.Build(content);
                }
            }
            
            // Set platform, scene list, and create BuildData for editor use.
            buttonRect.x += buttonRect.width;
            if(GUI.Button(buttonRect, new GUIContent("Enable Target"))) {
                var editorDataFolder = "Assets/StreamingAssets/BuildDataAssetBundle";

                if (!AssetDatabase.IsValidFolder("Assets/StreamingAssets"))
                  AssetDatabase.CreateFolder("Assets", "StreamingAssets");
                if (AssetDatabase.IsValidFolder(editorDataFolder)) {
                    FileUtil.DeleteFileOrDirectory("Assets/StreamingAssets/BuildDataAssetBundle");
                }

                AssetDatabase.CreateFolder("Assets/StreamingAssets", "BuildDataAssetBundle");
                BuildExecutor.BuildAssetBundle(content, editorDataFolder);

                var buildSettingsScenes = content.CompleteSceneList.Select(x =>
                    new EditorBuildSettingsScene(x.Path, true)
                    ).ToArray();
                EditorBuildSettings.scenes = buildSettingsScenes;
            }
            
            // Clear loaded bundle data.
            buttonRect.x += buttonRect.width;
            if (GUI.Button(buttonRect, new GUIContent("Unload Asset Bundles"))) {
                foreach(var bundle in AssetBundle.GetAllLoadedAssetBundles()) {
                    Debug.Log("Unloading bundle.");
                    bundle.Unload(true);
                }
            }
        }

        private void CreateNewBuildData() {
            var newBuildData = CreateInstance<BuildData>();

            var assetPath = AssetDatabase.GenerateUniqueAssetPath(NewBuildDataPath);
            AssetDatabase.CreateAsset(newBuildData, assetPath);
            AssetDatabase.SaveAssets();

            var serializedBuildData = new SerializedObject(newBuildData);
            var defaultLevel = AssetDatabase.LoadAssetAtPath<LevelData>(DefaultLevelPath);
            serializedBuildData.FindProperty("_startupLevel").objectReferenceValue = defaultLevel;
            serializedBuildData.ApplyModifiedProperties();
        }

        private void RefreshBuildDataReferences() {
            var buildDataFiles = AssetDatabase.FindAssets("BuildData");

            var buildDataFileList = new List<BuildData>();
            foreach(var file in buildDataFiles) {
                var path = AssetDatabase.GUIDToAssetPath(file);
                var asset = AssetDatabase.LoadAssetAtPath<BuildData>(path);
                if(asset != null) buildDataFileList.Add(asset);
            }
            var buildData = buildDataFileList.ToArray();
            // Should we reset the current build data index?
            if (_buildData == null ||
                _currentBuildDataIdx > buildData.Length - 1 ||
                _currentBuildDataIdx > _buildData.Length - 1 ||
                buildData[_currentBuildDataIdx] != _buildData[_currentBuildDataIdx]) {
                _currentBuildDataIdx = 0;
                _scrollPosition = Vector2.zero;
            }
            _buildData = buildData;
        }
    }
}
