using System;
using ScriptableObjects;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(CellContentBuildInstructions)), CanEditMultipleObjects]
    public class CellContentBuildInstructionsEditor : UnityEditor.Editor
    {
        public SerializedProperty
            requiresCellContentTypeAsNeighbor,
            neighborRequirements,
            models,
            weightedModels,
            changesWithCertainNeighbor,
            neighborsToCheck,
            usesWeightedModels;

        private void OnEnable()
        {
            requiresCellContentTypeAsNeighbor = serializedObject.FindProperty("requiresCellContentTypeAsNeighbor");
            neighborRequirements = serializedObject.FindProperty("neighborRequirements");
            usesWeightedModels = serializedObject.FindProperty("usesWeightedModels");
            changesWithCertainNeighbor = serializedObject.FindProperty("changesWithCertainNeighbor");
            neighborsToCheck = serializedObject.FindProperty("neighborsToCheck");
            AdditionOnEnable();
        }

        protected virtual void AdditionOnEnable() { }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(requiresCellContentTypeAsNeighbor, new GUIContent("Requires Neighbor"));
            if (requiresCellContentTypeAsNeighbor.boolValue)
            {
                EditorGUILayout.PropertyField(neighborRequirements, new GUIContent("Neighbor Requirements"));
            }

            EditorGUILayout.PropertyField(usesWeightedModels, new GUIContent("Uses Weighted Models"));
            if (usesWeightedModels.boolValue)
            {
                UseWeightedModels();
            }
            else
            {
                UseUnweightedModels();
            }
            EditorGUILayout.PropertyField(changesWithCertainNeighbor, new GUIContent("Changes With Neighbor", "If a neighboring cell is build with a certain content, this cell content will possibly change its model"));
            if (changesWithCertainNeighbor.boolValue)
            {
                EditorGUILayout.PropertyField(neighborsToCheck, new GUIContent("Neighbors To Check", "Array of content types after which this cell will recalculate it's model"));
            }

            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void UseUnweightedModels() { }

        protected virtual void UseWeightedModels() { }
    }
}
