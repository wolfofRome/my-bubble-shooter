using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class BallCatapult : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private float _maxPullBackDistance = 2f;
    [SerializeField] private float _nonShootPullBackDistance = 0.5f;
    [Range(10f, 180f)]
    [SerializeField] private float _maxMoveBallAngle;
    [SerializeField] private Transform _nextBallHolder;
    [SerializeField] private Trajectory _trajectory;

    private Ball _activeBall;
    private Queue<Ball> _availableBalls = new Queue<Ball>();
    private List<BallTypePrefab> _activeBallPrefabs;

    #region Cache Components
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

    private BoxCollider2D _catapultBoxCollider;
    public BoxCollider2D CatapultBoxCollider
    {
        get
        {
            if (_catapultBoxCollider == null)
                _catapultBoxCollider = GetComponent<BoxCollider2D>();
            return _catapultBoxCollider;
        }
    }

    private CameraSettings _cameraSettings;
    private CameraSettings CameraSettings
    {
        get
        {
            if (_cameraSettings == null)
                _cameraSettings = Camera.main.GetComponent<CameraSettings>();
            return _cameraSettings;
        }
    }
    #endregion

    private void Awake()
    {
        if(_trajectory == null)
        {
            Debug.LogError("Trajectory is not set!");
            return;
        }

        _activeBallPrefabs = LevelDataHolder.LevelData.BallsTypeInLevel;

        GameplayEvents.OnAllGameActionsEnd.AddListener(UnlockCatapult);
        GameplayEvents.OnAllGameActionsEnd.AddListener(ChangeActiveBall);
    }

    private void Start()
    {
        for(int i = 0; i < LevelDataHolder.LevelData.CountAvailableBalls; i++)
        {
            BallTypePrefab ballTypePrefab = GetRandomBallType();
            Ball ball = Instantiate(ballTypePrefab.Prefab, _nextBallHolder.position, Quaternion.identity, _nextBallHolder);
            ball.TypeId = ballTypePrefab.Id;
            ball.RbBall.mass = 0f;
            ball.gameObject.SetActive(false);

            _availableBalls.Enqueue(ball);
        }

        ChangeActiveBall();
    }

    private void OnMouseDrag()
    {
        PullBackBall();

        if (GetPullBackDistance() >= _nonShootPullBackDistance)
            _trajectory.ShowTrajectory(_activeBall, CalculateForceForBall());
        else
            _trajectory.HideTrajectory(_activeBall);
    }

    private void OnMouseUp()
    {
        if (GetPullBackDistance() <= _nonShootPullBackDistance)
        {
            MoveBallAtCatapultPosition(_activeBall);
            _trajectory.HideTrajectory(_activeBall);
            return; 
        }

        _activeBall.MoveBall(CalculateForceForBall());
        LockCatapult();
    }

    private void OnDestroy()
    {
        GameplayEvents.OnAllGameActionsEnd.RemoveListener(UnlockCatapult);
        GameplayEvents.OnAllGameActionsEnd.RemoveListener(ChangeActiveBall);
    }

    public void LockCatapult()
    {
        CatapultBoxCollider.enabled = false;
    }

    public void UnlockCatapult()
    {
        CatapultBoxCollider.enabled = true;
    }

    private void PullBackBall()
    {
        Vector2 mouseCatapultDistance = GetMousePosition() - CatapultRb.position;
        Vector2 direction = mouseCatapultDistance.normalized;

        if(Vector2.SignedAngle(CatapultRb.position, mouseCatapultDistance) >= _maxMoveBallAngle)
        {
            direction = Quaternion.Euler(0, 0, _maxMoveBallAngle) * Vector2.down;
        }

        if(Vector2.SignedAngle(CatapultRb.position, mouseCatapultDistance) <= _maxMoveBallAngle * -1)
        {
            direction = Quaternion.Euler(0, 0, _maxMoveBallAngle * -1) * Vector2.down;
        }
        
        _activeBall.RbBall.MovePosition(CatapultRb.position + direction * GetPullBackDistance());
        
        //Debug.Log(CalculateForceForBall());
        //Debug.Log(Mathf.Round(currentPullBackDistance / _maxPullBackDistance * 100));
    }

    private void MoveBallAtCatapultPosition(Ball ball)
    {
        ball.RbBall.position = transform.position;
    }

    private void ChangeActiveBall()
    {
        SetActiveBall();
        ShowNextBall();
    }

    private void SetActiveBall()
    {
        if(!_availableBalls.TryDequeue(out _activeBall))
        {
            GameplayEvents.OnAvailableBallsEnd.Invoke();
            return;
        }


        _activeBall.gameObject.SetActive(true);
        _activeBall.transform.SetParent(transform);
        _activeBall.transform.position = transform.position;
        _activeBall.IsActiveBall = true;
    }

    private void ShowNextBall()
    {
        if (_availableBalls.Count == 0)
            return;

        //_nextBall.NextBallImage.sprite = _availableBalls.Peek().BallSpriteRenderer.sprite;
        //_nextBall.NextBallImage.color = _availableBalls.Peek().BallSpriteRenderer.color;
    }

    private float GetPullBackDistance()
    {
        float distance = Vector2.Distance(GetMousePosition(), CatapultRb.position);

        if (distance > _maxPullBackDistance)
            distance = _maxPullBackDistance;

        return distance;
    }

    private Vector2 GetMousePosition()
    {
        return CameraSettings.Camera.ScreenToWorldPoint(Input.mousePosition);
    }

    private Vector2 CalculateForceForBall()
    {
        return ((_activeBall.RbBall.position - CatapultRb.position).normalized * -1) * GetPullBackDistance();
    }

    private BallTypePrefab GetRandomBallType()
    {
        return _activeBallPrefabs[Random.Range(0, _activeBallPrefabs.Count)];
    }
}
