using UnityEngine;

public class TouchCube : MonoBehaviour
{
    private Camera _myCamera;
    private float _touchTimer;
    private bool _hasBeenMoved;
    private Controllable _currentlySelectedCube;
    private const float MaxTapTime = 0.12f;
    private float _distanceToCube;
    private Vector3 _rotationAxis;
    private float _startAngle;
    private Quaternion _startRotation;
    private float _yDistance;
    private float _xDistance;

    private void Start()
    {
        _myCamera = Camera.main;
        _touchTimer = 0.0f;
        _distanceToCube = 0.0f;
        _rotationAxis = _myCamera.transform.forward;
        _startAngle = 0.0f;
        _startRotation = Quaternion.identity;
        _yDistance = 0.0f;
        _xDistance = 0.0f;
    }

    private void Update()
    {
        _rotationAxis = _myCamera.transform.forward;
        
        if (Input.touchCount == 1)
        {
            ProcessTouchAndDrag();
        }
        
        if (Input.touchCount > 1)
        {
            ProcessScaleAndRotate();
        }

        _touchTimer += Time.deltaTime;
    }

    private void ProcessTouchAndDrag()
    {
        Touch touch = Input.touches[0];
        Ray currentRay = _myCamera.ScreenPointToRay(touch.position);
        switch (touch.phase)
        {
            case TouchPhase.Began:
            {
                ResetTouchChecks();

                if (Physics.Raycast(currentRay, out RaycastHit startHit) &&
                    startHit.transform.GetComponent<Controllable>() == _currentlySelectedCube)
                    _distanceToCube = (startHit.transform.position - transform.position).magnitude;
                break;
            }
            case TouchPhase.Moved:
            {
                _hasBeenMoved = true;

                if (_currentlySelectedCube)
                    _currentlySelectedCube.transform.position = currentRay.GetPoint(_distanceToCube);

                break;
            }
            case TouchPhase.Ended:
            {
                if (_touchTimer <= MaxTapTime && !_hasBeenMoved)
                {
                    if (Physics.Raycast(currentRay, out RaycastHit endHit))
                    {
                        Controllable controllable = endHit.transform.GetComponent<Controllable>();
                        SetCurrentlySelected(controllable);
                    }
                    else
                    {
                        SetCurrentlySelected(null);
                    }
                }

                break;
            }
        }
    }

    private void ProcessScaleAndRotate()
    {
        if (_currentlySelectedCube)
        {
            Touch firstTouch = Input.touches[0];
            Touch secondTouch = Input.touches[1];
            _yDistance = (secondTouch.position.y - firstTouch.position.y);
            _xDistance = (secondTouch.position.x - firstTouch.position.x);

            if (firstTouch.phase == TouchPhase.Began || secondTouch.phase == TouchPhase.Began)
            {
                _startAngle = Mathf.Atan2(_yDistance, _xDistance);
                _startRotation = _currentlySelectedCube.transform.rotation;
            }

            if (firstTouch.phase == TouchPhase.Moved || secondTouch.phase == TouchPhase.Moved)
            {
                float newAngle = Mathf.Atan2(_yDistance, _xDistance);
                float anglesDifference = Mathf.Rad2Deg * (newAngle - _startAngle);
                _currentlySelectedCube.transform.rotation = Quaternion.AngleAxis(anglesDifference, _rotationAxis) * _startRotation;
            }

            if (firstTouch.phase == TouchPhase.Ended || secondTouch.phase == TouchPhase.Ended)
            {
                _yDistance = 0.0f;
                _xDistance = 0.0f;
                _startAngle = 0.0f;
                _startRotation = Quaternion.identity;
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
        if (_currentlySelectedCube)
        {
            _currentlySelectedCube.ChangeColour(Color.white);
            _currentlySelectedCube = null;
        }

        if (controllable)
        {
            controllable.ChangeColour(Color.blue);
            _currentlySelectedCube = controllable;
        }
    }
}
