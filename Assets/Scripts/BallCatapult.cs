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


    private void Update()
    {

    }

    private void OnMouseDrag()
    {
        PullBackBall();
    }

    private void OnMouseUp()
    {
        
    }

    private void PullBackBall()
    {
        Vector2 mousePosition = GetMousePosition();

        if (GetPullBackDistanse() > _maxPullBackDistance)
            _currentBall.RbBall.position = CatapultRb.position + (mousePosition - CatapultRb.position).normalized * _maxPullBackDistance;
        else
            _currentBall.RbBall.position = mousePosition;
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
