using UnityEngine;

public class Structure : MonoBehaviour
{
    private float _yHeigth;
    public void SwapModel(GameObject model, Quaternion rotation)
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        var structure = Instantiate(model, transform);
        structure.transform.localPosition = new Vector3(0,_yHeigth,0);
        structure.transform.localRotation = rotation;
    }
}
