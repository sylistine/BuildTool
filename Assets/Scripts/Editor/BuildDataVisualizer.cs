using UnityEngine;
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
            var startupLevelName = _buildData.FindProperty("_startupLevel").FindPropertyRelative("_name");
            if (string.IsNullOrWhiteSpace(startupLevelName.stringValue)) {
                startupLevelName.stringValue = "Startup Scene";
                _buildData.ApplyModifiedProperties();
            }
        }

        public void OnGUI(Rect position) {
            var nameProp = _buildData.FindProperty("_name");
            var startupLevelProp = _buildData.FindProperty("_startupLevel");
            var levelDataListProp = _buildData.FindProperty("_levelDataList").
                FindPropertyRelative("_data");

            // General details column.
            EditorGUI.BeginChangeCheck();

            var generalDataColumnWidth = 360f;

            var namePosition = position;
            namePosition.width = generalDataColumnWidth;
            namePosition.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(namePosition, nameProp, new GUIContent("Build Name"));

            var levelListGUI = new ListGUIUtil("Levels", levelDataListProp);
            var levelsListPosition = namePosition;
            levelsListPosition.height = levelListGUI.GetTotalHeight();
            levelsListPosition.y += namePosition.height;
            levelListGUI.OnGUI(levelsListPosition);

            var levelDetailsColumn = position;
            levelDetailsColumn.width -= generalDataColumnWidth;
            levelDetailsColumn.x += generalDataColumnWidth;
            var levelWindowRect = DrawLevelDetails(levelDetailsColumn, startupLevelProp);

            if(EditorGUI.EndChangeCheck()) {
                _buildData.ApplyModifiedProperties();
            }

            // Level details column.
            for(var i = 0; i < levelDataListProp.arraySize; ++i) {
                var levelDataElement = levelDataListProp.GetArrayElementAtIndex(i).objectReferenceValue;
                if(levelDataElement == null) continue;

                var levelObject = new SerializedObject(levelDataElement);
                var levelProp = levelObject.FindProperty("_level");

                EditorGUI.BeginChangeCheck();
                if (levelProp != null) {
                    levelWindowRect.y += levelWindowRect.height + 2f;
                    levelWindowRect = DrawLevelDetails(levelWindowRect, levelProp);
                }
                if (EditorGUI.EndChangeCheck()) {
                    levelObject.ApplyModifiedProperties();
                }
            }
        }

        private Rect DrawLevelDetails(Rect rect, SerializedProperty prop) {
            rect.height = EditorGUI.GetPropertyHeight(prop) + 6f;
            var contentRect = rect;
            contentRect.width -= 6f;
            contentRect.height -= 6f;
            contentRect.x += 3f;
            contentRect.y += 3f;

            GUI.Box(rect, GUIContent.none);
            EditorGUI.PropertyField(contentRect, prop);
            return rect;
        }
    }
}
