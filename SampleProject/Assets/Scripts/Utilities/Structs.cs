namespace Utilities
{
    /// <summary>
    /// A combination of a prefab and a float to select a model based on a weight
    /// </summary>
    [System.Serializable]
    public struct WeightedModel    
    {
        public UnityEngine.GameObject prefab;
        [UnityEngine.Range(0,1)] public float weight;
    }
    /// <summary>
    /// A combination of a model and a quaternion to save a models dedicated rotation for instantiation
    /// </summary>
    public struct ModelInfo
    {
        public UnityEngine.GameObject model;
        public UnityEngine.Quaternion rotation;
        
        private bool Equals(ModelInfo other) => Equals(model, other.model) && rotation.Equals(other.rotation);
        public override bool Equals(object obj) => obj is ModelInfo other && Equals(other);

        public static bool operator ==(ModelInfo x, ModelInfo y) => x.model == y.model && x.rotation == y.rotation;
        public static bool operator !=(ModelInfo x, ModelInfo y) => !(x == y);
    }
}