using System.Timers;
using UnityEngine;

public class TouchCube : MonoBehaviour
{
    private Camera _myCamera;
    private float _touchTimer;
    private bool _hasBeenMoved;
    private Controllable _currentlySelectedCube;
    private const float MaxTapTime = 0.12f;
    private readonly float _moveSpeed = 10f;

    private void Start()
    {
        _myCamera = Camera.main;
        _touchTimer = 0.0f;
    }

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            ProcessTouch();
        }

        _touchTimer += Time.deltaTime;
    }

    private void ProcessTouch()
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began) { ResetTouchChecks(); }

            else if (touch.phase == TouchPhase.Moved)
            {
                _hasBeenMoved = true;

                if (_currentlySelectedCube != null)
                {
                    Vector3 touchPosition = _myCamera.ScreenToWorldPoint(touch.position);
                    touchPosition.z = 0;
                    _currentlySelectedCube.SetDirection(touchPosition - _currentlySelectedCube.transform.position);
                    float directionX = _currentlySelectedCube.GetDirection().x;
                    float directionY = _currentlySelectedCube.GetDirection().y;
                    float directionZ = _currentlySelectedCube.GetDirection().z;
                    _currentlySelectedCube.GetRigidBody().velocity = new Vector3(directionX, directionY, directionZ) * _moveSpeed;
                }
            }
            
            else if (touch.phase == TouchPhase.Ended)
            {
                if (_currentlySelectedCube != null)
                {
                    _currentlySelectedCube.GetRigidBody().velocity = Vector3.zero;
                }
                if (_touchTimer <= MaxTapTime && !_hasBeenMoved)
                {
                    Ray currentRay = _myCamera.ScreenPointToRay(touch.position);
                    if (Physics.Raycast(currentRay, out RaycastHit hitInfo))
                    {
                        Controllable controllable = hitInfo.transform.GetComponent<Controllable>();
                        SetCurrentlySelected(controllable);
                    }
                    else
                    {
                        SetCurrentlySelected(null);
                    }
                }
            }
        }
    }

    private void ResetTouchChecks()
    {
        _touchTimer = 0.0f;
        _hasBeenMoved = false;
    }

    private void SetCurrentlySelected(Controllable controllable)
    {
        if (_currentlySelectedCube != null)
        {
            _currentlySelectedCube.ChangeColour(Color.white);
            _currentlySelectedCube = null;
        }

        if (controllable != null)
        {
            controllable.ChangeColour(Color.blue);
            _currentlySelectedCube = controllable;
        }
    }
}
