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
        
        private bool[] CalculateStreetBoundaries(Vector3Int pos, int width, int height)
        {
            var retVal = new [] {true, true,true,true};
            //left
            //SideNeighborsStreet(pos.x, pos.z, height);
            if (pos.x > 0)
            {
                for (var i = pos.z; i < height; i++)
                {
                    if (GridExtension.GetCellType(new Vector3Int(pos.x-1,0,i)) == CellContentType.Street) continue;
                    retVal[0] = false;
                    break;
                }
            }
            else
            {
                retVal[0] = false;
            }
            //top
            if (pos.z < GridExtension.GridHeight - 1)
            {
                for (var i = pos.x; i < width; i++)
                {
                    if (GridExtension.GetCellType(new Vector3Int(pos.z+1,0,i)) == CellContentType.Street) continue;
                    retVal[1] = false;
                    break;
                }
            }
            else
            {
                retVal[1] = false;
            }
            //right
            if (pos.x < GridExtension.GridWidth - 1)
            {
                for (var i = pos.z; i < height; i++)
                {
                    if (GridExtension.GetCellType(new Vector3Int(pos.x+1,0,i)) == CellContentType.Street) continue;
                    retVal[2] = false;
                    break;
                }
            }
            else
            {
                retVal[2] = false;
            }
            //down
            if (pos.z > 0)
            {
                for (var i = pos.x; i < width; i++)
                {
                    if (GridExtension.GetCellType(new Vector3Int(pos.z-1,0,i)) == CellContentType.Street) continue;
                    retVal[3] = false;
                    break;
                }
            }
            else
            {
                retVal[3] = false;
            }

            return retVal;
        }

        private bool SideNeighborsStreet(int pos1, int pos2, int houseDimension, int? gridEnd = null)
        {
            throw new NotImplementedException();
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
