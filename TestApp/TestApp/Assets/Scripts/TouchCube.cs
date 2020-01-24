using System;
using System.Timers;
using UnityEngine;

public class TouchCube : MonoBehaviour
{
    
    private Ray _currentRay;
    private Camera _myCamera;
    private bool _hasTouchMoved;
    private Timer _touchTimer;

    private void Start()
    {
        _myCamera = Camera.main;
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            TouchingSomething();
        }
    }

    private void TouchingSomething()
    {
        foreach (Touch touch in Input.touches)
        {
            _currentRay = _myCamera.ScreenPointToRay(touch.position);
            Debug.DrawRay(_currentRay.origin, _currentRay.direction * 100, Color.blue);
            RaycastHit hitInfo;
            if (Physics.Raycast(_currentRay, out hitInfo))
            {
                Controllable controllable = hitInfo.transform.GetComponent<Controllable>();
                if (controllable.GetBounce()) { controllable.BounceCube(); }
                else { CubeColour(controllable); }
            }
        }
    }

    void CubeColour(Controllable controllable)
    {
        foreach (Touch touch in Input.touches)
        {
            switch (touch.phase)
            {
                case TouchPhase.Began:
                {
                    controllable.ChangeColour(Color.black);
                    break;
                }

                case TouchPhase.Stationary:
                {
                    controllable.ChangeColour(Color.blue);
                    break;
                }

                case TouchPhase.Moved:
                {
                    controllable.ChangeColour(Color.red);
                    break;
                }

                case TouchPhase.Ended:
                {
                    controllable.ChangeColour(Color.green);
                    break;
                }

                case TouchPhase.Canceled:
                {
                    controllable.ChangeColour(Color.yellow);
                    break;
                }
                
                default:
                {
                    Debug.LogError("This should never happen ever");
                    break;
                }
            }
        }
    }
}
