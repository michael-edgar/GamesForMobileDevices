using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCube : MonoBehaviour
{
    [SerializeField]
    private bool _bounce = false;
    private int _direction = 1;

    void Update()
    {
        if (Input.touchCount > 0)
        {
            if (_bounce)
            {
                BounceCube();
            }
            else
            {
                DragCube();
            }
        }
    }

    void BounceCube()
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

    void DragCube()
    {
        foreach (Touch touch in Input.touches)
        {
            switch (touch.phase)
            {
                case TouchPhase.Began:
                {
                    
                    break;
                }

                case TouchPhase.Stationary:
                {
                    
                    break;
                }

                case TouchPhase.Moved:
                {
                        
                    break;
                }

                case TouchPhase.Ended:
                {
                    
                    break;
                }

                //TouchPhase.Canceled
                default:
                {
                    break;
                }
            }
        }
    }
}
