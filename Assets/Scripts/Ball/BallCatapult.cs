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
    [SerializeField] private Transform _nextBallPosition;
    [SerializeField] private Trajectory _trajectory;

    private Ball _activeBall;
    private int _countAvailableBalls = 1;
    private Queue<Ball> _availableBalls = new Queue<Ball>();
    private List<BallTypePrefab> _activeBallPrefabs;

    public int CountAvailableBalls
    {
        set 
        {
            if (value == 0)
                return;

            _countAvailableBalls = value;
            GameplayEvents.OnCountAvailableBallsChanged.Invoke(_countAvailableBalls);
        }

        get
        {
            return _countAvailableBalls;
        }
    }

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
        _countAvailableBalls = LevelDataHolder.LevelData.CountAvailableBalls;

        GameplayEvents.OnActiveBallSetOnField.AddListener(_trajectory.HideTrajectory);
        GameplayEvents.OnActiveBallSetOnField.AddListener(ChangeActiveBall);

        GameplayEvents.OnActiveBallDestroyed.AddListener(_trajectory.HideTrajectory);
        GameplayEvents.OnActiveBallDestroyed.AddListener(ChangeActiveBall);

    }

    private void Start()
    {
        for(int i = 0; i<100; i++)
        {
            BallTypePrefab ballTypePrefab = GetRandomBallType();
            Ball ball = Instantiate(ballTypePrefab.Prefab, _nextBallPosition.position, Quaternion.identity, _nextBallPosition);
            ball.TypeId = ballTypePrefab.Id;
            ball.RbBall.mass = 0f;
            ball.gameObject.SetActive(false);

            _availableBalls.Enqueue(ball);
        }

        SetActiveBall();
        SetNextBall();
    }

    private void OnMouseDrag()
    {
        PullBackBall();
        _trajectory.ShowTrajectory(_activeBall, CalculateForceForBall());
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
    }

    private void OnDestroy()
    {
        GameplayEvents.OnActiveBallSetOnField.RemoveListener(ChangeActiveBall);
        GameplayEvents.OnActiveBallSetOnField.RemoveListener(_trajectory.HideTrajectory);

        GameplayEvents.OnActiveBallDestroyed.RemoveListener(ChangeActiveBall);
        GameplayEvents.OnActiveBallDestroyed.RemoveListener(_trajectory.HideTrajectory);
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

    private void ChangeActiveBall(Ball activeBall)
    {
        activeBall.IsActiveBall = false;

        SetActiveBall();
        CountAvailableBalls -= 1;

        SetNextBall();
    }

    private void SetActiveBall()
    {
        _activeBall = _availableBalls.Dequeue();
        _activeBall.transform.SetParent(transform);

        _activeBall.transform.position = transform.position;

        _activeBall.IsActiveBall = true;
        _activeBall.gameObject.SetActive(true);
    }

    private void SetNextBall()
    {
        if (_countAvailableBalls == 0)
            return;
        
        _availableBalls.Peek().gameObject.SetActive(true);
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
