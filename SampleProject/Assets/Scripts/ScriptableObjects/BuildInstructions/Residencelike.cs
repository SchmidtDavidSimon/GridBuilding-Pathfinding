using System;
using System.Collections.Generic;
using System.Linq;
using Grid;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ScriptableObjects.BuildInstructions
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Cell Content/Building Instructions/Residencelike")]
    public class Residencelike : CellContentBuildInstructions
    {
        [SerializeField] private List<GameObject> models;
        [SerializeField] private List<WeightedModel> weightedModels;

        #region public methods

        protected override ModelInfo CreateModelInfo(Vector3Int pos, int width, int height)
        {
            Debug.Log(pos);
            var neighbors = CalculateStreetBoundaries(pos, width, height);
            var streetCount = neighbors.Count(x => x);
            switch (streetCount)
            {
                case 0:
                    throw new Exception("Trying to build a residence away from a street, this shouldn't be possible");
                case 1:
                case 2:
                case 3:
                    return SomeSides(neighbors);
                default:
                    return AllSides();
            }
        }

        public override void CorrectPath(ref List<Vector3Int> path)
        {
            for (var i = path.Count -1; i >= 0; i--)
            {
                if (GridExtension.GetNeighborTypes(path[i]).Count(neighbor => neighbor == CellContentType.Street) > 0) continue;
                path.RemoveAt(i);
            }
        }

        #endregion

        #region private methods

        #region utilities
        
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
        

        private bool SideNeighborsStreet(int pos1, int pos2, int houseDimension, bool xFirst, int? gridEnd = null)
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
            if (condition)
            {
                for (var i = 0; i < houseDimension; i++)
                {
                    var pos = new Vector3Int(
                        xFirst
                            ? gridEnd == null
                                ? pos1 - 1
                                : pos1 + 1
                            : pos2 + i,
                        0,
                        xFirst
                            ? pos2 + i
                            : gridEnd == null
                                ? pos1 - 1
                                : pos1 + 1);
                    if (GridExtension.CellIsOfType(pos, CellContentType.Street)) continue;
                    return false;
                }
            }
            else
            {
                return false;
            }

            return true;
        }

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

        private ModelInfo SomeSides(bool[] neighbors)
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

        private ModelInfo AllSides() => new ModelInfo
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
