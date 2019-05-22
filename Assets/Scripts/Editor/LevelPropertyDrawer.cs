using UnityEngine;
using UnityEditor;

namespace Djn.Builds {
    [CustomPropertyDrawer(typeof(Level))]
    public class LevelPropertyDrawer : PropertyDrawer {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            // Property label, level name, main scene, and subscene list add button rows.
            var height = 3f * EditorGUIUtility.singleLineHeight;

            var subSceneProp =
                property.FindPropertyRelative("_subScenes").FindPropertyRelative("_scenes");
            var subScenePropHeight = (subSceneProp.arraySize + 1f) * EditorGUIUtility.singleLineHeight;
            height += subScenePropHeight;

            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);

            var levelNameProperty = property.FindPropertyRelative("_name");
            var mainSceneProperty = property.FindPropertyRelative("_mainScene");

            // Property field header.
            var headerPosition = position;
            headerPosition.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.LabelField(headerPosition, label);

            // Level name.
            var namePosition = headerPosition;
            namePosition.y += headerPosition.height;
            EditorGUI.PropertyField(namePosition, levelNameProperty, new GUIContent("Level Name"));

            // Main scene.
            var mainScenePosition = namePosition;
            mainScenePosition.y += namePosition.height;
            EditorGUI.PropertyField(mainScenePosition, mainSceneProperty, new GUIContent("Main Scene"));

            // SubScene List area.
            var subSceneProp =
                property.FindPropertyRelative("_subScenes").FindPropertyRelative("_scenes");

            var subSceneArrayPosition = mainScenePosition;
            subSceneArrayPosition.y += mainScenePosition.height;

            var subSceneArrayLabelPosition = subSceneArrayPosition;
            subSceneArrayLabelPosition.width = EditorGUIUtility.labelWidth;
            EditorGUI.LabelField(subSceneArrayLabelPosition, new GUIContent("Sub Scenes"));

            var subSceneArrayAddButtonPosition = subSceneArrayPosition;
            subSceneArrayAddButtonPosition.width -= EditorGUIUtility.labelWidth;
            subSceneArrayAddButtonPosition.x += EditorGUIUtility.labelWidth;
            if(GUI.Button(subSceneArrayAddButtonPosition, new GUIContent("+"))) {
                subSceneProp.InsertArrayElementAtIndex(subSceneProp.arraySize);
            }

            var subSceneArrayElementPosition = subSceneArrayPosition;
            subSceneArrayElementPosition.width -= EditorGUIUtility.singleLineHeight;
            var xButtonPosition = subSceneArrayElementPosition;
            xButtonPosition.x += subSceneArrayElementPosition.width;
            xButtonPosition.width = EditorGUIUtility.singleLineHeight;
            for(var i = 0; i < subSceneProp.arraySize; ++i) {
                subSceneArrayElementPosition.y += EditorGUIUtility.singleLineHeight;
                xButtonPosition.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(subSceneArrayElementPosition, subSceneProp.GetArrayElementAtIndex(i), new GUIContent(i.ToString()));
                if(GUI.Button(xButtonPosition, new GUIContent("×"))) {
                    subSceneProp.DeleteArrayElementAtIndex(i);
                }
            }

            EditorGUI.EndProperty();
        }
    }
}