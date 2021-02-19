using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

namespace ScriptableObjects
{
    /// <summary>
    /// Special instructions for cell content models, this is only a base class and can not be used as-is. Inherit from this class to create specific instructions
    /// </summary>
    public class CellContentBuildInstructions : ScriptableObject
    {
        #region fields

        [SerializeField] protected bool requiresCellContentTypeAsNeighbor;
        [SerializeField] protected List<CellContentType> neighborRequirements;
        [SerializeField] protected bool usesWeightedModels;
        
        [SerializeField] private bool changesWithCertainNeighbor;
        [SerializeField] private List<CellContentType> neighborsToCheck;
        
        private readonly Dictionary<Vector3Int,ModelInfo> _currentModelInfos = new Dictionary<Vector3Int,ModelInfo>();

        /// <summary>
        /// Returns the previously defined neighborsToCheck, adds CellContentType.None because it applies to every content and so it doesnt have to be assigned in the inspector each time. 
        /// </summary>
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

        #endregion
        
        #region public methods

        /// <summary>
        /// 1. Create a ModelInfo for the given position with the contents width and height.
        /// 2. Save the ModelInfo for future reference
        /// </summary>
        /// <returns>Newly created ModelInfo</returns>
        public ModelInfo SelectModel(Vector3Int pos, int width, int height)
        {
            var info = CreateModelInfo(pos, width, height);
            SaveModelInfo(pos, info);
            return info;
        }
        
        /// <summary>
        /// Returns true if all of the below are given:
        /// 1. changesWithCertainNeighbor is set to true
        /// 2. The neighborType is contained in the list for neighborTypes to check
        /// 3. The newly created model is different from the model that is saved for this position  
        /// </summary>
        /// <param name="pos">Position of the content</param>
        /// <param name="neighbor">Type of the changed neighbor</param>
        /// <param name="width">Width of the content</param>
        /// <param name="height">Height of the content</param>
        /// <returns>Returns true if the content on this position needs a new Model</returns>
        public bool NeedsNewModel(Vector3Int pos,CellContentType neighbor, int width, int height)
            => changesWithCertainNeighbor 
               && NeighborsToCheck.Any(contentType => contentType == neighbor)
               && _currentModelInfos[pos] != CreateModelInfo(pos, width, height);

        /// <summary>
        /// Depending on the neighborRequirements, delete positions out of the path that don't fit the requirements 
        /// </summary>
        /// <param name="path">The path to check</param>
        /// <param name="width">The width of the content</param>
        /// <param name="height">The height of the content</param>
        public virtual void CorrectPath(ref List<Vector3Int> path, int width, int height) { }

        #endregion

        #region private methods

        /// <summary>
        /// Select a model based on its weight out of a list of applicable models
        /// 1. Create an Array out of the weights of all of the weighted models
        /// 2. Calculate the sum of all the weights
        /// 3. Pick a random value between 0 and the weight sum
        /// 4. Start the check at 0
        /// 5. For each weight check if
        ///     1. The random value is bigger or equals than the check value
        ///     2. The random value is lower that the check value plus the current weight
        /// 6. If both are true, return the prefab of the weighted models with the index of the current weight
        /// 7. If not add the current weight to the check value
        /// 8. If nothing has been returned, return prefab of the first entry of the weightedModels
        /// </summary>
        /// <returns>A prefab based on its weight</returns>
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

        /// <summary>
        /// Based on the contents position, create a ModelInfo that applies to the contents specific building instructions
        /// </summary>
        /// <returns>Returns ModelInfo that applies to its building instruction</returns>
        protected virtual ModelInfo CreateModelInfo(Vector3Int pos, int width, int height) => throw new NotImplementedException();

        #endregion
    }
}
