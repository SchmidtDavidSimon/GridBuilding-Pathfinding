using System.Collections.Generic;
using System.Linq;
using Grid;
using ScriptableObjects;
using UnityEngine;

namespace Utilities
{ 
    /// <summary>
    /// Handles The correct placement of cells including the data and the model.
    /// </summary>
    public class PlacementHandler
    {
        #region fields

        private readonly Transform _modelParent;
        private readonly Dictionary<Vector3Int, CellContent> _contents = new Dictionary<Vector3Int, CellContent>();
        private readonly Dictionary<Vector3Int, CellContent> _temporaryContents = new Dictionary<Vector3Int, CellContent>();
        private readonly Dictionary<Vector3Int, CellContent> _alteredCells = new Dictionary<Vector3Int, CellContent>();
        private readonly Dictionary<Vector3Int, CellContent> _permanentPathContent = new Dictionary<Vector3Int, CellContent>();
        private readonly Dictionary<Vector3Int, GameObject> _models = new Dictionary<Vector3Int, GameObject>();
        private readonly Dictionary<CellContentType, CellContent> _contentTemplates = new Dictionary<CellContentType, CellContent>();
    
        private CellContentType? _selectedContent;
        private Vector3Int? _startPos, _endPos;

        #endregion

        #region public methods

        /// <summary>
        /// Set the model parent and save all the contents in a dictionary by its type for easier use
        /// </summary>
        /// <param name="modelParent">A game objects transform where the instantiated models should be attached</param>
        /// <param name="cellContents">A List of all available cell contents</param>
        public PlacementHandler(Transform modelParent, List<CellContent> cellContents)
        {
            _modelParent = modelParent;
            foreach (var cellContent in cellContents)
            {
                _contentTemplates.Add(cellContent.Type,cellContent);
            }
        }
        
        /// <summary>
        /// Set all cells to of the grid to the given type.
        /// </summary>
        /// <param name="type">The given type</param>
        public void PlaceAll(CellContentType type)
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
        
        public void SetSelectedContent(CellContentType? selectedContent) => _selectedContent = selectedContent;

        /// <summary>
        /// Place the selected content onto the given position.
        /// 1. check if there is any content selected, and if the endPosition is a new one
        /// 2. delete the temporary content
        /// 3. get the selected content
        /// 4. get a new path
        /// 5. let the content correct its path
        /// 6. delete any permanent content on the path
        /// 7. place new content on the path,
        /// 8. replace neighbor content
        /// </summary>
        /// <param name="pos">the position of the mouse</param>
        public void Place(Vector3Int pos)
        {
            if (_selectedContent == null) return;
            if (_endPos == pos) return;
            
            DeleteTemporaryContent();
            var content = _contentTemplates[(CellContentType) _selectedContent];
            var path = CalculatePath(pos, content);
            content.CorrectPath(ref path);
            DeleteContentOnPath(path);
            PlaceContentOnPath(path, content);
            ReplaceNeighbors();
        }
    
        /// <summary>
        /// 1. Set both the start and the end position back to null
        /// 2. Turn temporary content into permanent content
        /// 3. Play placement sound
        /// </summary>
        public void FinishPlacement()
        {
            _startPos = null;
            _endPos = null;
            PermanentizeTemporaryContents();
        }

        #endregion
    
        #region private methods

        #region placement
        
        private void PlaceContentOnPath(List<Vector3Int> path, CellContent content)
        {
            foreach (var pathPoint in path)
            {
                PlaceContent(pathPoint, content, true);
            }
        }
    
        /// <summary>
        /// Place content on a specific cell
        /// 1. Check if conditions are met
        /// 2. If content should be placed temporarily, but is actually already permanent, set placeTemporarily to false
        /// 3. Place the model of the content
        /// 4. Add to corresponding dictionary
        /// 5. Add neighbors to be potentially replaced 
        /// </summary>
        /// <param name="pos">Position to be placed on</param>
        /// <param name="content">Content to be placed</param>
        /// <param name="placeTemporarily">Save as permanent or as temporary content</param>
        private void PlaceContent(Vector3Int pos, CellContent content, bool placeTemporarily = false)
        {
            if (!PlacementConditionsAreMet(pos, content, placeTemporarily)) return;
            if (placeTemporarily && _permanentPathContent.ContainsKey(pos))
            {
                placeTemporarily = false;
            }

            PlaceModel(pos, content);

            for (var i = 0; i < content.Width; i++)
            {
                for (var j = 0; j < content.Height; j++)
                {
                    if (placeTemporarily)
                    {
                        _temporaryContents.Add(new Vector3Int(pos.x + i, pos.y, pos.z + j), content);
                    }
                    else
                    {
                        _contents.Add(new Vector3Int(pos.x + i, pos.y, pos.z + j), content);
                    }
                }
            }

            if (!_alteredCells.ContainsKey(pos))
            {
                _alteredCells.Add(pos, content);
            }
        }
    
        /// <summary>
        /// Place model on a specific cell
        /// 1. Get the model from the content,
        /// 2. Set the models properties (parent, name, position)
        /// 3. Add to model dict
        /// </summary>
        private void PlaceModel(Vector3Int pos, CellContent content)
        {
            var model = content.GetModel(pos);
            model.transform.parent = _modelParent;
            model.name = $"P({pos.x}|{pos.z})";
            var modelPos = model.transform.position;
            model.transform.position = new Vector3(modelPos.x, 0f, modelPos.z);
            _models.Add(pos,model);
        }
    
