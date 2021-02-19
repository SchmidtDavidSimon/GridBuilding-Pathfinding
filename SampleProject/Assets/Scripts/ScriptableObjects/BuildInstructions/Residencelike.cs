using System;
using System.Collections.Generic;
using System.Linq;
using Grid;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

namespace ScriptableObjects.BuildInstructions
{
    /// <summary>
    /// Instructions for content that behaves like a residence 
    /// </summary>
    [CreateAssetMenu(menuName = "Scriptable Objects/Cell Content/Building Instructions/ResidenceLike")]
    public class ResidenceLike : CellContentBuildInstructions
    {
        #region fields

        [SerializeField] private List<GameObject> models;
        [SerializeField] private List<WeightedModel> weightedModels;

        #endregion

        #region public methods

        /// <summary>
        /// Selects what model to create depending on how many neighboring streets this position has
        /// </summary>
        protected override ModelInfo CreateModelInfo(Vector3Int pos, int width, int height)
        {
            var neighbors = CalculateStreetBoundaries(pos, width, height);
            var streetCount = neighbors.Count(x => x);
            switch (streetCount)
            {
                case 0:
                    throw new Exception("Trying to build a residence away from a street, this shouldn't be possible");
                case 1:
                case 2:
                case 3:
                    return StreetsOnSomeSides(neighbors);
                default:
                    return StreetsOnAllSides();
            }
        }

        /// <summary>
        /// Deletes every position of the path that is either not next to a street or is inside the dimensions of the previous content
        /// </summary>
        public override void CorrectPath(ref List<Vector3Int> path, int width, int height)
        {
            var removedIndices = new List<int>();
            for (var i = path.Count -1; i >= 0; i--)
            {
                var prevPathIdx = i + 1;
                if (prevPathIdx >= path.Count)
                {
                    if (CalculateStreetBoundaries(path[i], width, height).Count(neighbor => neighbor) > 0) continue;
                    path.RemoveAt(i);
                    removedIndices.Add(i);
                }
                else
                {
                    if (removedIndices.Contains(prevPathIdx))
                    {
                        if (CalculateStreetBoundaries(path[i], width, height).Count(neighbor => neighbor) > 0) continue;
                        path.RemoveAt(i);
                        removedIndices.Add(i);
                    }
                    else if (prevPathIdx < i + width || prevPathIdx < i + height)
                    {
                        path.RemoveAt(i);
                        removedIndices.Add(i);
                    }
                    else
                    {
                        if (CalculateStreetBoundaries(path[i], width, height).Count(neighbor => neighbor) > 0) continue;
                        path.RemoveAt(i);
                        removedIndices.Add(i);
                    }
                }
            }
        }

        #endregion

        #region private methods

        #region utilities
        
        /// <summary>
        /// Returns a bool array telling if each side of a content is on a street or not 
        /// </summary>
        private bool[] CalculateStreetBoundaries(Vector3Int pos, int width, int height) => new []
            {
                //left
                SideNeighborsStreet(pos.x, pos.z, height, true),
                //top
                SideNeighborsStreet(pos.z,pos.x, width, false, GridExtension.GridHeight),
                
                //right
                SideNeighborsStreet(pos.x,pos.z, height,true, GridExtension.GridWidth),
                //down
                SideNeighborsStreet(pos.z, pos.x, width, false)
            };
        

        /// <summary>
        /// Calculates if a specific side of a content is on a street or not
        /// 1. Decide what condition is used to determine if the neighbor is out of bounds or not
        ///     1.1 If a gridEnd is not given, check against 0
        ///     1.1 If a gridEnd is given, check the primary coordinate against the gridEnd - 1
        /// 2. If the conditions aren't met return false because the neighbor is out of bounds
        /// 3. If not:
        ///     1. Calculate the position to check based on the dimension of the house, the end of the grid, the primary and secondary coordinate and if the x position is primary
        ///     2. Check if the position is not of type street
        ///         2.1 If so: return false, if not: do nothing
        /// 4. If false hasn't been returned yet, return true
        /// </summary>
        /// <param name="pos1">The primary coordinate</param>
        /// <param name="pos2">The secondary position</param>
        /// <param name="houseDimension">The current used dimensions of the house</param>
        /// <param name="xIsPrimary">If the x pos is the primary position</param>
        /// <param name="gridEnd">the end of the grid</param>
        private bool SideNeighborsStreet(int pos1, int pos2, int houseDimension, bool xIsPrimary, int? gridEnd = null)
        {
            bool condition;
            if (gridEnd == null)
            {
                condition = pos1 > 0;
            }
            else
            {
                condition = pos1 < gridEnd - 1;
            }

            if (!condition) return false;
            
            for (var i = 0; i < houseDimension; i++)
            {
                var pos = new Vector3Int(
                    xIsPrimary
                        ? gridEnd == null
                            ? pos1 - 1
                            : pos1 + 1
                        : pos2 + i,
                    0,
                    xIsPrimary 
                        ? pos2 + i 
                        : gridEnd == null 
                            ? pos1 - 1 
                            : pos1 + 1);
                
                if (!GridExtension.CellIsOfType(pos, CellContentType.Street)) return false;
            }

            return true;
        }

        /// <summary>
        /// Calculate a random Y Rotation until its part of the allowed Y rotations to get a random rotation.
        /// </summary>
        private int CalculateYRotation(List<int> allowedYRotations)
        {
            int y;
            do
            {
                y = 90 * Random.Range(0, 5);
            } while (!allowedYRotations.Contains(y));

            return y;
        }

        #endregion

        #region selectors

        /// <summary>
        /// 1. Check on which sides of the content are streets and add available rotations to a list
        /// 2. Return a ModelInfo with a random Y allowed rotation
        /// </summary>
        private ModelInfo StreetsOnSomeSides(bool[] neighbors)
        {
            var allowedYRotations = new List<int>();
            if (neighbors[0])
            {
                allowedYRotations.Add(90);
            }
            if (neighbors[1])
            {
                allowedYRotations.Add(180);
            }
            if (neighbors[2])
            {
                allowedYRotations.Add(270);
            }
            if (neighbors[3])
            {
                allowedYRotations.Add(0);
            }

            return new ModelInfo
            {
                model = usesWeightedModels
                    ? SelectWeightedModel(weightedModels)
                    : models[Random.Range(0,models.Count)],
                rotation = Quaternion.Euler(0,CalculateYRotation(allowedYRotations),0)
            };
        }

        /// <summary>
        /// 2. Return a ModelInfo with a random Y rotation
        /// </summary>
        private ModelInfo StreetsOnAllSides() => new ModelInfo
        {
            model = usesWeightedModels
                ? SelectWeightedModel(weightedModels)
                : models[Random.Range(0,models.Count)],
            rotation = Quaternion.Euler(0, 90*Random.Range(1,5),0)
        };

        #endregion

        #endregion
    }
}
