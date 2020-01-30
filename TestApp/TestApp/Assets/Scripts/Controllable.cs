using UnityEngine;

public class Controllable : MonoBehaviour
{
    
    private Material _material;
    private Rigidbody _rigidbody;
    private Vector3 _direction;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        
        if (GetComponent <Renderer>() && GetComponent <Renderer>().material)
        {
            _material = GetComponent<Renderer>().material;
        }
    }

    public Vector3 GetDirection()
    {
        return _direction;
    }

    public void SetDirection(Vector3 newDirection)
    {
        _direction = newDirection;
    }

    public Rigidbody GetRigidBody()
    {
        return _rigidbody;
    }

    public void ChangeColour(Color newColour)
    {
        _material.color = newColour;
    }
}
