using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities;

namespace ScriptableObjects.BuildInstructions
{
    /// <summary>
    /// Instructions for content that behaves like a street 
    /// </summary>
    [CreateAssetMenu(menuName = "Scriptable Objects/Cell Content/Building Instructions/StreetLike")]
    public class StreetLike : CellContentBuildInstructions
    {
        #region private structs

        /// <summary>
        /// All models needed for content that behaves like a street
        /// </summary>
        [Serializable]
        private struct Models
        {
            public GameObject deadEnd;
            public GameObject straight;
            public GameObject corner;
            public GameObject threeWay;
            public GameObject fourWay;
        }

        /// <summary>
        /// All models weighted needed for content that behaves like a street
        /// </summary>
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

        #region fields

        [SerializeField] private Models models;
        [SerializeField] private WeightedModels weightedModels;

        #endregion
        
        #region public methods

        /// <summary>
        /// Selects what model to create depending on how many neighboring streets this position has
        /// </summary>
        protected override ModelInfo CreateModelInfo(Vector3Int pos, int width, int height)
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
        
        /// <summary>
        /// Returns true if the neighboring streets are either left and right or up and down
        /// </summary>
        private bool CanCreateStraightStreet(CellContentType[] neighbors)
        {
            return neighbors[0] == CellContentType.Street && neighbors[2] == CellContentType.Street
                   || neighbors[1] == CellContentType.Street && neighbors[3] == CellContentType.Street;
        }

        #endregion

        #region selectors

        /// <summary>
        /// Selects a rotation for a dead end street based on where the neighbor is
        /// </summary>
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
        
        /// <summary>
        /// Selects a rotation for a straight street based on where the neighbors are
        /// </summary>
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
        
        /// <summary>
        /// Selects a rotation for a straight street based on where the neighbors are
        /// </summary>
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
        
        /// <summary>
        /// Selects a rotation for a three way street based on where the neighbors are
        /// </summary>
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
        
        /// <summary>
        /// Returns a four way model info
        /// </summary>
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
