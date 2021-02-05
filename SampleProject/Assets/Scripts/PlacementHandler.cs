using System.Collections.Generic;
using System.Linq;
using Grid;
using ScriptableObjects;
using UnityEngine;

/// <summary>
/// Class to handle the placement of cell content
/// </summary>
public class PlacementHandler
{
    private readonly Transform _modelParent;
    private readonly Dictionary<Vector3Int, CellContent> _contents = new Dictionary<Vector3Int, CellContent>();
    private readonly Dictionary<Vector3Int, CellContent> _temporaryContents = new Dictionary<Vector3Int, CellContent>();
    private readonly Dictionary<Vector3Int, CellContent> _positionsToCheckNeighborsOf = new Dictionary<Vector3Int, CellContent>();
    private readonly Dictionary<Vector3Int, CellContent> _permanentPathContent = new Dictionary<Vector3Int, CellContent>();
    private readonly Dictionary<Vector3Int, GameObject> _models = new Dictionary<Vector3Int, GameObject>();
    private readonly Dictionary<CellContentType, CellContent> _contentTemplates = new Dictionary<CellContentType, CellContent>();
    
    private CellContentType? _selectedContent;
    private Vector3Int? _startPos, _endPos;
    
    /// <summary>
    /// Enum to declare what kind of content should be deleted 
    /// </summary>
    private enum Delete
    {
        Permanent,
        Temporary,
    }
    
    public PlacementHandler(Transform modelParent, List<CellContent> cellContents)
    {
        _modelParent = modelParent;
        foreach (var cellContent in cellContents)
        {
            _contentTemplates.Add(cellContent.Type,cellContent);
        }
    }

    #region public methods

    /// <summary>
    /// Set all cells to of the grid to the given type
    /// </summary>
    /// <param name="type">The given type</param>
    public void SetAll(CellContentType type)
    {
        var content = _contentTemplates[type];
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
        ReplaceNeighbors();
    }
    
    /// <summary>
    /// Set the selected content type to the given type
    /// </summary>
    /// <param name="selectedContent">The given type</param>
    public void SetSelectedContent(CellContentType? selectedContent) => _selectedContent = selectedContent;

    /// <summary>
    /// Place the selected content onto the given position
    /// </summary>
    /// <param name="pos">The given position</param>
    public void Place(Vector3Int pos)
    {
        if (_selectedContent == null) return;
        if (_endPos == pos) return;

        //Delete temp content
        DeleteTemporaryContent();
        //Calculate path
        var content = _contentTemplates[(CellContentType) _selectedContent];
        var path = CalculatePath(pos, content);
        // Debug.Log(path.Count);
        //Correct path
        content.CorrectPath(ref path);
        //Delete perm content on path
        DeleteContentOnPath(path);
        //Place path
        PlacePath(path, content);
        //Replace neighbors
        ReplaceNeighbors();
    }
    
    public void FinishPlacement()
    {
        _startPos = null;
        _endPos = null;
        PermanentizeTemporaryContents();
        _permanentPathContent.Clear();
        Game.Game.instance.AudioPlayer.PlayPlacementSound();
    }

    #endregion
    
    #region private methods

    #region placement
    
    private void PlacePath(List<Vector3Int> path, CellContent content)
    {
        foreach (var pathPoint in path)
        {
            PlaceContent(pathPoint, content, true);
        }
    }
    
    private void PlaceContent(Vector3Int pos, CellContent content, bool placeTemporarily = false)
    {
        if (!ConditionsAreMet(pos, content, placeTemporarily)) return;
        if (placeTemporarily && _permanentPathContent.ContainsKey(pos))
        {
            placeTemporarily = false;
        }

        PlaceModel(pos, content);
        
        if (placeTemporarily)
        {
            for (var i = 0; i < content.Width; i++)
            {
                for (var j = 0; j < content.Height; j++)
                {
                    _temporaryContents.Add(new Vector3Int(pos.x + i, pos.y, pos.z + j), content);
                }
            }
        }
        else
        {
            for (var i = 0; i < content.Width; i++)
            {
                for (var j = 0; j < content.Height; j++)
                {
                    _contents.Add(new Vector3Int(pos.x + i, pos.y, pos.z + j), content);
                }
            }
        }

        if (_positionsToCheckNeighborsOf.ContainsKey(pos)) return;
        _positionsToCheckNeighborsOf.Add(pos,content);
    }
    
    private void PlaceModel(Vector3Int pos, CellContent content)
    {
        var model = content.GetModel(pos);
        model.transform.parent = _modelParent;
        model.name = $"P({pos.x}|{pos.z})";
        var modelPos = model.transform.position;
        model.transform.position = new Vector3(modelPos.x, 0f, modelPos.z);
        _models.Add(pos,model);
    }
    
    private void ReplaceNeighbors()
    {
        while (true)
        {
            var replacementArray = (from neighbor in _positionsToCheckNeighborsOf select neighbor).ToArray();
            if (replacementArray.Length == 0) return;
            foreach (var item in replacementArray)
            {
                foreach (var neighbor in GridExtension.GetNeighborPositions(item.Key))
                {
                    if (!_temporaryContents.TryGetValue(neighbor, out var neighborContent) && !_contents.TryGetValue(neighbor, out neighborContent)) continue;
                    if (!neighborContent.NeedsNewModel(neighbor, neighborContent.Type)) continue;
                    DeleteContent(neighbor, _contents.ContainsKey(neighbor) 
                        ? Delete.Permanent
                        : Delete.Temporary);
                    PlaceContent(neighbor, neighborContent, !_contents.ContainsKey(neighbor));
                    if (_positionsToCheckNeighborsOf.ContainsKey(neighbor)) continue;
                        _positionsToCheckNeighborsOf.Add(neighbor, neighborContent);
                }

                _positionsToCheckNeighborsOf.Remove(item.Key);
            }
        }
    }
    
