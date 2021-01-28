using System;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Group", menuName = "Scriptable Objects/Structure Groups/Temporary")]
    public class TemporaryStructureGroup : ScriptableObject
    {
        public List<TemporaryStructurePair> value;
    }

    [Serializable]
    public struct TemporaryStructurePair
    {
        public string name;
        public GameObject prefab;
    }
}