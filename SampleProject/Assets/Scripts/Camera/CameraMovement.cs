using UnityEngine;

namespace Camera
{
    /// <summary>
    /// Handles movement of the game camera
    /// </summary>
    public class CameraMovement : MonoBehaviour
    {
        #region fields

        [SerializeField] private float lookSpeedH = 2f;
        [SerializeField] private float lookSpeedV = 2f;
        [SerializeField] private float zoomSpeed = 2f;
        [SerializeField] private float dragSpeed = 3f;
 
        private float _yaw;
        private float _pitch;

        #endregion

        #region private methods

        /// <summary>
        /// Set the correct initial rotation
        /// </summary>
        private void Start()
        {
            var eulerAngles = transform.eulerAngles;
            _yaw = eulerAngles.y;
            _pitch = eulerAngles.x;
        }
 
        /// <summary>
        /// Check right mouse and adjust camera rotation based on the axes and the look-speeds
        /// Check middle mouse and adjust camera position based on the axis and the drag-speed
        /// Check the scroll-wheel and adjust camera position based on the axis and the zoom-speed 
        /// </summary>
        private void Update()
        {
            if (Input.GetMouseButton(1))
            {
                _yaw += lookSpeedH * Input.GetAxis("Mouse X");
                _pitch -= lookSpeedV * Input.GetAxis("Mouse Y");
 
                transform.eulerAngles = new Vector3(_pitch, _yaw, 0f);
            }
            
            if (Input.GetMouseButton(2))
            {
                transform.Translate(-Input.GetAxisRaw("Mouse X") * Time.deltaTime * dragSpeed, -Input.GetAxisRaw("Mouse Y") * Time.deltaTime * dragSpeed, 0);
            }
            
            transform.Translate(0, 0, Input.GetAxis("Mouse ScrollWheel") * zoomSpeed, Space.Self);
        }

        #endregion
    }
}