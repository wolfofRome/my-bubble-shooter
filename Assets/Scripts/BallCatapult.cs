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

    private Camera _cameraMain;

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

    private void Start()
    {
        _cameraMain = Camera.main;
    }

    private void OnMouseDrag()
    {
        PullBackBall();
    }

    private void OnMouseUp()
    {
        if (GetPullBackDistanse() <= _nonShootPullBackDistance)
        {
            _currentBall.RbBall.MovePosition(CatapultRb.position);
            return;
        }

        Vector2 flyDirection = (GetMousePosition() - CatapultRb.position).normalized * -1;
        _currentBall.BallFlying(flyDirection);
    }

    private void PullBackBall()
    {

        
        Vector2 mousePosition = GetMousePosition();

        if (GetPullBackDistanse() > _maxPullBackDistance)
        {
            _currentBall.RbBall.MovePosition(CatapultRb.position + (mousePosition - CatapultRb.position).normalized * _maxPullBackDistance);

        }
        else
        {
            _currentBall.RbBall.MovePosition(mousePosition);
        }
            
    }

    private void ShootBall()
    {
        
    }

    private float GetPullBackDistanse()
    {
        return  Vector2.Distance(GetMousePosition(), CatapultRb.position);
    }

    private Vector2 GetMousePosition()
    {
        return _cameraMain.ScreenToWorldPoint(Input.mousePosition);
    }
}
