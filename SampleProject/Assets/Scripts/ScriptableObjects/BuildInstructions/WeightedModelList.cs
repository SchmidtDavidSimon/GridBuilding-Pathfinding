using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects.BuildInstructions
{
    [CreateAssetMenu(fileName = "Group", menuName = "Scriptable Objects/Cell Content/Building Instructions/Weighted Models")]
    public class WeightedModelList : ScriptableObject
    {
        public List<WeightedModel> value;
    }
}