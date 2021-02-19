using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game
{
    /// <summary>
    /// Handles the mouse input
    /// </summary>
    public class Input : MonoBehaviour
    {
        #region actions

        public Action<Vector3Int> mouseDown;
        public Action mouseUp;

        #endregion

        #region fields

        [SerializeField] private UnityEngine.Camera mainCamera;
        [SerializeField] private LayerMask groundLayer;

        #endregion

        #region private methods

        /// <summary>
        /// Check for mouse input every frame
        /// </summary>
        private void Update()
        {
            CheckClickUp();
            CheckClickDown();
        }

        /// <summary>
        /// If the mouse is clicked and not over the ui
        /// If so: Send a raycast to the ground and get the ground pos
        /// If the pos exists
        /// Invoke the mouseDown action
        /// </summary>
        private void CheckClickDown()
        {
            if (!UnityEngine.Input.GetMouseButton(0) || EventSystem.current.IsPointerOverGameObject()) return;
            var pos = RaycastGround();
            if (pos == null) return;
            mouseDown?.Invoke(pos.Value);
        }
        
        /// <summary>
        /// If the mouse button goes up and not over the ui
        /// Invoke the mouseUp action
        /// </summary>
        private void CheckClickUp()
        {
            if (!UnityEngine.Input.GetMouseButtonUp(0) || EventSystem.current.IsPointerOverGameObject()) return;
            mouseUp?.Invoke();
        }
        
        #region utilities

        /// <summary>
        /// If there is a main camera
        /// Cast a ray from the mouse position to the world to get the clicked position
        /// IF the ray hits return the world position
        /// </summary>
        private Vector3Int? RaycastGround()
        {
            if (mainCamera is null) return null;
            var ray = mainCamera.ScreenPointToRay(UnityEngine.Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, groundLayer)) 
                return Vector3Int.RoundToInt(hit.point);
            return null;
        }

        #endregion

        #endregion
    }
}
