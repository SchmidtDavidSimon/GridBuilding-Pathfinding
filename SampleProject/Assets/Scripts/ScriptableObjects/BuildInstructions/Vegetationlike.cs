using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects.BuildInstructions
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Cell Content/Building Instructions/Vegetationlike")]
    public class Vegetationlike : CellContentBuildInstructions
    {
        [SerializeField] private List<GameObject> models;
        [SerializeField] private WeightedModelList weightedModels;
        
        
        #region public methods

        protected override ModelInfo CreateModelInfo(Vector3Int pos)
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
