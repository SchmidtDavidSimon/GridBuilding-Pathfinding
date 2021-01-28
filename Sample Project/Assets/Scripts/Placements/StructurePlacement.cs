using System.Collections.Generic;
using Grid;
using UnityEngine;

namespace Placements
{
    public class StructurePlacement
    {
        protected Vector3Int startPos;
        protected List<Vector3Int> temporaryStructurePositions = new List<Vector3Int>();
        
        private bool _mouseIsDragged;
        protected readonly PlacementHandler handler;

        protected StructurePlacement(PlacementHandler handler)
        {
            this.handler = handler;
        }

        public void CreateStructure(Vector3Int position)
        {
            if (!ConditionsStart(position)) return;
            if (!Conditions(position)) return;
                
            if (!_mouseIsDragged)
            {
                MouseClickStart(position);
                MouseClick();
            }
            else
            {
                MouseDragStart();
                MouseDrag(position);
            }
            PlaceStructures();
        }

        private bool ConditionsStart(Vector3Int position) => GridExtension.PositionIsInBound(position);
        
        private void MouseClickStart(Vector3Int position)
        {
            _mouseIsDragged = true;
            startPos = position;
            temporaryStructurePositions.Clear();
        }

        private void MouseDragStart()
        {
            handler.RemoveAllTemporaryStructures();
            temporaryStructurePositions.Clear();
        }
        
        

        protected virtual bool Conditions(Vector3Int position) => true;
        
        protected virtual void MouseClick() {}

        protected virtual void MouseDrag(Vector3Int position) {}
        
        protected virtual void PlaceStructures() { }

        public void FinishPlacement()
        {
            _mouseIsDragged = false;
            startPos = Vector3Int.zero;
            handler.ChangeTemporaryToPermanentStructures();
            if (temporaryStructurePositions.Count > 0)
            {
                Game.Game.instance.AudioPlayer.PlayPlacementSound();
            }
            temporaryStructurePositions.Clear();
        }
    }
}
