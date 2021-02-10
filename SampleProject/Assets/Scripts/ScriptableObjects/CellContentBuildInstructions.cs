using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities;
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
        
        private readonly Dictionary<Vector3Int,ModelInfo> _currentModelInfos = new Dictionary<Vector3Int,ModelInfo>();

        private List<CellContentType> NeighborsToCheck
        {
            get
            {
                var retVal = new List<CellContentType>();
                retVal.AddRange(neighborsToCheck);
                retVal.Add(CellContentType.None);
                return retVal;
            }
        }

        public bool RequiresCellContentTypeAsNeighbor => requiresCellContentTypeAsNeighbor;
        public List<CellContentType> NeighborRequirements => neighborRequirements;
        
        #region public methods

        public ModelInfo SelectModel(Vector3Int pos, int width, int height)
        {
            var info = CreateModelInfo(pos, width, height);
            SaveModelInfo(pos, info);
            return info;
        }
        
        public bool NeedsNewModel(Vector3Int pos,CellContentType neighbor, int width, int height)
            => changesWithCertainNeighbor 
               && NeighborsToCheck.Any(contentType => contentType == neighbor)
               && _currentModelInfos[pos] != CreateModelInfo(pos, width, height);

        public virtual void CorrectPath(ref List<Vector3Int> path, int width, int height) { }

        #endregion

        #region private methods

        protected GameObject SelectWeightedModel(List<WeightedModel> weightedModels)
        {
            var weights = new float[weightedModels.Count];
            for (var i = 0; i < weightedModels.Count; i++)
            {
                weights[i] =  weightedModels[i].weight;
            }

            var weightSum = weights.Sum();
            var randomValue = Random.Range(0, weightSum);
            var temp = 0f;
            for (var i = 0; i < weights.Length; i++)
            {
                if (randomValue >= temp && randomValue < temp + weights[i])
                {
                    return weightedModels[i].prefab;
                }
                temp += weights[i];
            }
            return weightedModels[0].prefab;
        }
        

        private void SaveModelInfo(Vector3Int pos, ModelInfo info)
        {
            if (_currentModelInfos.ContainsKey(pos))
            {
                _currentModelInfos.Remove(pos);
            }
            _currentModelInfos.Add(pos, info);
        }

        protected virtual ModelInfo CreateModelInfo(Vector3Int pos, int width, int height) => throw new NotImplementedException();

        #endregion
        

        
    }
}
