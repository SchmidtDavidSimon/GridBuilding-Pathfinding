using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game
{
    public class Input : MonoBehaviour
    {
        public Action<Vector3Int> mouseHold;
        public Action mouseUp;
        
        [SerializeField] private UnityEngine.Camera mainCamera;
        [SerializeField] private LayerMask groundLayer;
        
         private void Update()
         {
             CheckClickUp();
             CheckClickHold();
         }

        private Vector3Int? RaycastGround()
        {
            if (mainCamera is null) return null;
            var ray = mainCamera.ScreenPointToRay(UnityEngine.Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, groundLayer)) 
                return Vector3Int.RoundToInt(hit.point);
            return null;
        }

        private void CheckClickUp()
        {
            if (!UnityEngine.Input.GetMouseButtonUp(0) || EventSystem.current.IsPointerOverGameObject()) return;
            mouseUp?.Invoke();
        }

        private void CheckClickHold()
        {
            if (!UnityEngine.Input.GetMouseButton(0) || EventSystem.current.IsPointerOverGameObject()) return;
            var pos = RaycastGround();
            if (pos == null) return;
            mouseHold?.Invoke(pos.Value);
        }
    }
}
