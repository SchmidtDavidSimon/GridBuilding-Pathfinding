using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using Grid;
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
        private GridExtension1 _gE;

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
            
            //new GridExtension(width, height);
            //_placementHandler = new PlacementHandler(modelParent, cellContents);
            
            _gE = new GridExtension1(width, height);
            Debug.Log($"Width: {GridExtension1.GridWidth}");
            Debug.Log($"Width: {GridExtension1.GridHeight}");
            Debug.Log($"Type: {GridExtension1.GetCell(Vector3Int.zero)}");
            GridExtension1.SetCell(Vector3Int.zero, cellContents.Find(content => content.name == "Street"));
            Debug.Log($"Type: {GridExtension1.GetCell(Vector3Int.zero)}");
            GridExtension1.EmptyCell(Vector3Int.zero);
            Debug.Log($"Type: {GridExtension1.GetCell(Vector3Int.zero)}");
            Debug.Log($"InBound: {GridExtension1.CellIsInBound(Vector3Int.zero)}");
            Debug.Log($"IsStreet: {GridExtension1.CellIsOfType(Vector3Int.zero, CellContentType.Street)}");
            Debug.Log($"IsFree: {GridExtension1.CellIsFree(Vector3Int.zero)}");
            foreach (var neighborType in GridExtension1.GetNeighborTypes(Vector3Int.zero))
            {
                Debug.Log($"neighborType: {neighborType}");
            }

            foreach (var neighbors in GridExtension1.GetNeighborsOfTypes(Vector3Int.zero,
                new List<CellContentType> {CellContentType.None}))
            {
                Debug.Log($"None neighbors: ({neighbors.x}|{neighbors.z})");
            }
            foreach (var neighbors in GridExtension1.GetNeighborsOfTypes(Vector3Int.zero, null))
            {
                Debug.Log($"Neighbor cells: ({neighbors.x}|{neighbors.z})");
            }

            foreach (var cell in GridExtension1.GetPathOfTypeBetween(Vector3Int.zero, Vector3Int.forward, null))
            {
                Debug.Log($"Path goes on: ({cell.x}|{cell.z})");
            }
            foreach (var cell in GridExtension1.GetPathOfTypeBetween(Vector3Int.zero, Vector3Int.forward, new List<CellContentType>{CellContentType.None}))
            {
                Debug.Log($"Path goes on: ({cell.x}|{cell.z})");
            }
        }

        /// <summary>
        /// Place vegetation on all cells to have the starting models
        /// Connect UI and input actions with the placement handler
        /// </summary>
        private void Start() 
        {
             // _placementHandler.PlaceAll(CellContentType.Vegetation);
             // ui.contentSelected += _placementHandler.SetSelectedContent;
             // ui.contentSelected += SetAudio;
             // _input.mouseDown += _placementHandler.Place;
             // _input.mouseUp += _placementHandler.FinishPlacement;
        }

        private void SetAudio(CellContentType? contentType)
        {
            if (contentType != null)
            {
                _input.mouseUp += AudioPlayer.PlayPlacementSound;
            }
            else
            {
                _input.mouseUp -= AudioPlayer.PlayPlacementSound;
            }
        }

        private void OnDestroy()
        {
            _gE.Shutdown();
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
