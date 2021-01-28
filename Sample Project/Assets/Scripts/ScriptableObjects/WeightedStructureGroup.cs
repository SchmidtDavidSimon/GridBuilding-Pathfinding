using System;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Group", menuName = "Scriptable Objects/Structure Groups/Weighted")]
    public class WeightedStructureGroup : ScriptableObject
    {
        public List<StructurePrefabWeighted> value;
    }

    [Serializable]
    public struct StructurePrefabWeighted    
    {
        public GameObject prefab;
        [Range(0,1)] public float weight;
    }
}