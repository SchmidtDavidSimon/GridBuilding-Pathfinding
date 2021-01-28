using UnityEngine;

namespace SVS
{
    /// <summary>
    /// Created by: Sunny Valley Studio 
    ///	https://svstudio.itch.io
    /// Altered by David Schmidt
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class CameraMovement : MonoBehaviour
    {
        [SerializeField] private float cameraMovementSpeed = 5;

        private Camera _gameCamera;

        public Camera GameCamera => _gameCamera;

        private void Awake()
        {
            _gameCamera = GetComponent<Camera>();
        }
        public void MoveCamera(Vector3 inputVector)
        {
            var movementVector = Quaternion.Euler(0,30,0) * inputVector;
            _gameCamera.transform.position += movementVector * Time.deltaTime * cameraMovementSpeed;
        }
    }
}