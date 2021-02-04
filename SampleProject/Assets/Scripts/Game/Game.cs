using System.Collections.Generic;
using Grid;
using ScriptableObjects;
using UnityEngine;

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
        [SerializeField] private Transform modelParent;
        [Header("Cell Contents")]
        [SerializeField] private List<CellContent> cellContents;
        [Header("Other Stuff")]
        [SerializeField] private UI ui;
    
        private Input _input;
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
            
            new GridExtension(width, height);
            _placementHandler = new PlacementHandler(modelParent, cellContents);
        }

        private void Start() 
        {
            //_placementHandler.SetAll(CellContentType.Vegetation);
            ui.contentSelected += _placementHandler.SetSelectedContent;
            _input.mouseHold += _placementHandler.Place;
            _input.mouseUp += _placementHandler.FinishPlacement;
            
             _placementHandler.SetSelectedContent(CellContentType.Street);
             _placementHandler.Place(new Vector3Int(8,0,5));
             _placementHandler.Place(new Vector3Int(8,0,6));
             _placementHandler.Place(new Vector3Int(8,0,7));
             _placementHandler.FinishPlacement();
        }

        public void DestroyGo(GameObject gO)
        {
            Destroy(gO);
        }
    }
}
