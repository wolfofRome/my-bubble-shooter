using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Ball : MonoBehaviour
{
    public bool _isBallCollide = false;
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

    public Coroutine BallFlying(Vector2 flyDirection, float pullBackDistance)
    {
        return StartCoroutine(BallFlyingCoroutine(flyDirection, pullBackDistance));
    }

    public IEnumerator BallFlyingCoroutine(Vector2 direction, float pullBackDistance)
    {
        float time = 0;
        Vector2 force;
        while (!_isBallCollide)
        {
            if (_cameraMain.WorldToViewportPoint(RbBall.position + _ballOffset).x >= _cameraMain.rect.max.x ||
                _cameraMain.WorldToViewportPoint(RbBall.position - _ballOffset).x <= _cameraMain.rect.min.x )
            {
                direction *= -1;
            }

            force = direction * pullBackDistance;
            RbBall.MovePosition(RbBall.position + force + Mathf.Pow(time, 2) * Physics2D.gravity / 2);

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