using System.Collections.Generic;
using System.Linq;
using Grid;
using ScriptableObjects;
using UnityEngine;

public class PlacementHandler
{
    private readonly Transform _modelParent;
    private readonly List<CellContent> _cellContents;

    private Dictionary<Vector3Int, CellContent> _contents = new Dictionary<Vector3Int, CellContent>();
    private Dictionary<Vector3Int, CellContent> _temporaryContents = new Dictionary<Vector3Int, CellContent>();
    private Dictionary<Vector3Int, GameObject> _models = new Dictionary<Vector3Int, GameObject>();
    private CellContentType? _selectedContent;
    private Vector3Int? _startPos;
    
    private enum Delete
    {
        Permanent,
        Temporary,
        Any
    }
    public PlacementHandler(Transform modelParent, List<CellContent> cellContents)
    {
        _modelParent = modelParent;
        _cellContents = cellContents;
    }

    #region public methods

    public void SetAll(CellContentType type)
    {
        var content = _cellContents.First(cellContent => cellContent.Type == type);
        if (content.HasSpecialBuildInstructions)
        {
            if (content.BuildInstructions.RequiresCellContentTypeAsNeighbor)
            {
                Debug.LogError($"Trying to fill grid with content that has adjacency requirements: {type} \nRequired type {content.BuildInstructions.NeighborRequirements.First()}");
                return;
            }
        }

        for (var i = 0; i < GridExtension.GridWidth; i++)
        {
            for (var j = 0; j < GridExtension.GridHeight; j++)
            {
                PlaceContent(new Vector3Int(i, 0, j), content);
            }
        }
    }
    
    public void SetSelectedContent(CellContentType selectedContent) => _selectedContent = selectedContent;

    public void Place(Vector3Int pos)
    {
        var deletionList = (from temporaryContent in _temporaryContents select temporaryContent.Key).ToArray();
        foreach (var item in deletionList)
        {
            DeleteContent(item,Delete.Temporary);
        }
        if (_selectedContent == null) return;
        
        var content = _cellContents.First(cellContent => cellContent.Type == _selectedContent);
        _startPos ??= pos;
        var path = GridExtension.GetPathOfTypeBetween(
            (Vector3Int)_startPos,
            pos,
            content.OverwritableContentTypes.ToArray()
        );
        foreach (var pathPos in path)
        {
            if (!deletionList.Contains(pathPos))
            {
                DeleteContent(pathPos);
            }
            PlaceContent(pathPos, content, true);
        }
    }

    public void FinishPlacement()
    {
        _startPos = null;
        PermanentizeTemporaryContents();
    }

    #endregion

    #region private methods

    #region placement

    private void PlaceContent(Vector3Int pos, CellContent content, bool placeTemporarily = false)
    {
        if (!GridExtension.CellIsFree(pos) || !GridExtension.CellIsInBound(pos))
        {
            Debug.LogError($"Cell ({pos.x}|{pos.z}) is either not in bounds or not free");
            return;
        }
        if (_contents.ContainsKey(pos))
        {
            Debug.LogError($"Key {pos} already exists in _contents. Value: {_contents[pos]}");
            return;
        }
        
        var successful = GridExtension.SetCell(pos, content);
        if (!successful)
        {
            Debug.LogError($"Trying to build a structure that reaches outside of the grid");
            return;
        }
        
        var model = content.GetModel(pos);
        model.transform.parent = _modelParent;
        model.name = $"P({pos.x}|{pos.z})";
        var modelPos = model.transform.position;
        model.transform.position = new Vector3(modelPos.x, 0f, modelPos.z);
        _models.Add(pos,model);
        if (placeTemporarily)
        {
            _temporaryContents.Add(pos, content);
        }
        else
        {
            _contents.Add(pos,content);
        }
        foreach (var neighbor in GridExtension.GetNeighborPositions(pos))
        {
            if (!_temporaryContents.TryGetValue(neighbor, out var neighborContent) && !_contents.TryGetValue(neighbor, out neighborContent)) continue;
            if (!neighborContent.NeedsNewModel(neighbor,content.Type)) continue;
            DeleteContent(neighbor);
            PlaceContent(neighbor, neighborContent);
        }
    }
    
    private void PermanentizeTemporaryContents()
    {
        foreach (var temporaryContent in _temporaryContents)
        {
            _contents.Add(temporaryContent.Key,temporaryContent.Value);
        }
        _temporaryContents = new Dictionary<Vector3Int, CellContent>();
    }

    #endregion

    #region deletion

    private void DeleteContent(Vector3Int pos, Delete contentsToDelete = Delete.Any)
    {
        if (CellIsEmpty(pos, contentsToDelete)) return;
        
        var successful = GridExtension.EmptyCell(pos);
        if (!successful)
        {
            Debug.LogError($"Trying to clear a cell that is outside of the grid");
            return;
        }
        Game.Game.instance.DestroyGo(_models[pos]);
        _models.Remove(pos);
        RemoveFromDict(pos, contentsToDelete);
    }

    private void RemoveFromDict(Vector3Int pos, Delete contentsToDelete)
    {
        switch (contentsToDelete)
        {
            case Delete.Permanent:
                _contents.Remove(pos);
                break;
            case Delete.Temporary:
                _temporaryContents.Remove(pos);
                break;
            default:
                if (_contents.ContainsKey(pos))
                {
                    _contents.Remove(pos);
                }
                else
                {
                    _temporaryContents.Remove(pos);
                }
                break;
        }
    }

    private bool CellIsEmpty(Vector3Int pos, Delete contentsToDelete)
    {
        switch (contentsToDelete)
        {
            case Delete.Permanent:
                if (!_contents.ContainsKey(pos))
                {
                    Debug.LogError("Trying to remove content that doesn't exists");
                    return true;
                }
                break;
            case Delete.Temporary:
                if (!_temporaryContents.ContainsKey(pos))
                {
                    Debug.LogError("Trying to remove content that doesn't exists");
                    return true;
                }
                break;
            default:
                if (!_contents.ContainsKey(pos) && !_temporaryContents.ContainsKey(pos))
                {
                    Debug.LogError("Trying to remove content that doesn't exists");
                    return true;
                }
                break;
        }
        return false;
    }

    #endregion

    #endregion
}
