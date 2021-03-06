﻿using UnityEngine;
using UnityEditor;

namespace Djn.Builds {
    [CustomPropertyDrawer(typeof(Scene))]
    public class ScenePropertyDrawer : PropertyDrawer {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);

            var guidPropertyName = "_guid";
            var pathPropertyName = "_path";
            var guid = property.FindPropertyRelative(guidPropertyName).stringValue;
            var path = property.FindPropertyRelative(pathPropertyName).stringValue;

            var serializedDataChanged = false;

            var pathFromGUID = AssetDatabase.GUIDToAssetPath(guid);
            if(pathFromGUID != path) {
                serializedDataChanged = true;
                path = pathFromGUID;
            }

            var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
            var newSceneAsset = EditorGUI.ObjectField(position, label, sceneAsset, typeof(SceneAsset), false);
            if(newSceneAsset != sceneAsset) {
                serializedDataChanged = true;
                path = AssetDatabase.GetAssetPath(newSceneAsset);
                guid = AssetDatabase.AssetPathToGUID(path);
            }

            if(serializedDataChanged) {
                property.FindPropertyRelative(guidPropertyName).stringValue = guid;
                property.FindPropertyRelative(pathPropertyName).stringValue = path;
                property.serializedObject.ApplyModifiedProperties();
            }

            EditorGUI.EndProperty();
        }
    }
}
