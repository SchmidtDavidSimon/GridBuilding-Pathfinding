using System.Linq;
using Grid;
using UnityEngine;

namespace Placements
{
    public class HousePlacement : StructurePlacement
    {
        public HousePlacement(PlacementHandler handler) : base(handler) { }

        protected override bool Conditions(Vector3Int position) =>
            GridExtension.GetNeighborsOfType(position, CellType.Street).Count != 0  &&
            GridExtension.PositionIsFreeOrOfType(position, CellType.House);
        

        protected override void MouseDrag(Vector3Int position)
        {
            temporaryStructurePositions = GridExtension.GetPathOfTypeBetween(startPos, position, CellType.House);
            var idc = (from housePosition in temporaryStructurePositions where !Conditions(housePosition) select temporaryStructurePositions.IndexOf(housePosition)).ToList();
            foreach (var idx in idc)
            {
                temporaryStructurePositions.RemoveAt(idx);
            }
            foreach (var housePosition in temporaryStructurePositions.Where(GridExtension.PositionIsFree))
            {
                handler.CreateTemporaryStructure(housePosition,CellType.House);
            }
        }

        protected override void PlaceStructures()
        {
            foreach (var position in temporaryStructurePositions)
            {
                var structureInfo = StructureSelector.SelectStructureForPos(position, CellType.House);
                handler.ModifyStructure(position, structureInfo.model, structureInfo.rotation);
            }
        }
    }
}
