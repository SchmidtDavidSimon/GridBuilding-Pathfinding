using System;
using System.Collections.Generic;
using System.Linq;
using ScriptableObjects.BuildInstructions;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ScriptableObjects
{
    public class CellContentBuildInstructions : ScriptableObject
    {
        [SerializeField] protected bool requiresCellContentTypeAsNeighbor;
        [SerializeField] protected List<CellContentType> neighborRequirements;
        [SerializeField] protected bool usesWeightedModels;
        [SerializeField] private bool changesWithCertainNeighbor;
        [SerializeField] private List<CellContentType> neighborsToCheck;
        protected Dictionary<Vector3Int,ModelInfo> currentModelInfos = new Dictionary<Vector3Int,ModelInfo>();

        public bool RequiresCellContentTypeAsNeighbor => requiresCellContentTypeAsNeighbor;
        public List<CellContentType> NeighborRequirements => neighborRequirements;
        public bool ChangesWithCertainNeighbor => changesWithCertainNeighbor;

        protected GameObject SelectWeightedModel(WeightedModelList weightedModels)
        {
            var weights = new float[weightedModels.value.Count];
            for (var i = 0; i < weightedModels.value.Count; i++)
            {
                weights[i] =  weightedModels.value[i].weight;
            }

            var weightSum = weights.Sum();
            var randomValue = Random.Range(0, weightSum);
            var temp = 0f;
            for (var i = 0; i < weights.Length; i++)
            {
                if (randomValue >= temp && randomValue < temp + weights[i])
                {
                    return weightedModels.value[i].prefab;
                }
                temp += weights[i];
            }
            return weightedModels.value[0].prefab;
        }
        

        public ModelInfo SelectModel(Vector3Int pos)
        {
            var info = CreateModelInfo(pos);
            SaveModelInfo(pos, info);
            return info;
        }

        private void SaveModelInfo(Vector3Int pos, ModelInfo info)
        {
            if (currentModelInfos.ContainsKey(pos))
            {
                currentModelInfos.Remove(pos);
            }
            currentModelInfos.Add(pos, info);
        }

        protected virtual ModelInfo CreateModelInfo(Vector3Int pos) => throw new NotImplementedException();

        public bool NeedsNewModel(Vector3Int pos,CellContentType neighbor)
            => neighborsToCheck.Any(contentType => contentType == neighbor)
               && currentModelInfos[pos] != CreateModelInfo(pos);
    }
}
