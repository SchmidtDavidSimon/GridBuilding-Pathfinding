using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace ScriptableObjects.BuildInstructions
{
    /// <summary>
    /// Instructions for content that behaves like vegetation 
    /// </summary>
    [CreateAssetMenu(menuName = "Scriptable Objects/Cell Content/Building Instructions/VegetationLike")]
    public class VegetationLike : CellContentBuildInstructions
    {
        [SerializeField] private List<GameObject> models;
        [SerializeField] private List<WeightedModel> weightedModels;
        
        
        #region public methods

        /// <summary>
        /// Selects either a random or a weighted model and creates a random rotation around the y axe
        /// </summary>
        protected override ModelInfo CreateModelInfo(Vector3Int pos, int width, int height)
        {
            return new ModelInfo
            {
                model = usesWeightedModels
                    ? SelectWeightedModel(weightedModels)
                    : models[Random.Range(0, models.Count - 1)],
                rotation = Quaternion.Euler(0, Random.Range(0, 360), 0)
            };
        }

        #endregion
    }
}
