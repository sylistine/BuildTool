﻿using UnityEngine;
using UnityEditor;

namespace Djn.Builds {
    public class BuildDataVisualizer {
        private readonly SerializedObject _buildData;

        public BuildDataVisualizer(BuildData data) {
            _buildData = new SerializedObject(data);
            var buildTargetName = _buildData.FindProperty("_name");
            if (string.IsNullOrWhiteSpace(buildTargetName.stringValue)) {
                buildTargetName.stringValue = "Build Target";
            }
            var startupLevel = new SerializedObject(_buildData.FindProperty("_startupLevel").objectReferenceValue);
            var startupLevelName = startupLevel.FindProperty("_name");
            if (string.IsNullOrWhiteSpace(startupLevelName.stringValue)) {
                startupLevelName.stringValue = "Startup Scene";
                _buildData.ApplyModifiedProperties();
            }
        }

        public void OnGUI(Rect position) {
            var nameProp = _buildData.FindProperty("_name");
            var startupLevelProp = _buildData.FindProperty("_startupLevel");
            var startupLevelSerialized = new SerializedObject(startupLevelProp.objectReferenceValue);
            var levelDataListProp =
                _buildData.FindProperty("_levelDataList").FindPropertyRelative("_data");

            // General details column.
            EditorGUI.BeginChangeCheck();

            var generalDataColumnWidth = 360f;

            // Build name.
            var namePosition = position;
            namePosition.width = generalDataColumnWidth;
            namePosition.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(namePosition, nameProp, new GUIContent("Build Name"));

            // Startup level.
            var startupLevelPosition = namePosition;
            startupLevelPosition.y += namePosition.height;
            EditorGUI.PropertyField(startupLevelPosition, startupLevelProp, false);

            // Sub levels.
            var levelListGUI = new ListGUIUtil("Levels", levelDataListProp);
            var levelsListPosition = startupLevelPosition;
            levelsListPosition.height = levelListGUI.GetTotalHeight();
            levelsListPosition.y += startupLevelPosition.height;
            levelListGUI.OnGUI(levelsListPosition);

            if (EditorGUI.EndChangeCheck()) {
              _buildData.ApplyModifiedProperties();
            }

            // Level details column.
            var levelDetailsColumn = position;
            levelDetailsColumn.width -= generalDataColumnWidth;
            levelDetailsColumn.x += generalDataColumnWidth;
            var levelWindowRect = DrawLevelDetails(
                levelDetailsColumn,
                startupLevelSerialized,
                new GUIContent("Startup Level"));
            
            for(var i = 0; i < levelDataListProp.arraySize; ++i) {
                var levelDataElement = levelDataListProp.GetArrayElementAtIndex(i).objectReferenceValue;
                if(levelDataElement == null) continue;

                var levelObject = new SerializedObject(levelDataElement);

                if (levelObject != null) {
                    levelWindowRect.y += levelWindowRect.height + 2f;
                    levelWindowRect = DrawLevelDetails(levelWindowRect, levelObject);
                }
            }
        }

        // Just boxes the default GUI for LevelData.
        private Rect DrawLevelDetails(Rect rect, SerializedObject so, GUIContent label = null) {
            rect.height = LevelDataEditor.GetPropertyHeight(so, label) + 6f;
            var contentRect = rect;
            contentRect.width -= 6f;
            contentRect.height -= 6f;
            contentRect.x += 3f;
            contentRect.y += 3f;

            GUI.Box(rect, GUIContent.none);
            LevelDataEditor.OnGUI(contentRect, so, label);
            return rect;
        }
    }
}
