using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Ball : MonoBehaviour
{
    private bool _isBallCollide = false;

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
    }

    public Coroutine BallFlying(Vector2 flyDirection)
    {
        return StartCoroutine(BallFlyingCoroutine(flyDirection));
    }

    public IEnumerator BallFlyingCoroutine(Vector2 flyDirection)
    {
        while (!_isBallCollide)
        {
            RbBall.position += flyDirection;
            yield return new WaitForFixedUpdate();
        }
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