using System.Linq;
using Grid;
using UnityEngine;

namespace Placements
{
    public class SpecialPlacement : StructurePlacement
    {
        protected SpecialPlacement(PlacementHandler handler) : base(handler) { }
        protected override bool Conditions(Vector3Int position)
        {
            var retVal = false;
            for (var i = 0; i < 2; i++)
            {
                for (var j = 0; j < 2; j++)
                {
                    if (!(GridExtension.PositionIsInBound(new Vector3Int(position.x + i, position.y, position.z + j)) 
                          && GridExtension.PositionIsFreeOrOfType(new Vector3Int(position.x+i,position.y,position.z+j),CellType.Special))) 
                        return false;
                    
                    if (GridExtension.GetNeighborsOfType(new Vector3Int(position.x +i,position.y, position.z + j), CellType.Street).Count == 0) continue;
                    retVal = true;
                }
            }
            return retVal;
        }

        protected override void MouseDrag(Vector3Int position)
        {
            GridExtension.GetPathOfTypeBetween(startPos, position, CellType.Special);
            
            var idc = (from specialPosition in temporaryStructurePositions where !Conditions(specialPosition) select temporaryStructurePositions.IndexOf(specialPosition)).ToList();
            foreach (var idx in idc)
            {
                temporaryStructurePositions.RemoveAt(idx);
            }

            foreach (var specialPosition in temporaryStructurePositions.Where(GridExtension.PositionIsFree))
            {
                handler.CreateTemporaryStructure(specialPosition, CellType.Special);
            }
        }

        protected override void PlaceStructures()
        {
            foreach (var position in temporaryStructurePositions)
            {
                var structureInfo = StructureSelector.SelectStructureForPos(position, CellType.Special);
                handler.ModifyStructure(position, structureInfo.model, structureInfo.rotation);
            }
        }
    }
}
