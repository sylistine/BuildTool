using UnityEngine;
using UnityEditor;

namespace Djn.Builds {
    public class LevelDataEditor {
        public static float GetPropertyHeight(SerializedObject serializedObject, GUIContent label = null) {
            // level name, main scene, and subscene list add button rows.
            var height = (label != null ? 3f : 2f) * EditorGUIUtility.singleLineHeight;

            var subSceneProp =
                serializedObject.FindProperty("_subScenes").FindPropertyRelative("_scenes");
            var subScenePropHeight = (subSceneProp.arraySize + 1f) * EditorGUIUtility.singleLineHeight;
            height += subScenePropHeight;

            return height;
        }

        public static void OnGUI(Rect position, SerializedObject serializedObject, GUIContent label = null) {
            EditorGUI.BeginChangeCheck();
            var levelNameProperty = serializedObject.FindProperty("_name");
            var mainSceneProperty = serializedObject.FindProperty("_mainScene");

            var currentRect = position;
            currentRect.width = 0f;
            currentRect.height = 0f;

            if (label != null) {
                currentRect.height = EditorGUIUtility.singleLineHeight;
                currentRect.width = position.width;
                EditorGUI.LabelField(currentRect, label);
            }

            // Level name.
            currentRect.x = position.x;
            currentRect.y += currentRect.height;
            currentRect.width = position.width;
            currentRect.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(currentRect, levelNameProperty, new GUIContent("Level Name"));

            // Main scene.
            currentRect.y += currentRect.height;
            EditorGUI.PropertyField(currentRect, mainSceneProperty, new GUIContent("Main Scene"));

            // SubScene List area.
            var subSceneProp =
                serializedObject.FindProperty("_subScenes").FindPropertyRelative("_scenes");

            currentRect.y += currentRect.height;

            var subSceneArrayLabelPosition = currentRect;
            subSceneArrayLabelPosition.width = EditorGUIUtility.labelWidth;
            EditorGUI.LabelField(subSceneArrayLabelPosition, new GUIContent("Sub Scenes"));

            var subSceneArrayAddButtonPosition = currentRect;
            subSceneArrayAddButtonPosition.width -= EditorGUIUtility.labelWidth;
            subSceneArrayAddButtonPosition.x += EditorGUIUtility.labelWidth;
            if(GUI.Button(subSceneArrayAddButtonPosition, new GUIContent("+"))) {
                subSceneProp.InsertArrayElementAtIndex(subSceneProp.arraySize);
            }

            var subSceneArrayElementPosition = currentRect;
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
            if(EditorGUI.EndChangeCheck()) {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}