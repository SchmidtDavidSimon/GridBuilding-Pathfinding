[System.Serializable]
public struct WeightedModel    
{
    public UnityEngine.GameObject prefab;
    [UnityEngine.Range(0,1)] public float weight;
}
public struct ModelInfo
{
    public UnityEngine.GameObject model;
    public UnityEngine.Quaternion rotation;

    public static bool operator ==(ModelInfo x, ModelInfo y) => x.model == y.model && x.rotation == y.rotation;
    public static bool operator !=(ModelInfo x, ModelInfo y) => !(x == y);
    
}