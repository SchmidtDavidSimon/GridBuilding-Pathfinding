using System;
using System.Collections.Generic;
using System.Linq;
using Grid;
using UnityEngine;

namespace ScriptableObjects.BuildInstructions
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Cell Content/Building Instructions/Streetlike")]
    public class Streetlike : CellContentBuildInstructions
    {
        #region private structs

        [Serializable]
        private struct Models
        {
            public GameObject deadEnd;
            public GameObject straight;
            public GameObject corner;
            public GameObject threeWay;
            public GameObject fourWay;
        }

        [Serializable]
        private struct WeightedModels
        {
            public List<WeightedModel> deadEnds;
            public List<WeightedModel> straights;
            public List<WeightedModel> corners;
            public List<WeightedModel> threeWays;
            public List<WeightedModel> fourWays;
        }

        #endregion

        [SerializeField] private Models models;
        [SerializeField] private WeightedModels weightedModels;


        #region public methods

        protected override ModelInfo CreateModelInfo(Vector3Int pos)
        {
            var neighbors = GridExtension.GetNeighborTypes(pos);
            var streetCount = neighbors.Count(x => x == CellContentType.Street);
            switch (streetCount)
            {
                case 0:
                case 1:
                    return DeadEnd(neighbors);
                case 2 when CanCreateStraightStreet(neighbors):
                    return StraightRoad(neighbors);
                case 2:
                    return Corner(neighbors);
                case 3:
                    return ThreeWay(neighbors);
                default:
                    return FourWay();
            }
        }

        #endregion

        #region private methods

        #region utilities
        
        private bool CanCreateStraightStreet(CellContentType[] neighbors)
        {
            return neighbors[0] == CellContentType.Street && neighbors[2] == CellContentType.Street
                   || neighbors[1] == CellContentType.Street && neighbors[3] == CellContentType.Street;
        }

        #endregion

        #region selectors

        private ModelInfo DeadEnd(CellContentType[] neighbors)
        {
            if (neighbors[2] == CellContentType.Street)
            {
                return new ModelInfo
                {
                    model = usesWeightedModels
                        ? SelectWeightedModel(weightedModels.deadEnds)
                        : models.deadEnd,
                    rotation = Quaternion.identity
                };
            }

            if (neighbors[3] == CellContentType.Street)
            {
                return new ModelInfo
                {
                    model = usesWeightedModels
                        ? SelectWeightedModel(weightedModels.deadEnds)
                        : models.deadEnd,
                    rotation = Quaternion.Euler(0, 90, 0)
                };
            }

            if (neighbors[0] == CellContentType.Street)
            {
                return new ModelInfo
                {
                    model = usesWeightedModels
                        ? SelectWeightedModel(weightedModels.deadEnds)
                        : models.deadEnd,
                    rotation = Quaternion.Euler(0, 180, 0)
                };
            }

            return new ModelInfo
            {
                model = usesWeightedModels
                    ? SelectWeightedModel(weightedModels.deadEnds)
                    : models.deadEnd,
                rotation = Quaternion.Euler(0, 270, 0)
            };
        }
        
        private ModelInfo StraightRoad(CellContentType[] neighbors)
        {
            if (neighbors[0] == CellContentType.Street && neighbors[2] == CellContentType.Street)
            {
                return new ModelInfo
                {
                    model = usesWeightedModels
                        ? SelectWeightedModel(weightedModels.straights)
                        : models.straight,
                    rotation = Quaternion.identity
                };
            }
            return new ModelInfo
            {
                model = usesWeightedModels
                    ? SelectWeightedModel(weightedModels.straights)
                    : models.straight,
                rotation = Quaternion.Euler(0,90,0)
            };
        }
        
        private ModelInfo Corner(CellContentType[] neighbors)
        {
            if (neighbors[0] == CellContentType.Street && neighbors[1] == CellContentType.Street)
            {
                return new ModelInfo
                {
                    model = usesWeightedModels
                        ? SelectWeightedModel(weightedModels.corners)
                        : models.corner,
                    rotation = Quaternion.identity
                };
            }
            if (neighbors[1] == CellContentType.Street && neighbors[2] == CellContentType.Street)
            {
                return new ModelInfo
                {
                    model = usesWeightedModels
                        ? SelectWeightedModel(weightedModels.corners)
                        : models.corner,
                    rotation = Quaternion.Euler(0,90,0)
                };}
            if (neighbors[2] == CellContentType.Street && neighbors[3] == CellContentType.Street)
            {
                return new ModelInfo
                {
                    model = usesWeightedModels
                        ? SelectWeightedModel(weightedModels.corners)
                        : models.corner,
                    rotation = Quaternion.Euler(0,180,0)
                };
            }
            return new ModelInfo
            {
                model = usesWeightedModels
                    ? SelectWeightedModel(weightedModels.corners)
                    : models.corner,
                rotation = Quaternion.Euler(0,270,0)
            };
        }
        
        private ModelInfo ThreeWay(CellContentType[] neighbors)
        {
            if (neighbors[0] != CellContentType.Street)
            {
                return new ModelInfo
                {
                    model = usesWeightedModels
                        ? SelectWeightedModel(weightedModels.threeWays)
                        : models.threeWay,
                    rotation = Quaternion.identity
                };
            }
            if (neighbors[1] != CellContentType.Street)
            {
                return new ModelInfo
                {
                    model = usesWeightedModels
                        ? SelectWeightedModel(weightedModels.threeWays)
                        : models.threeWay,
                    rotation = Quaternion.Euler(0,90,0)
                };
            }
            if (neighbors[2] != CellContentType.Street)
            {
                return new ModelInfo
                {
                    model = usesWeightedModels
                        ? SelectWeightedModel(weightedModels.threeWays)
                        : models.threeWay,
                    rotation = Quaternion.Euler(0,180,0)
                };
            
            }
            return new ModelInfo
            {
                model = usesWeightedModels
                    ? SelectWeightedModel(weightedModels.threeWays)
                    : models.threeWay,
                rotation = Quaternion.Euler(0,270,0)
            };
        }
        
        private ModelInfo FourWay() => new ModelInfo
        {
            model = usesWeightedModels
                ? SelectWeightedModel(weightedModels.fourWays)
                : models.fourWay,
            rotation = Quaternion.identity
        };
        
        #endregion

        #endregion
    }

}
