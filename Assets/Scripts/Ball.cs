using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Ball : MonoBehaviour
{
    private bool _isBallCollide = false;
    private Vector2 _ballPositionOffset;
    private Camera _cameraMain;

    public bool IsBallCollide
    {
        set { _isBallCollide = value; }
        get { return _isBallCollide; }
    }

    public Vector2 BallPositionOffset
    {
        set { _ballPositionOffset = value; }
        get { return _ballPositionOffset; }
    }

    #region Cache Components
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
    #endregion

    private void Start()
    {
        _cameraMain = Camera.main;
        _ballPositionOffset = new Vector2(gameObject.transform.localScale.x/2, gameObject.transform.localScale.y);
    }

    public Coroutine BallFlying(Vector2 force)
    {
        return StartCoroutine(BallFlyingCoroutine(force));
    }

    public IEnumerator BallFlyingCoroutine(Vector2 force)
    {
        float time = 0;
        while (!_isBallCollide)
        {
            if (_cameraMain.WorldToViewportPoint(RbBall.position + _ballPositionOffset).x >= _cameraMain.rect.max.x ||
                _cameraMain.WorldToViewportPoint(RbBall.position - _ballPositionOffset).x <= _cameraMain.rect.min.x )
            {
                force.x *= -1;
            }

            if(_cameraMain.WorldToViewportPoint(RbBall.position + _ballPositionOffset).y >= _cameraMain.rect.max.y)
            {
                force.y *= -1;
            }

            RbBall.position += force + Mathf.Pow(time, 2) * Physics2D.gravity / 2;

            yield return new WaitForFixedUpdate();
            time += Time.fixedDeltaTime;
        }

        yield break;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Ball>() != null)
        {
            if(_isBallCollide != true)
                _isBallCollide = true;
        } 
    }
}