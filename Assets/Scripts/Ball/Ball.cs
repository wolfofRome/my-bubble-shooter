using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Van.HexGrid;
using System.Linq;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class Ball : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private FixedJoint2D _jointConnetctionPrefab;


    [Header("Active")]
    [SerializeField] private bool _isActiveBall;

    private Collision2D _wallCollision;
    private bool _isBallCollide = false;
    private string _typeId;

    public bool IsBallCollide
    {
        set { _isBallCollide = value; }
        get { return _isBallCollide; }
    }

    public float BallRadius
    {
        get { return transform.localScale.x / 2; }
    }

    public bool IsActiveBall { get => _isActiveBall; set => _isActiveBall = value; }

    public string TypeId { get => _typeId; set => _typeId = value; }

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

    public Coroutine MoveBall(Vector2 force)
    {
        return StartCoroutine(MoveBallCoroutine(force));
    }

    public IEnumerator MoveBallCoroutine(Vector2 force)
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

    public void AddJointConnection(Ball connectedBall)
    {
        if (_jointConnetctionPrefab == null)
        {
            Debug.LogWarning("Joint Connection Prefab is not set!");
            return;
        }

        FixedJoint2D newJointConnection = gameObject.AddComponent<FixedJoint2D>();
        newJointConnection.connectedBody = connectedBall.RbBall;
        newJointConnection.autoConfigureConnectedAnchor = false;
        newJointConnection.frequency = 0;
    }

    private void Awake()
    {
        GameplayEvents.OnBallGroupDestroyed.AddListener(RemoveEmptyJointConnections);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.rigidbody.GetComponent<Ball>() != null && _isActiveBall)
        {
            if (_isBallCollide != true)
                _isBallCollide = true;
 
            GameplayEvents.OnActiveBallCollided.Invoke(this);
            return;
        }

        _wallCollision = collision;
    }

    private void OnDestroy()
    {
        GameplayEvents.OnBallGroupDestroyed.RemoveListener(RemoveEmptyJointConnections);
    }

    private void RemoveEmptyJointConnections(List<Ball> balls)
    {
        
    }
}