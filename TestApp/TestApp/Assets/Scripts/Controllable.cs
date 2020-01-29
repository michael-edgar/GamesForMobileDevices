using UnityEngine;

public class Controllable : MonoBehaviour
{
    [SerializeField]
    private bool bounce = false;
    private Material _material;
    private int _direction = 1;

    public bool GetBounce()
    {
        return bounce;
    }

    private void Start()
    {
        if (GetComponent <Renderer>() && GetComponent <Renderer>().material)
        {
            _material = GetComponent<Renderer>().material;
        }
    }
    
    public void BounceCube()
    {
        gameObject.transform.position += Vector3.up * _direction;
            
        if (gameObject.transform.position.y >= 6.0f)
        {
            _direction = -1;
        }
            
        else if (gameObject.transform.position.y <= -4.0f)
        {
            _direction = 1;
        }
    }

    public void ChangeColour(Color newColour)
    {
        _material.color = newColour;
    }
}
