/// <summary>
/// A combination of a prefab and a weight to select models based on their weight
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

    public static bool operator ==(ModelInfo x, ModelInfo y) => x.model == y.model && x.rotation == y.rotation;
    public static bool operator !=(ModelInfo x, ModelInfo y) => !(x == y);
    
}