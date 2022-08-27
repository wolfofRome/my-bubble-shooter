using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Van.HexGrid;
using System.Linq;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class Ball : MonoBehaviour
{
    private bool _isBallCollided;
    private bool _isActiveBall;
    private bool _isFirstLineBall = false;
    private string _typeId;
    private WallSettings _wallSettingsCollision;
    
    public bool IsBallCollided { get => _isBallCollided; set => _isBallCollided = value; }
    public bool IsActiveBall { get => _isActiveBall; set => _isActiveBall = value; }
    public bool IsFirstLineBall { get => _isFirstLineBall; set => _isFirstLineBall = value; }
    public string TypeId { get => _typeId; set => _typeId = value; }
    public float BallRadius
    {
        get
        {
            return transform.localScale.x * CircleCollider.radius;
        }
    }
    public WallSettings WallSettingsCollision
    {
        get
        {
            if (_wallSettingsCollision == null)
                Debug.LogError("Wall settings collison is not set!");
            return _wallSettingsCollision;
        }

        set
        {
            if (value == null)
            {
                Debug.LogError("Wall settings collision is equal null!");
                return;
            }

            _wallSettingsCollision = value;
        }
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

    private CameraSettings _cameraSettings;
    public CameraSettings CameraSettings
    {
        get
        {
            if (_cameraSettings == null)
                _cameraSettings = Camera.main.GetComponent<CameraSettings>();
            return _cameraSettings;
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

    private FixedJoint2D _jointConnection;
    public FixedJoint2D JointConnection
    {
        get
        {
            if (_jointConnection == null)
                Debug.LogWarning("Joint connection is not set!");

            return _jointConnection;
        }
    }
    #endregion

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Ball collidedBall;
        if (collision.gameObject.TryGetComponent<Ball>(out collidedBall) && _isActiveBall)
        {
            _isBallCollided = true;
            GameplayEvents.OnActiveBallCollided.Invoke(this);
        }
    }

    public Coroutine MoveBall(Vector2 force)
    {
        return StartCoroutine(MoveBallCoroutine(force));
    }

    public IEnumerator MoveBallCoroutine(Vector2 force)
    {
        _isBallCollided = false;

        float time = 0;
        while (!_isBallCollided)
        {   
            if (_wallSettingsCollision != null)
            {
                force = Vector2.Reflect(force, _wallSettingsCollision.WallNormal);
                _wallSettingsCollision = null;
            }

            RbBall.MovePosition(RbBall.position + force + Mathf.Pow(time, 2) * Physics2D.gravity / 2);

            yield return new WaitForFixedUpdate();
            time += Time.fixedDeltaTime;
        }

        yield break;
    }

    public void AddJointConnection(Vector2 connectedAnchorPosition)
    {
        _jointConnection = gameObject.AddComponent<FixedJoint2D>();
        _jointConnection.autoConfigureConnectedAnchor = false;
        _jointConnection.connectedAnchor = connectedAnchorPosition;
    }

    public void DestroyBall()
    {
        if (IsActiveBall)
            GameplayEvents.OnActiveBallDestroyed.Invoke(this);
            
        gameObject.SetActive(false);
    }
}