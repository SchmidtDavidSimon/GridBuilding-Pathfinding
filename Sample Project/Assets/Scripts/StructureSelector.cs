using System;
using System.Collections.Generic;
using System.Linq;
using Grid;
using ScriptableObjects;
using UnityEngine;
using Random = UnityEngine.Random;

public static class StructureSelector
{
    public static StructureModifyInfo SelectStructureForPos(Vector3Int pos, CellType cellType)
    {
        switch (cellType)
        {
            case CellType.Street:
                return SelectStreet(pos);
            case CellType.House:
                return SelectHouse(pos);
            default:
                throw new Exception("Unknown cell type");
        }
    }
    
    private static int GetRandomWeightedIndex(float[] weights)
    {
        var weightSum = weights.Sum();
        var randomValue = UnityEngine.Random.Range(0, weightSum);
        var temp = 0f;
        for (var i = 0; i < weights.Length; i++)
        {
            if (randomValue >= temp && randomValue < temp + weights[i])
            {
                return i;
            }
            temp += weights[i];
        }
        return 0;
    }

    #region Selecting Streets
    
    private static Dictionary<string, GameObject> _streetDict;
    
    private static StructureModifyInfo SelectStreet(Vector3Int pos)
    {
        var streets = Resources.Load<TemporaryStructureGroup>("Streets").value;
        if (_streetDict == null || _streetDict.Count != streets.Count)
        {
            _streetDict = new Dictionary<string, GameObject>();
            
            foreach (var tsp in streets)
            {
                _streetDict.Add(tsp.name,tsp.prefab);
            }
        }

        var neighbors = GridExtension.GetNeighborTypes(pos);
        var streetCount = neighbors.Count(x => x == CellType.Street);
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

    private static bool CanCreateStraightStreet(CellType[] neighbors)
    {
        return neighbors[0] == CellType.Street && neighbors[2] == CellType.Street
               || neighbors[1] == CellType.Street && neighbors[3] == CellType.Street;
    }
    
    private static StructureModifyInfo DeadEnd(CellType[] neighbors)
    {
        if (neighbors[2] == CellType.Street)
        {
            return new StructureModifyInfo
            {
                model = _streetDict["DeadEnd"],
                rotation = Quaternion.identity
            };
        }
        if (neighbors[3] == CellType.Street)
        {
            return new StructureModifyInfo
            {
                model = _streetDict["DeadEnd"],
                rotation = Quaternion.Euler(0,90,0)
            };
        }
        if (neighbors[0] == CellType.Street)
        {
            return new StructureModifyInfo
            {
                model = _streetDict["DeadEnd"],
                rotation = Quaternion.Euler(0,180,0)
            };
        }
        return new StructureModifyInfo
        {
            model = _streetDict["DeadEnd"],
            rotation = Quaternion.Euler(0,270,0)
        };
    }

    private static StructureModifyInfo StraightRoad(CellType[] neighbors)
    {
        if (neighbors[0] == CellType.Street && neighbors[2] == CellType.Street)
        {
            return new StructureModifyInfo
            {
                model = _streetDict["Straight"],
                rotation = Quaternion.identity
            };
        }
        return new StructureModifyInfo
        {
            model = _streetDict["Straight"],
            rotation = Quaternion.Euler(0,90,0)
        };
    }
    
    private static StructureModifyInfo Corner(CellType[] neighbors)
    {
        if (neighbors[0] == CellType.Street && neighbors[1] == CellType.Street)
        {
            return new StructureModifyInfo
            {
                model = _streetDict["Corner"],
                rotation = Quaternion.identity
            };
        }
        if (neighbors[1] == CellType.Street && neighbors[2] == CellType.Street)
        {
            return new StructureModifyInfo
            {
                model = _streetDict["Corner"],
                rotation = Quaternion.Euler(0,90,0)
            };}
        if (neighbors[2] == CellType.Street && neighbors[3] == CellType.Street)
        {
            return new StructureModifyInfo
            {
                model = _streetDict["Corner"],
                rotation = Quaternion.Euler(0,180,0)
            };
        }
        return new StructureModifyInfo
        {
            model = _streetDict["Corner"],
            rotation = Quaternion.Euler(0,270,0)
        };
    }

    private static StructureModifyInfo ThreeWay(CellType[] neighbors)
    {
        if (neighbors[0] != CellType.Street)
        {
            return new StructureModifyInfo
            {
                model = _streetDict["ThreeWay"],
                rotation = Quaternion.identity
            };
        }
        if (neighbors[1] != CellType.Street)
        {
            return new StructureModifyInfo
            {
                model = _streetDict["ThreeWay"],
                rotation = Quaternion.Euler(0,90,0)
            };
        }
        if (neighbors[2] != CellType.Street)
        {
            return new StructureModifyInfo
            {
                model = _streetDict["ThreeWay"],
                rotation = Quaternion.Euler(0,180,0)
            };
            
        }
        return new StructureModifyInfo
        {
            model = _streetDict["ThreeWay"],
            rotation = Quaternion.Euler(0,270,0)
        };
    }
    
    private static StructureModifyInfo FourWay() => new StructureModifyInfo
    {
        model = _streetDict["FourWay"],
        rotation = Quaternion.identity
    };
    
    #endregion
    
    #region Selecting Houses

    private static List<GameObject> _houseObjects;
    private static List<float> _houseWeights;
    
    private static StructureModifyInfo SelectHouse(Vector3Int position)
    {
        var houses = Resources.Load<WeightedStructureGroup>("Houses").value;
        if (_houseObjects == null || _houseObjects.Count != houses.Count)
        {
            _houseObjects = new List<GameObject>();
            _houseWeights = new List<float>();
            foreach (var house in houses)
            {
                _houseObjects.Add(house.prefab);
                _houseWeights.Add(house.weight);
            }
        }

        var neighbors = GridExtension.GetNeighborTypes(position);
        var streetCount = neighbors.Count(type => type == CellType.Street);  
        var weightedIdx = GetRandomWeightedIndex(_houseWeights.ToArray());
        switch (streetCount)
        {
            case 0:
                throw new Exception("Zero Street next to a building, shouldn't be possible");
            case 1:
                return SelectHouseOnOneStreet(neighbors, weightedIdx);
            case 2:
                return SelectHouseOnTwoStreets(neighbors, weightedIdx);
            case 3:
                return SelectHouseOnThreeStreets(neighbors, weightedIdx);
            default:
                return SelectHouseOnFourStreets(weightedIdx);
        }
    }
    
    private static StructureModifyInfo SelectHouseOnOneStreet(CellType[] neighbors, int index)
    {
        var yes = -1;
        if (neighbors[0] == CellType.Street)
        {
            yes = 90;
        }

        if (neighbors[1] == CellType.Street)
        {
            yes = 180;
        }
        if (neighbors[2] == CellType.Street)
        {
            yes = 270;
        }
        if (neighbors[3] == CellType.Street)
        {
            yes = 0;
        };
        var y = 0;
        while (y != yes)
        {
            y += 90;
        }
        return new StructureModifyInfo
        {
            model = _houseObjects[index],
            rotation = Quaternion.Euler(0,y,0)
        };
    }

    private static StructureModifyInfo SelectHouseOnTwoStreets(CellType[] neighbors, int index)
    {
        var not = new List<int>();
        if (neighbors[0] != CellType.Street)
        {
            not.Add(90);
        }
        if (neighbors[1] != CellType.Street)
        {
            not.Add(180);
        }
        if (neighbors[2] != CellType.Street)
        {
            not.Add(2700);
        }
        if (neighbors[3] != CellType.Street)
        {
            not.Add(0);
        }

        int y;
        do
        {
            var randomNumber = Random.Range(0, 4);
            y = 90 * randomNumber;
        } while (not.Any(x => x == y));
        
        return new StructureModifyInfo
        {
            model = _houseObjects[index],
            rotation = Quaternion.Euler(0,y,0)
        };
    }
    
    private static StructureModifyInfo SelectHouseOnThreeStreets(CellType[] neighbors, int index)
    {
        var not =-1;
        if (neighbors[0] != CellType.Street)
        {
            not = 90;
        }
        if (neighbors[1] != CellType.Street)
        {
            not = 180;
        }
        if (neighbors[2] != CellType.Street)
        {
            not = 270;
        }
        if (neighbors[3] != CellType.Street)
        {
            not = 0;
        }
        
        int y;
        do
        {
            var randomNumber = Random.Range(0, 4);
            y = 90 * randomNumber;
        } while (y == not);
        
        return new StructureModifyInfo
        {
            model = _houseObjects[index],
            rotation = Quaternion.Euler(0,y,0)
        };
    }

    private static StructureModifyInfo SelectHouseOnFourStreets(int index)
    {
        var randomValue = Random.Range(0, 4);
        return new StructureModifyInfo
        {
            model = _houseObjects[index],
            rotation = Quaternion.Euler(0,90*randomValue,0)
        };
    }

    #endregion
}

public struct StructureModifyInfo
{
    public GameObject model;
    public Quaternion rotation;
}
