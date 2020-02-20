using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Reset : MonoBehaviour
{
    private const int Layer = 8;
    private List<Vector3> _positions;
    private List<Vector3> _scales;
    private List<Quaternion> _rotations;
    private float _cameraFov;
    
    
    // Start is called before the first frame update
    void Start()
    {
        GameObject[] objectsToReset = FindResettableObjects();
        _positions = new List<Vector3>();
        _scales = new List<Vector3>();
        _rotations = new List<Quaternion>();

        foreach (GameObject currentObject in objectsToReset)
        {
            _positions.Add(currentObject.transform.position);
            _scales.Add(currentObject.transform.localScale);
            _rotations.Add(currentObject.transform.rotation);

            if (currentObject.GetComponent<Camera>())
            {
                _cameraFov = currentObject.GetComponent<Camera>().fieldOfView;
            }
        }
    }

    public void ResetEverything()
    {
        GameObject[] objectsToReset = FindResettableObjects();

        for (int i = 0; i < objectsToReset.Length; i++)
        {
            objectsToReset[i].transform.position = _positions[i];
            objectsToReset[i].transform.localScale = _scales[i];
            objectsToReset[i].transform.rotation = _rotations[i];
            
            if (objectsToReset[i].GetComponent<Camera>())
            {
                objectsToReset[i].GetComponent<Camera>().fieldOfView = _cameraFov;
            }
        }
    }

    public void AddNewObjectToReset(GameObject newObject)
    {
        _positions.Add(newObject.transform.position);
        _scales.Add(newObject.transform.localScale);
        _rotations.Add(newObject.transform.rotation);
    }
    

    // found method here https://answers.unity.com/questions/179310/how-to-find-all-objects-in-specific-layer.html
    GameObject[] FindResettableObjects()
    {
        GameObject[] goArray = FindObjectsOfType<GameObject>();
        List<GameObject> goList = new List<GameObject>();
        foreach (GameObject goItem in goArray)
        {
            if (goItem.layer == Layer) {
                goList.Add(goItem);
            }
        }
        if (goList.Count == 0) {
            return null;
        }
        return goList.ToArray();
    }
}
