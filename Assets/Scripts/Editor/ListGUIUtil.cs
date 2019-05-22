using System;
using UnityEngine;
using UnityEditor;

public class ListGUIUtil {
    private readonly string _label;
    private readonly SerializedProperty _listProperty;
    private readonly float _lineHeight;

    public ListGUIUtil(string label, SerializedProperty arrayProperty) {
        if(!arrayProperty.isArray) throw new ArgumentException();
        _label = label;
        _listProperty = arrayProperty;
        _lineHeight = EditorGUIUtility.singleLineHeight;
    }

    public float GetTotalHeight() {
        var height = _lineHeight * (_listProperty.arraySize + 1);
        height += 4f;
        return height;
    }

    public void OnGUI(Rect position) {
        // Do box.
        GUI.Box(position, new GUIContent("box"));
        position.width -= 4f;
        position.height -= 4f;
        position.x += 2f;
        position.y += 2f;

        var elementPosition = position;
        elementPosition.height = _lineHeight;

        // Do top.
        var labelPosition = elementPosition;
        labelPosition.width = EditorGUIUtility.labelWidth;
        var addButtonPosition = elementPosition;
        addButtonPosition.width -= EditorGUIUtility.labelWidth;
        addButtonPosition.x += EditorGUIUtility.labelWidth;
        EditorGUI.LabelField(labelPosition, new GUIContent(_label));
        if(GUI.Button(addButtonPosition, new GUIContent("+"))) {
            _listProperty.InsertArrayElementAtIndex(_listProperty.arraySize);
        }

        // Do content.
        elementPosition.width -= _lineHeight;
        var xButtonPosition = elementPosition;
        xButtonPosition.width = _lineHeight;
        xButtonPosition.x += elementPosition.width;
        for(int i = 0; i < _listProperty.arraySize; ++i) {
            elementPosition.y += _lineHeight;
            xButtonPosition.y += _lineHeight;
            EditorGUI.PropertyField(elementPosition, _listProperty.GetArrayElementAtIndex(i));
            if (GUI.Button(xButtonPosition, new GUIContent("×"))) {
                _listProperty.DeleteArrayElementAtIndex(i);
            }
        }
    }
}
