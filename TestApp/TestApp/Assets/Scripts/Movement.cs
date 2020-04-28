using System;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private Camera _myCamera;
    private Controllable _currentlySelectedCube;
    private Quaternion _startRotation;
    private Vector3 _rotationAxis;
    private Vector3 _startingRayDirection;
    private Vector2 _startPosition;
    private float _distanceToCube;
    private float _startAngle;
    private float _yDistance;
    private float _xDistance;
    private float _totalDistance;
    private float _touchTimer;
    private float _fov;
    private float _zAngle;
    float _firstSecondDistance;
    float _firstThirdDistance;
    float _secondThirdDistance;
    private int _currentTouchCount;
    private bool _hasBeenMoved;
    private bool _isRotating;
    private Ray _currentRay;
    private const float MaxTapTime = 0.12f;
    private const float MinRotateDistance = 10.0f;
    private const float MinScaleDistance = 1.0f;
    private const float ScaleFactor = 1.03f;
    private const float MinFov = 15f;
    private const float MaxFov = 90f;
    private const float Sensitivity = 1f;
    private const float CameraAngleDistance = 10.0f; // Distance from camera for Ray points
    private const float DegreesInACircle = 360.0f;

    private void Start()
    {
        _myCamera = Camera.main;
        _touchTimer = 0.0f;
        _distanceToCube = 0.0f;
        if (_myCamera != null)
        {
            _rotationAxis = _myCamera.transform.forward;
            _fov = _myCamera.fieldOfView;
            _zAngle = _myCamera.transform.rotation.z;
        }
        _startAngle = 0.0f;
        _startRotation = Quaternion.identity;
        _yDistance = 0.0f;
        _xDistance = 0.0f;
        _totalDistance = 0.0f;
        _isRotating = false;
        _startPosition = new Vector2();
        _startingRayDirection = new Vector3();
        _currentTouchCount = 0;
        _firstSecondDistance = 0;
        _firstThirdDistance = 0;
        _secondThirdDistance = 0;
    }

    private void Update()
    {
        _rotationAxis = _myCamera.transform.forward;
        _fov = _myCamera.fieldOfView;
        
        if (Input.touchCount == 1 && (_currentTouchCount == 0 || _currentTouchCount == 1))
        {
            OneTouchMovement();
        }
        
        else if (Input.touchCount == 2 && (_currentTouchCount == 0 || _currentTouchCount == 2))
        {
            Touch firstTouch = Input.touches[0];
            Touch secondTouch = Input.touches[1];
            TwoTouchMovement(firstTouch, secondTouch);
        }

        else if (Input.touchCount == 3 && (_currentTouchCount == 0 || _currentTouchCount == 3))
        {
            //Move Camera forward/backward
            Touch firstTouch = Input.touches[0];
            Touch secondTouch = Input.touches[1];
            Touch thirdTouch = Input.touches[2];
            ThreeTouchMovement(firstTouch, secondTouch, thirdTouch);
        }
        _touchTimer += Time.deltaTime;
        _myCamera.fieldOfView = _fov;
    }

    private void OneTouchMovement()
    {
        Touch touch = Input.touches[0];
        _currentRay = _myCamera.ScreenPointToRay(touch.position);
        switch (touch.phase)
        {
            case TouchPhase.Began:
            {
                _currentTouchCount = 1;
                _touchTimer = 0.0f;
                _hasBeenMoved = false;

                if (Physics.Raycast(_currentRay, out RaycastHit startHit) &&
                    startHit.transform.GetComponent<Controllable>() == _currentlySelectedCube)
                    _distanceToCube = (startHit.transform.position - transform.position).magnitude;
                else
                {
                    _startPosition = touch.position;
                }
                break;
            }
            case TouchPhase.Moved:
            {
                _hasBeenMoved = true;

                if (_currentlySelectedCube)
                    _currentlySelectedCube.transform.position = _currentRay.GetPoint(_distanceToCube);
                else
                {
                    Vector2 newPos = (touch.position - (Vector2)_startPosition) * 0.05f;
                    Transform cameraTransform = _myCamera.transform;
                    cameraTransform.position = new Vector3(newPos.x, newPos.y, cameraTransform.position.z);
                }

                break;
            }
            case TouchPhase.Ended:
            {
                if (_touchTimer <= MaxTapTime && !_hasBeenMoved)
                {
                    if (Physics.Raycast(_currentRay, out RaycastHit endHit))
                    {
                        Controllable controllable = endHit.transform.GetComponent<Controllable>();
                        SetCurrentlySelected(controllable);
                    }
                    else
                    {
                        SetCurrentlySelected(null);
                    }
                }

                _currentTouchCount = 0;
                break;
            }
        }
    }

    private void TwoTouchMovement(Touch firstTouch, Touch secondTouch)
    {
        _yDistance = (secondTouch.position.y - firstTouch.position.y);
        _xDistance = (secondTouch.position.x - firstTouch.position.x);

        if (firstTouch.phase == TouchPhase.Began || secondTouch.phase == TouchPhase.Began)
        {
            _currentTouchCount = 2;
            SetStartingProps(firstTouch, secondTouch);
        }

        if (firstTouch.phase == TouchPhase.Moved || secondTouch.phase == TouchPhase.Moved)
        {
            if (_currentlySelectedCube)
            {
                ProcessObjectScaleAndRotate(firstTouch, secondTouch);
            }
            else
            {
                ProcessCameraRotateAndZoom(firstTouch, secondTouch);
            }
        }

        if (firstTouch.phase == TouchPhase.Ended || secondTouch.phase == TouchPhase.Ended)
        {
            ResetRotateScaleProps();
        }
    }

    private void ThreeTouchMovement(Touch firstTouch, Touch secondTouch, Touch thirdTouch)
    {
        if (firstTouch.phase == TouchPhase.Began || secondTouch.phase == TouchPhase.Began || thirdTouch.phase == TouchPhase.Began)
        {
            _currentTouchCount = 3;
            _firstSecondDistance = Vector2.Distance(firstTouch.position, secondTouch.position);
            _firstThirdDistance = Vector2.Distance(firstTouch.position, thirdTouch.position);
            _secondThirdDistance = Vector2.Distance(secondTouch.position, thirdTouch.position);
        }

        if (firstTouch.phase == TouchPhase.Moved || secondTouch.phase == TouchPhase.Moved || thirdTouch.phase == TouchPhase.Moved)
        {
            float newFirstSecondDistance = Vector2.Distance(firstTouch.position, secondTouch.position);
            float newFirstThirdDistance = Vector2.Distance(firstTouch.position, thirdTouch.position);
            float newSecondThirdDistance = Vector2.Distance(secondTouch.position, thirdTouch.position);
            
            if ((newFirstSecondDistance - _firstSecondDistance) > MinScaleDistance 
                || (newFirstThirdDistance - _firstThirdDistance) > MinScaleDistance 
                || (newSecondThirdDistance - _secondThirdDistance) > MinScaleDistance)
            {
                _myCamera.transform.position += ScaleFactor * _myCamera.transform.forward;
            }
            
            else if ((newFirstSecondDistance - _firstSecondDistance) < -MinScaleDistance 
                     || (newFirstThirdDistance - _firstThirdDistance) < -MinScaleDistance 
                     || (newSecondThirdDistance - _secondThirdDistance) < -MinScaleDistance)
            {
                _myCamera.transform.position -= ScaleFactor * _myCamera.transform.forward;
            }
        }

        if (firstTouch.phase == TouchPhase.Ended || secondTouch.phase == TouchPhase.Ended || thirdTouch.phase == TouchPhase.Ended)
        {
            _firstSecondDistance = 0;
            _firstThirdDistance = 0;
            _secondThirdDistance = 0;
            _currentTouchCount = 0;
        }
    }

    private void ProcessObjectScaleAndRotate(Touch firstTouch, Touch secondTouch)
    {
        float anglesDifference = Mathf.Rad2Deg * (Mathf.Atan2(_yDistance, _xDistance) - _startAngle);
        if (anglesDifference >= MinRotateDistance || anglesDifference <= -MinRotateDistance)
        {
            _currentlySelectedCube.transform.rotation = Quaternion.AngleAxis(anglesDifference, _rotationAxis) * _startRotation;
            _isRotating = true;
        }
        else if (!_isRotating)
        {
            float distanceDifference = Vector2.Distance(firstTouch.position, secondTouch.position) - _totalDistance;
            _totalDistance = Vector2.Distance(firstTouch.position, secondTouch.position);
            if(distanceDifference > MinScaleDistance)
                _currentlySelectedCube.transform.localScale *= ScaleFactor;
            else if (distanceDifference < -MinScaleDistance)
                _currentlySelectedCube.transform.localScale *= (1/ScaleFactor);
        }
    }
    
    private void ProcessCameraRotateAndZoom(Touch firstTouch, Touch secondTouch)
    {
        float anglesDifference = Mathf.Rad2Deg * (Mathf.Atan2(_yDistance, _xDistance) - _startAngle);
        //rotating using two finger circular motion
        if (anglesDifference >= MinRotateDistance || anglesDifference <= -MinRotateDistance)
        {
            _myCamera.transform.rotation = Quaternion.AngleAxis(anglesDifference, _rotationAxis) * _startRotation;
            _zAngle = _myCamera.transform.rotation.z;
            _isRotating = true;
        }
        else if (!_isRotating)
        {
            float distanceDifference = Vector2.Distance(firstTouch.position, secondTouch.position) - _totalDistance;
            _totalDistance = Vector2.Distance(firstTouch.position, secondTouch.position);
            if (distanceDifference > MinScaleDistance || distanceDifference < -MinScaleDistance)
            {
                // found code here: https://kylewbanks.com/blog/unity3d-panning-and-pinch-to-zoom-camera-with-touch-and-mouse-input
                _fov = Mathf.Clamp(_fov - (distanceDifference * Sensitivity), MinFov, MaxFov);
            }
            else
            {
                // Had to give up on this for time reasons, but so far rotating right seems to work
                Vector3 currentRayDirection = GetDirectionForAngleCalculation(firstTouch, secondTouch);
                float rotationAngle = Mathf.Rad2Deg*(Mathf.Acos(Vector3.Dot(_startingRayDirection, currentRayDirection)));
                _rotationAxis = IsXRotating(firstTouch, secondTouch) ? _myCamera.transform.up : _myCamera.transform.right;
                _myCamera.transform.rotation = Quaternion.AngleAxis(rotationAngle, _rotationAxis) * _startRotation;
            }
        }
    }

    private bool IsXRotating(Touch firstTouch, Touch secondTouch)
    {
        //if the distance between the starting y position and the ending y position is greater than those of the x axis then false
        return true;
    }

    private Vector3 GetDirectionForAngleCalculation(Touch firstTouch, Touch secondTouch)
    {
        Vector2 vector2Pos = (firstTouch.position + secondTouch.position) / 2.0f;
        return _myCamera.ScreenPointToRay(new Vector3(vector2Pos.x, vector2Pos.y, _zAngle)).direction;
    }

    private void SetStartingProps(Touch firstTouch, Touch secondTouch)
    {
        _totalDistance = Vector2.Distance(firstTouch.position, secondTouch.position);
        _startAngle = Mathf.Atan2(_yDistance, _xDistance);
        _startRotation = _currentlySelectedCube ? _currentlySelectedCube.transform.rotation : _myCamera.transform.rotation;
        _startingRayDirection = GetDirectionForAngleCalculation(firstTouch, secondTouch);
    }

    private void ResetRotateScaleProps()
    {
        _yDistance = 0.0f;
        _xDistance = 0.0f;
        _startAngle = 0.0f;
        _startRotation = Quaternion.identity;
        _isRotating = false;
        _currentTouchCount = 0;
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
