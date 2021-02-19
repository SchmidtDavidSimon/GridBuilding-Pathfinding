using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace ScriptableObjects
{
    /// <summary>
    /// Scriptable object that describes any type of content that can be placed on a cell 
    /// </summary>
    [CreateAssetMenu(menuName = "Scriptable Objects/Cell Content/Cell Content")]
    public class CellContent : ScriptableObject
    {
        #region fields

        [SerializeField] private int width, height;
        [SerializeField] private List<GameObject> models;
        [SerializeField] private bool hasSpecialBuildInstructions;
        [SerializeField] private CellContentBuildInstructions buildInstructions;
        [SerializeField] private CellContentType type;
        [SerializeField] private bool allowsMovement;
        [SerializeField] private List<MovementType> allowedMovementTypes;
        [SerializeField] private int movementCost;
        [SerializeField] private bool hasAppeal;
        [SerializeField] private int appeal;
        [SerializeField] private List<CellContentType> overwritableContentTypes;

        public int Width => width;
        public int Height => height;
        public bool AllowsMovement => allowsMovement;
        public CellContentType Type => type;
        public int MovementCost => movementCost;
        public bool HasSpecialBuildInstructions => hasSpecialBuildInstructions;
        public CellContentBuildInstructions BuildInstructions => buildInstructions;
        public bool HasAppeal => hasAppeal;
        public int Appeal => appeal;
        /// <summary>
        /// Returns the previously defined overwritableContentType, adds CellContentType.None and the own type because they applies to every content and so they don't have to be assigned in the inspector each time. 
        /// </summary>
        public List<CellContentType> OverwritableContentTypes
        {
            get
            {
                var retVal = new List<CellContentType>();
                retVal.AddRange(overwritableContentTypes);
                retVal.Add(type);
                retVal.Add(CellContentType.None);
                return retVal;
            }
        }

        #endregion

        #region public methods

        /// <summary>
        /// If the content doesn't have specialBuildingInstructions return a random gameObject out of the models
        /// If not let the instructions create a ModelInfo and return the instantiated GameObject based on that ModelInfo
        /// </summary>
        /// <param name="pos">position</param>
        /// <returns>Returns instantiated GameObject for that position</returns>
        public GameObject GetModel(Vector3Int pos)
        {
            if (!hasSpecialBuildInstructions) return Instantiate(models[Random.Range(0, models.Count - 1)], pos, Quaternion.identity);
            var info = buildInstructions.SelectModel(pos, width, height);
            return Instantiate(info.model, pos, info.rotation);
        }
        
        public bool NeedsNewModel(Vector3Int pos, CellContentType neighbor) 
            => hasSpecialBuildInstructions && buildInstructions.NeedsNewModel(pos, neighbor, width, height);
        
        public void CorrectPath(ref List<Vector3Int> path)
        {
            if (!hasSpecialBuildInstructions) return;
            buildInstructions.CorrectPath(ref path, width, height);
        }

        #endregion
    }
}
