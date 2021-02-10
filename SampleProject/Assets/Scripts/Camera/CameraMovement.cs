using UnityEngine;

namespace Camera
{
    public class CameraMovement : MonoBehaviour
    {
        [SerializeField] private float lookSpeedH = 2f;
        [SerializeField] private float lookSpeedV = 2f;
        [SerializeField] private float zoomSpeed = 2f;
        [SerializeField] private float dragSpeed = 3f;
 
        private float _yaw;
        private float _pitch;
 
        private void Start()
        {
            // Initialize the correct initial rotation
            var eulerAngles = transform.eulerAngles;
            _yaw = eulerAngles.y;
            _pitch = eulerAngles.x;
        }
 
        private void Update()
        {
            //Look around with Right Mouse
            if (Input.GetMouseButton(1))
            {
                _yaw += lookSpeedH * Input.GetAxis("Mouse X");
                _pitch -= lookSpeedV * Input.GetAxis("Mouse Y");
 
                transform.eulerAngles = new Vector3(_pitch, _yaw, 0f);
            }
 
            //drag camera around with Middle Mouse
            if (Input.GetMouseButton(2))
            {
                transform.Translate(-Input.GetAxisRaw("Mouse X") * Time.deltaTime * dragSpeed, -Input.GetAxisRaw("Mouse Y") * Time.deltaTime * dragSpeed, 0);
            }

            //Zoom in and out with Mouse Wheel
            transform.Translate(0, 0, Input.GetAxis("Mouse ScrollWheel") * zoomSpeed, Space.Self);
        }
    }
}