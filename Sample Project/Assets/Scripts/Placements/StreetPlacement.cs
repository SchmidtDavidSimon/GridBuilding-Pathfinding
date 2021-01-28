using System.Collections.Generic;
using System.Linq;
using Grid;
using UnityEngine;

namespace Placements
{
    public class StreetPlacement : StructurePlacement
    {
        private readonly List<Vector3Int> _streetsToFix = new List<Vector3Int>();
        public StreetPlacement(PlacementHandler handler) : base(handler) { }

        protected override bool Conditions(Vector3Int position) =>
            GridExtension.PositionIsFreeOrOfType(position, CellType.Street);

        protected override void MouseClick()
        {
            _streetsToFix.Clear();
        }

        protected override void MouseDrag(Vector3Int position)
        {
            temporaryStructurePositions = GridExtension.GetPathOfTypeBetween(startPos, position, CellType.Street);
            
            foreach (var streetPosition in temporaryStructurePositions)
            {
                handler.CreateTemporaryStructure(streetPosition, CellType.Street);
            }

            foreach (var streetPos in _streetsToFix)
            {
                StructureSelector.SelectStructureForPos(streetPos, CellType.Street);
            }
        }

        protected override void PlaceStructures()
        {
            foreach (var position in temporaryStructurePositions)
            {
                var structureInfo = StructureSelector.SelectStructureForPos(position, CellType.Street);
                handler.ModifyStructure(position, structureInfo.model,structureInfo.rotation);
                
                var neighbors = GridExtension.GetNeighborsOfType(position, CellType.Street);
                foreach (var neighbor in neighbors.Where(neighbor => !_streetsToFix.Contains(neighbor)))
                {
                    _streetsToFix.Add(neighbor);
                }

                foreach (var streetPos in _streetsToFix)
                {
                    structureInfo = StructureSelector.SelectStructureForPos(streetPos, CellType.Street);
                    handler.ModifyStructure(streetPos, structureInfo.model, structureInfo.rotation);
                }
            }
        }
    }
}
