using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Van.HexGrid;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class Ball : MonoBehaviour
{
    public HexGrid grid;
    public GameObject test;

    private bool _isBallCollide = false;
    private Collision2D _wallCollision;

    public bool IsBallCollide
    {
        set { _isBallCollide = value; }
        get { return _isBallCollide; }
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

    private CircleCollider2D _circleCollider;
    public CircleCollider2D CircleCollider
    {
        get
        {
            if (_circleCollider == null)
                _circleCollider = GetComponent<CircleCollider2D>();
            return _circleCollider;
        }
    }
    #endregion

    public Coroutine BallFlying(Vector2 force)
    {
        return StartCoroutine(BallFlyingCoroutine(force));
    }

    public IEnumerator BallFlyingCoroutine(Vector2 force)
    {
        float time = 0;
        while (!_isBallCollide)
        {
            if (_wallCollision != null)
            {
                force = Vector2.Reflect(force, _wallCollision.contacts[0].normal);
                _wallCollision = null;
            }

            RbBall.MovePosition(RbBall.position + force + Mathf.Pow(time, 2) * Physics2D.gravity / 2);

            yield return new WaitForFixedUpdate();
            time += Time.fixedDeltaTime;
        }

        yield break;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.rigidbody.GetComponent<Ball>() != null)
        {
            if (_isBallCollide != true)
                _isBallCollide = true;

            RbBall.constraints = RigidbodyConstraints2D.FreezeAll;
            HexCell cell = grid.GetCellFromPosition(RbBall.position);
            RbBall.position = cell.transform.position;
            return;
        }

        _wallCollision = collision;
    }
}