        /// <summary>
        /// Replace Neighbor Models if needed to
        /// 1. Create an array of keyValuePairs out of the replacement dictionary to alter the dict while going through it
        /// 2. As soon as the replacement array is empty return
        /// 3. Foreach neighbor of the altered cells do:
        ///     1. Check if there is content in there
        ///     2. Check if the neighbor needs a new model
        ///     3. Delete the old content
        ///     4. Place new content
        ///     5. Add neighbor to altered cells dict
        /// 4. Remove currently checked position from altered cells dict
        /// </summary>
        private void ReplaceNeighbors()
        {
            while (true)
            {
                var alteredArray = (from alteredCell in _alteredCells select alteredCell.Key).ToArray();
                if (alteredArray.Length == 0) return;
                foreach (var cell in alteredArray)
                {
                    var neighbors = GridExtension.GetNeighborPositions(cell);
                    foreach (var neighbor in neighbors)
                    {
                        if (!_temporaryContents.TryGetValue(neighbor, out var neighborContent) && !_contents.TryGetValue(neighbor, out neighborContent)) continue;
                        if (!neighborContent.NeedsNewModel(neighbor, neighborContent.Type)) continue;
                        DeleteContent(neighbor, _contents.ContainsKey(neighbor));
                        PlaceContent(neighbor, neighborContent, !_contents.ContainsKey(neighbor));
                        if (_alteredCells.ContainsKey(neighbor)) continue;
                        _alteredCells.Add(neighbor, neighborContent);
                    }

                    _alteredCells.Remove(cell);
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
                        DeleteContent(new Vector3Int(item.Key.x + i, item.Key.y, item.Key.z +j), false);
                    }
                }
            }
        }
    
        /// <summary>
        /// Delete content on a given position
        /// 1. Empty the cell in the data grid
        /// 2. Check if cell is in one of the dictionarys
        /// 3. Add the cell to the altered cells
        /// 4. Destroy the model
        /// 5. Delete the pos from the model dict
        /// 6. If the content was permanent, add it to the appropriate dict
        /// 7. Remove the content from the correct dict
        /// </summary>
        /// <param name="pos">position to delete content of</param>
        /// <param name="contentsToDelete"></param>
        private void DeleteContent(Vector3Int pos, bool deletePermanent)
        {
            if (!GridExtension.EmptyCell(pos))
            {
                Debug.LogError($"Trying to clear a cell that is outside of the grid");
                return;
            }
            if (!CellIsSaved(pos, deletePermanent)) return;
        
            AddToAlteredCells(pos, deletePermanent);
            Game.Game.instance.DestroyGo(_models[pos]);
            _models.Remove(pos);
            if (deletePermanent && !_permanentPathContent.ContainsKey(pos) &&_contents[pos].Type == _selectedContent)
            {
                _permanentPathContent.Add(pos,_contents[pos]);
            }
            RemoveFromDict(pos, deletePermanent);
        }
    
        /// <summary>
        /// Delete Content that is still on the path, so it must be permanent content.
        /// 1. Create an array of positions out of the contents dictionary, which are on the path to alter the dict while going through it
        /// 2. Delete each permanent content on the path for that contents dimensions
        /// </summary>
        /// <param name="path"></param>
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
                        DeleteContent(new Vector3Int(pos.x + i, pos.y, pos.z + j), true);
                    }
                }
            }
        }
    
        #endregion

        #region utilities
        
        private bool CellIsSaved(Vector3Int pos, bool deletePermanent)
        {
            if (deletePermanent)
            {
                if (_contents.ContainsKey(pos)) return true;
                Debug.LogError("Trying to remove content that doesn't exists in perm");
                return false;
            }

            if (_temporaryContents.ContainsKey(pos)) return true;
            Debug.LogError("Trying to remove content that doesn't exists in temp");
            return false;
        }
        
        private void AddToAlteredCells(Vector3Int pos, bool deletePermanent)
        {
            if (deletePermanent)
            {
                _contents.TryGetValue(pos, out var content);
                if (_alteredCells.ContainsKey(pos)) return;
                _alteredCells.Add(pos, content);
            }
            else
            {
                _temporaryContents.TryGetValue(pos, out var content);
                if (_alteredCells.ContainsKey(pos)) return;
                _alteredCells.Add(pos, content);
            }
        }

        private void RemoveFromDict(Vector3Int pos, bool deletePermanent)
        {
            if (deletePermanent)
            {
                _contents.Remove(pos);
            }
            else
            {
                _temporaryContents.Remove(pos);
            }
        }

        /// <summary>
        /// Define the start and end positions and return a path created by the Grid
        /// </summary>
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
    
        /// <summary>
        /// Checks if the position is available to be placed
        /// For the whole ground of the content check if:
        /// 1. The cell is free and in bounds
        /// 2. The position is already saved in one of the dictionarys
        /// 3. The grid was able to set the cells 
        /// </summary>
        private bool PlacementConditionsAreMet(Vector3Int pos, CellContent content, bool placeTemporarily)
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
    
        /// <summary>
        /// Move all the temporary contents to the permanent contents and clear the responsible dictionarys
        /// </summary>
        private void PermanentizeTemporaryContents()
        {
            foreach (var temporaryContent in _temporaryContents)
            {
                _contents.Add(temporaryContent.Key,temporaryContent.Value);
            }
            _temporaryContents.Clear();
            _permanentPathContent.Clear();
        }
    
        #endregion
    
        #endregion
    }
}
