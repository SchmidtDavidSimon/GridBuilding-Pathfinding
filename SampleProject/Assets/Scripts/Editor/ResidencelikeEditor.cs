using ScriptableObjects.BuildInstructions;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(ResidenceLike)), CanEditMultipleObjects]
    public class ResidencelikeEditor : CellContentBuildInstructionsEditor
    {
        protected override void AdditionOnEnable()
        {
            models = serializedObject.FindProperty("models");
            weightedModels = serializedObject.FindProperty("weightedModels");
        }

        protected override void UseUnweightedModels()=> EditorGUILayout.PropertyField(models, new GUIContent("Models"));

        protected override void UseWeightedModels() => EditorGUILayout.PropertyField(weightedModels, new GUIContent("Weighted Models"));

    }
}
