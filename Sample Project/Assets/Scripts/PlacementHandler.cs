using System.Collections.Generic;
using System.Linq;
using Grid;
using Placements;
using UnityEngine;
using Input = Game.Input;

public class PlacementHandler
{
    private readonly Transform _structureParent;

    private Dictionary<Vector3Int, Structure> _temporaryStructures;
    private readonly Dictionary<Vector3Int, Structure> _altTemporaryStructures;
    private readonly Dictionary<Vector3Int, Structure> _permanentStructures;

    private readonly StreetPlacement _streetPlacement;
    private readonly HousePlacement _housePlacement;
    public PlacementHandler(Transform structureParent)
    {
        _structureParent = structureParent;
        
        _streetPlacement = new StreetPlacement(this);
        _housePlacement = new HousePlacement(this);
        
        _temporaryStructures = new Dictionary<Vector3Int, Structure>();
        _altTemporaryStructures = new Dictionary<Vector3Int, Structure>();
        _permanentStructures = new Dictionary<Vector3Int, Structure>();
    }
    
    public void CreateTemporaryStructure(Vector3Int position, CellType cellType)
    {
        if (_temporaryStructures.ContainsKey(position))
        {
            Debug.Log("Temp Contains Key");
            _altTemporaryStructures.Add(position,_temporaryStructures[position]);
            return;
        }
        GridExtension.SetGridCell(new Point(position.x, position.z), cellType);
        var structure = CreateNewStructure(position);
        _temporaryStructures.Add(position, structure);
        _temporaryStructures.Add(new Vector3Int(),new Structure());
        _temporaryStructures.Remove(position);
    }
    
    private Structure CreateNewStructure(Vector3Int position)
    {
        var gO = new GameObject(new Point(position.x,position.z).ToString());
        gO.transform.SetParent(_structureParent);
        gO.transform.localPosition = position;
        var structure = gO.AddComponent<Structure>();
        return structure;
    }
    
    public void ModifyStructure(Vector3Int position, GameObject newModel, Quaternion rotation)
    {
        if (_temporaryStructures.TryGetValue(position, out var tempStructure))
        {
            tempStructure.SwapModel(newModel, rotation);
        }
        else if (_permanentStructures.TryGetValue(position, out var permStructure))
        {
            permStructure.SwapModel(newModel, rotation);
        }
    }
    
    public void RemoveAllTemporaryStructures()
    {
        foreach (var structure in _temporaryStructures.Where(structure => !_altTemporaryStructures.ContainsKey(structure.Key)))
        {
            var pos = Vector3Int.RoundToInt(structure.Value.transform.position);
            GridExtension.SetGridCell(new Point(pos.x, pos.z), CellType.Empty);
            Game.Game.instance.DestroyGO(structure.Value.gameObject);
        }
        _temporaryStructures.Clear();
        _temporaryStructures = _altTemporaryStructures;
        _altTemporaryStructures.Clear();
    }    
    
    public void ChangeTemporaryToPermanentStructures()
    {
        foreach (var structure in _temporaryStructures)
        {
            _permanentStructures.Add(structure.Key,structure.Value);
            DestroyNature(structure.Key);
        }
        _temporaryStructures.Clear();
    }

    private void DestroyNature(Vector3Int position)
    {
        var hits = Physics.BoxCastAll(position + new Vector3(0, 0.5f, 0), 
                                            new Vector3(.5f, .5f, .5f), 
                                            Vector3.up, 
                                                    Quaternion.identity, 
                                            1f,
                                            1 << LayerMask.NameToLayer("Nature"));
        foreach (var hit in hits)
        {
            Game.Game.instance.DestroyGO(hit.collider.gameObject);
        }
    }

    public void ChangeSelection(Input input, CellType selectedType)
    {
        ClearInputActions(input);
        switch (selectedType)
        {
            case CellType.Street:
                input.onMouseClick += _streetPlacement.CreateStructure;
                input.onMouseHold += _streetPlacement.CreateStructure;
                input.onMouseUp += _streetPlacement.FinishPlacement;
                break;
            case CellType.House:
                input.onMouseClick += _housePlacement.CreateStructure;
                input.onMouseHold += _housePlacement.CreateStructure;
                input.onMouseUp += _housePlacement.FinishPlacement;
                break;
            case CellType.Special:
                break; 
        }
    }

    private void ClearInputActions(Input input)
    {
        input.onMouseClick = null;
        input.onMouseHold = null;
        input.onMouseUp = null;
    }
}
