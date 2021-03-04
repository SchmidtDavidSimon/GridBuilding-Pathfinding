using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using ScriptableObjects;
using UnityEngine;
using Utilities;

namespace Game
{
    /// <summary>
    /// Handles game-setup and acts as a service singleton.
    /// </summary>
    [RequireComponent(typeof(Input),typeof(AudioPlayer))]
    public class Game : MonoBehaviour
    {
        #region fields

        public static Game instance;
    
        public AudioPlayer AudioPlayer { get; private set; }
    
        [Header("Grid Stuff")]
        [SerializeField] private int width = 15;
        [SerializeField] private int height = 15;
        [SerializeField] private Transform modelParent;
        [Header("Cell Contents")]
        [SerializeField] private List<CellContent> cellContents;
        [Header("Other Stuff")]
        [SerializeField] private UI.UI ui;
    
        private Input _input;
        private PlacementHandler _placementHandler;
        private GridExtension _gridExtension;
        private bool _playingPlacementSound;

        #endregion

        #region private methods

        /// <summary>
        /// Set static instance
        /// Setup game mechanics:
        /// 1. get needed components
        /// 2. generate a new grid
        /// 3. create a placement handler
        /// </summary>
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
            _placementHandler = new PlacementHandler(modelParent, cellContents);
        }

        /// <summary>
        /// Place vegetation on all cells to have the starting models
        /// Connect UI and input actions with the placement handler
        /// </summary>
        private void Start() 
        {
             _placementHandler.PlaceAll(CellContentType.Vegetation);
             ui.contentSelected += _placementHandler.SetSelectedContent;
             ui.contentSelected += SetAudio;
             _input.mouseDown += _placementHandler.Place;
             _input.mouseUp += _placementHandler.FinishPlacement;
             
             // _placementHandler.SetSelectedContent(CellContentType.Residence1);
             // _placementHandler.Place(Vector3Int.zero);
             // _placementHandler.Place(new Vector3Int(0,0,10));
             // _placementHandler.FinishPlacement();
             
              _placementHandler.SetSelectedContent(CellContentType.Street);
              _placementHandler.Place(Vector3Int.zero);
              _placementHandler.Place(new Vector3Int(10,0,0));
              _placementHandler.FinishPlacement();
              _placementHandler.SetSelectedContent(CellContentType.Residence1);
              _placementHandler.Place(Vector3Int.forward);
              _placementHandler.Place(new Vector3Int(10,0,1));
              _placementHandler.FinishPlacement();
        }

        private void SetAudio(CellContentType? contentType)
        {
            if (contentType != null && !_playingPlacementSound)
            {
                _input.mouseUp += AudioPlayer.PlayPlacementSound;
                _playingPlacementSound = true;
            }
            if (contentType != null) return;
            _input.mouseUp -= AudioPlayer.PlayPlacementSound;
            _playingPlacementSound = false;
        }

        private void OnDestroy()
        {
            _gridExtension.Shutdown();
        }
        #endregion

        #region public methods

        /// <summary>
        /// Destroys a given GameObject
        /// </summary>
        public void DestroyGo(GameObject gO)
        {
            Destroy(gO);
        }

        #endregion
    }
}
