using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Van.HexGrid;
using System.Linq;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class Ball : MonoBehaviour
{
    private bool _isBallCollide;
    private bool _isActiveBall;
    private string _typeId;
    private WallSettings _wallSettingsCollision;

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
    #endregion

    public Coroutine MoveBall(Vector2 force)
    {
        return StartCoroutine(MoveBallCoroutine(force));
    }

    public IEnumerator MoveBallCoroutine(Vector2 force)
    {
        _isBallCollide = false;

        float time = 0;
        while (!_isBallCollide)
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
        FixedJoint2D newJointConnection = gameObject.AddComponent<FixedJoint2D>();
        newJointConnection.autoConfigureConnectedAnchor = false;
        newJointConnection.connectedAnchor = connectedAnchorPosition;
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

        if(collision.rigidbody.GetComponent<WallSettings>() != null && _isActiveBall)
        {
            _wallSettingsCollision = collision.rigidbody.GetComponent<WallSettings>();
            return;
        }
    }
}