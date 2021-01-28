using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game
{
    public class Input : MonoBehaviour
    {
        [SerializeField] private LayerMask groundMask;
    
        private Camera _mainCam;
        private Vector2 _cameraMovementVector;
    
        public Vector2 CameraMovementVector => _cameraMovementVector;
    
        public Action<Vector3Int> onMouseClick;
        public Action<Vector3Int> onMouseHold;
        public Action onMouseUp;

        public void SetCam(Camera camera)
        {
            _mainCam = camera;
        }
    
        private void Update()
        {
            CheckClickDown();
            CheckClickUp();
            CheckClickHold();
            CheckArrowInput();
        }

        private Vector3Int? RaycastGround()
        {
            var ray = _mainCam.ScreenPointToRay(UnityEngine.Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, groundMask)) 
                return Vector3Int.RoundToInt(hit.point);
            return null;
        }
    
        private void CheckClickDown()
        {
            if (!UnityEngine.Input.GetMouseButtonDown(0) || EventSystem.current.IsPointerOverGameObject()) return;
            var pos = RaycastGround();
            if (pos == null) return;
            onMouseClick?.Invoke(pos.Value);
        }

        private void CheckClickUp()
        {
            if (!UnityEngine.Input.GetMouseButtonUp(0) || EventSystem.current.IsPointerOverGameObject()) return;
            onMouseUp?.Invoke();
        }

        private void CheckClickHold()
        {
            if (!UnityEngine.Input.GetMouseButton(0) || EventSystem.current.IsPointerOverGameObject()) return;
            var pos = RaycastGround();
            if (pos == null) return;
            onMouseHold?.Invoke(pos.Value);
        }

        private void CheckArrowInput()
        {
            _cameraMovementVector = new Vector2(UnityEngine.Input.GetAxis("Horizontal"),
                UnityEngine.Input.GetAxis("Vertical"));
        }
    }
}
