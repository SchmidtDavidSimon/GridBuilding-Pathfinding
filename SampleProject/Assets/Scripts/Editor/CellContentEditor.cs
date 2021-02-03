using ScriptableObjects;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(CellContent)), CanEditMultipleObjects]
    public class CellContentEditor : UnityEditor.Editor
    {
        private Vector2Int _size = Vector2Int.zero;
        private SerializedProperty
            hasAppeal,
            appeal,
            width,
            height,
            models,
            hasSpecialBuildInstructions,
            buildInstructions,
            contentType,
            allowsMovement,
            allowedMovementTypes,
            overwritableContentTypes,
            movementCost;

        private void OnEnable()
        {
            width = serializedObject.FindProperty("width");
            height = serializedObject.FindProperty("height");
            hasSpecialBuildInstructions = serializedObject.FindProperty("hasSpecialBuildInstructions");
            buildInstructions = serializedObject.FindProperty("buildInstructions");
            models = serializedObject.FindProperty("models");
            contentType = serializedObject.FindProperty("type");
            allowsMovement = serializedObject.FindProperty("allowsMovement");
            allowedMovementTypes = serializedObject.FindProperty("allowedMovementTypes");
            movementCost = serializedObject.FindProperty("movementCost");
            hasAppeal = serializedObject.FindProperty("hasAppeal");
            appeal = serializedObject.FindProperty("appeal");
            overwritableContentTypes = serializedObject.FindProperty("overwritableContentTypes");
            
            
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.LabelField("Cell Information");
            EditorGUILayout.PropertyField(contentType, new GUIContent("Type"));
            EditorGUILayout.PropertyField(width, new GUIContent("Width"));
            EditorGUILayout.PropertyField(height, new GUIContent("Height"));
            if (width.intValue < 1)
            {
                width.intValue = 1;
            }
            if (height.intValue < 1)
            {
                height.intValue = 1;
            }
            EditorGUILayout.LabelField("Model Information");
            EditorGUILayout.PropertyField(hasSpecialBuildInstructions, new GUIContent("Has Special Building Instructions","Special building instructions include, but are not limited to: required adjacency, weighted models or certain rules to follow"));
            if (hasSpecialBuildInstructions.boolValue)
            {
                EditorGUILayout.PropertyField(buildInstructions, new GUIContent("Build Instructions"));
            }
            else
            {
                EditorGUILayout.PropertyField(models, new GUIContent("Models", "Randomly and unweighted placed models for this content"));
            }
            EditorGUILayout.LabelField("Movement Information");
            EditorGUILayout.PropertyField(allowsMovement, new GUIContent("Allows Movement"));
            if (allowsMovement.boolValue)
            {
                EditorGUILayout.PropertyField(allowedMovementTypes, new GUIContent("Allowed Movement Types"));
                EditorGUILayout.PropertyField(movementCost, new GUIContent("Movement Cost"));
            }
            EditorGUILayout.PropertyField(overwritableContentTypes, new GUIContent("Overwritable Content","When in placement mode, these content types will be overwritten on select. Own type does not need to be included"));
            EditorGUILayout.PropertyField(hasAppeal, new GUIContent("Has Appeal"));
            if (hasAppeal.boolValue)
            {
                EditorGUILayout.PropertyField(appeal, new GUIContent("Appeal"));
            }
            

            serializedObject.ApplyModifiedProperties();
        }
    }
}
