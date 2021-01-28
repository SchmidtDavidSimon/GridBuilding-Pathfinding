using Grid;
using SVS;
using UnityEngine;
using Input = Game.Input;

namespace Game
{
    [RequireComponent(typeof(Input),typeof(AudioPlayer))]
    public class Game : MonoBehaviour
    {
        public static Game instance;
    
        public AudioPlayer AudioPlayer { get; private set; }
    
        [Header("Grid Stuff")]
        [SerializeField] private int width = 15;
        [SerializeField] private int height = 15;
        [SerializeField] private Transform structureParent;
        [Header("Other Stuff")]
        [SerializeField] private CameraMovement cameraMovement;
        [SerializeField] private UI ui;
    
        private Input _input;
        private GridExtension _gridExtension;
        private PlacementHandler _placementHandler;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            AudioPlayer = GetComponent<AudioPlayer>();
            _input = GetComponent<Input>();
            
            _gridExtension = new GridExtension(width, height);
            _placementHandler = new PlacementHandler(structureParent);
        }

        private void Start()
        {
            _input.SetCam(cameraMovement.GameCamera);
        
            ui.OnStreetSelected += SelectionChangeHandler;
            ui.OnHouseSelected += SelectionChangeHandler;
            ui.OnSpecialSelected += SelectionChangeHandler;
        }

        private void SelectionChangeHandler(CellType selectedType) => _placementHandler.ChangeSelection(_input, selectedType);
        
        private void Update()
        {
            cameraMovement.MoveCamera(new Vector3(
                _input.CameraMovementVector.x,
                0,
                _input.CameraMovementVector.y));
        }

        public void DestroyGO(GameObject gO)
        {
            Destroy(gO);
        }
    }
}
