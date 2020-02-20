using UnityEngine;

public class Controllable : MonoBehaviour
{
    
    private Material _material;

    private void Start()
    {
        
        if (GetComponent <Renderer>() && GetComponent <Renderer>().material)
        {
            _material = GetComponent<Renderer>().material;
        }
    }

    public void ChangeColour(Color newColour)
    {
        _material.color = newColour;
    }
}