    #endregion

    #region deletion
    
    private void DeleteTemporaryContent()
    {
        var deletionArray = (from temporaryContent in _temporaryContents select temporaryContent).ToArray();
        foreach (var item in deletionArray)
        {
            for (var i = 0; i < item.Value.Width; i++)
            {
                for (var j = 0; j < item.Value.Height; j++)
                {
                    DeleteContent(new Vector3Int(item.Key.x + i, item.Key.y, item.Key.z +j), Delete.Temporary);
                }
            }
        }
    }
    
    private void DeleteContent(Vector3Int pos, Delete contentsToDelete)
    {
        if (!GridExtension.EmptyCell(pos))
        {
            Debug.LogError($"Trying to clear a cell that is outside of the grid");
            return;
        }
        if (CellIsEmpty(pos, contentsToDelete)) return;
        
        AddToCheckNeighbors(pos, contentsToDelete);
        Game.Game.instance.DestroyGo(_models[pos]);
        _models.Remove(pos);
        if (contentsToDelete == Delete.Permanent && !_permanentPathContent.ContainsKey(pos) &&_contents[pos].Type == _selectedContent)
        {
            _permanentPathContent.Add(pos,_contents[pos]);
        }
        RemoveFromDict(pos, contentsToDelete);
    }
    
    private void DeleteContentOnPath(List<Vector3Int> path)
    {
        var deletionArray = (from pathPoint in path where _contents.ContainsKey(pathPoint) select pathPoint).ToArray();
        foreach (var pos in deletionArray)
        {
            var content = _contents[pos];
            for (var i = 0; i < content.Width; i++)
            {
                for (var j = 0; j < content.Height; j++)
                {
                    DeleteContent(new Vector3Int(pos.x + i, pos.y, pos.z + j), Delete.Permanent);
                }
            }
        }
    }
    
    #endregion

    #region utilities

    private bool CellIsEmpty(Vector3Int pos, Delete contentsToDelete)
    {
        switch (contentsToDelete)
        {
            case Delete.Permanent:
                if (!_contents.ContainsKey(pos))
                {
                    Debug.LogError("Trying to remove content that doesn't exists in perm");
                    return true;
                }
                break;
            case Delete.Temporary:
                if (!_temporaryContents.ContainsKey(pos))
                {
                    Debug.LogError("Trying to remove content that doesn't exists in temp");
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

    private void AddToCheckNeighbors(Vector3Int pos, Delete contentsToDelete)
    {
        while (true)
        {
            switch (contentsToDelete)
            {
                case Delete.Permanent:
                    _contents.TryGetValue(pos, out var content);
                    if (_positionsToCheckNeighborsOf.ContainsKey(pos)) break;
                    _positionsToCheckNeighborsOf.Add(pos, content);
                    break;
                case Delete.Temporary:
                    _temporaryContents.TryGetValue(pos, out content);
                    if (_positionsToCheckNeighborsOf.ContainsKey(pos)) break;
                    _positionsToCheckNeighborsOf.Add(pos, content);
                    break;
                default:
                    contentsToDelete = _contents.ContainsKey(pos) ? Delete.Permanent : Delete.Temporary;
                    continue;
            }
            break;
        }
    }

    private void RemoveFromDict(Vector3Int pos, Delete contentsToDelete)
    {
        while (true)
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
                    contentsToDelete = _contents.ContainsKey(pos) ? Delete.Permanent : Delete.Temporary;
                    continue;
            }

            break;
        }
    }

    private List<Vector3Int> CalculatePath(Vector3Int pos, CellContent content)
    {
        _startPos ??= pos;
        _endPos = pos;
        return GridExtension.GetPathOfTypeBetween(
            (Vector3Int)_startPos,
            (Vector3Int)_endPos,
            content.OverwritableContentTypes.ToArray()
        );
    }
    
    private bool ConditionsAreMet(Vector3Int pos, CellContent content, bool placeTemporarily)
    {
        for (var i = 0; i < content.Width; i++)
        {
            for (var j = 0; j < content.Height; j++)
            {
                var posToChek = new Vector3Int(pos.x + i, pos.y, pos.z + j);
                if (!GridExtension.CellIsFree(posToChek) || !GridExtension.CellIsInBound(posToChek))
                {
                    Debug.LogError($"Cell ({posToChek.x}|{posToChek.z}) is either not in bounds or not free");
                    return false;
                }

                if (placeTemporarily)
                {
                    if (_temporaryContents.ContainsKey(posToChek))
                    {
                        Debug.LogError($"Key {posToChek} already exists in _contents. Value: {_temporaryContents[posToChek]}");
                        return false;
                    }
                }
                else
                {
                    if (_contents.ContainsKey(posToChek))
                    {
                        Debug.LogError($"Key {posToChek} already exists in _contents. Value: {_contents[posToChek]}");
                        return false;
                    }
                }
        
                var successful = GridExtension.SetCell(posToChek, content);
                if (!successful)
                {
                    Debug.LogError($"Trying to build a structure that reaches outside of the grid");
                    return false;
                }
            }
        }

        return true;
    }
    
    private void PermanentizeTemporaryContents()
    {
        foreach (var temporaryContent in _temporaryContents)
        {
            _contents.Add(temporaryContent.Key,temporaryContent.Value);
        }
        _temporaryContents.Clear();
    }
    
    #endregion
    
    #endregion
}
