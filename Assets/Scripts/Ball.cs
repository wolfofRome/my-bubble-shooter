using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Ball : MonoBehaviour
{
    private bool _isBallCollide = false;
    private Vector2 _ballOffset;

    private Rigidbody2D _rbBall;
    public Rigidbody2D RbBall
    {
        get
        {
            if (_rbBall == null)
                _rbBall = GetComponent<Rigidbody2D>();
            return _rbBall;
        }
    }

    private Camera _cameraMain;


    private void Start()
    {
        _cameraMain = Camera.main;
        _ballOffset = new Vector2(gameObject.transform.localScale.x/2, gameObject.transform.localScale.y);
    }

    public Coroutine BallFlying(Vector2 flyDirection)
    {
        return StartCoroutine(BallFlyingCoroutine(flyDirection));
    }

    public IEnumerator BallFlyingCoroutine(Vector2 flyDirection)
    {
        while (!_isBallCollide)
        {
            if (_cameraMain.WorldToViewportPoint(RbBall.position + _ballOffset).x >= _cameraMain.rect.max.x || 
                _cameraMain.WorldToViewportPoint(RbBall.position - _ballOffset).x <= _cameraMain.rect.min.x)
            {
                flyDirection.x *= -1;
            }
                
            RbBall.position += flyDirection * 0.5f;
            yield return new WaitForFixedUpdate();
        }

        yield break;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Ball>() != null)
        {
            _isBallCollide = true;
            Debug.Log(gameObject.name);
        }
        
    }
}