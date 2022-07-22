using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class BallCatapult : MonoBehaviour
{
    [SerializeField] private Ball _currentBall;
    [SerializeField] private float _maxPullBackDistance = 2f;
    [SerializeField] private float _nonShootPullBackDistance = 0.5f;

    [SerializeField] private Trajectory _mainTrajectory;

    private Camera _cameraMain;

    #region Cache Components
    private Rigidbody2D _catapultRb;
    public Rigidbody2D CatapultRb
    {
        get
        {
            if (_catapultRb == null)
                _catapultRb = GetComponent<Rigidbody2D>();
            return _catapultRb;
        }
    }
    #endregion

    private void Start()
    {
        _cameraMain = Camera.main;
    }

    private void OnMouseDrag()
    {
        PullBackBall();
        _mainTrajectory.ShowTrajectory(_currentBall, CalculateForceForBall());
    }

    private void OnMouseUp()
    {
        if (GetPullBackDistance() <= _nonShootPullBackDistance)
        {
            _currentBall.RbBall.MovePosition(CatapultRb.position);
            return;
        }

        _currentBall.BallFlying(CalculateForceForBall());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            _currentBall.RbBall.MovePosition(CatapultRb.position);
            _currentBall.IsBallCollide = false;
            _currentBall.StopAllCoroutines();
        }
            
    }

    private void PullBackBall()
    {
        Vector2 mousePosition = GetMousePosition();
        float currentPullBackDistance = GetPullBackDistance();

        if (currentPullBackDistance >= _maxPullBackDistance)
        {
            _currentBall.RbBall.MovePosition(CatapultRb.position + (mousePosition - CatapultRb.position).normalized * _maxPullBackDistance);

        }
        else
        {
            _currentBall.RbBall.MovePosition(mousePosition);
        }

        Debug.Log(Mathf.Round(currentPullBackDistance / _maxPullBackDistance * 100));
    }

    private float GetPullBackDistance()
    {
        float distance = Vector2.Distance(GetMousePosition(), CatapultRb.position);

        if (distance > _maxPullBackDistance)
            distance = _maxPullBackDistance;

        return distance;
    }

    private Vector2 GetMousePosition()
    {
        return _cameraMain.ScreenToWorldPoint(Input.mousePosition);
    }

    private Vector2 CalculateForceForBall()
    {
        return ((GetMousePosition() - CatapultRb.position).normalized * -1) * GetPullBackDistance();
    }
}
