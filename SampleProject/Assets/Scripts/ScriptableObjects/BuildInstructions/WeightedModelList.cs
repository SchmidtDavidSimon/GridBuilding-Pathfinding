using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace ScriptableObjects.BuildInstructions
{
    [CreateAssetMenu(fileName = "Group", menuName = "Scriptable Objects/Cell Content/Building Instructions/Weighted Models")]
    public class WeightedModelList : ScriptableObject
    {
        public List<WeightedModel> value;
    }
}