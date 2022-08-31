using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Trajectory))]
public class BallCatapult : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private LevelField _levelField;
    [SerializeField] private float _maxPullBackDistance = 2f;
    [SerializeField] private float _nonShootPullBackDistance = 0.5f;
    [Range(10f, 180f)]
    [SerializeField] private float _maxMoveBallAngle;

    private Transform _availableBallsHolder;
    private Ball _activeBall;
    private readonly Queue<Ball> _availableBalls = new Queue<Ball>();


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

    private Trajectory _trajectory;
    public Trajectory CatapultTrajectory
    {
        get
        {
            if (_trajectory == null)
                _trajectory = GetComponent<Trajectory>();
            return _trajectory;
        }
    }
    #endregion

    private void Awake()
    {
        if(_levelField == null)
        {
            Debug.LogError("Level field is not set!");
            return;
        }

        GameplayEvents.OnAllFieldActionsEnd.AddListener(UnlockCatapult);
        GameplayEvents.OnAllFieldActionsEnd.AddListener(ChangeActiveBall);
    }

    private void Start()
    {
        _availableBallsHolder = new GameObject("AvailableBallsHolder").transform;
        _availableBallsHolder.SetParent(transform);

        GenerateAvailableBalls(LevelDataHolder.LevelData.BallsTypeInLevel, LevelDataHolder.LevelData.CountAvailableBalls);
        ChangeActiveBall();
    }

    private void OnMouseDrag()
    {
        PullBackBall();

        if (GetPullBackDistance() >= _nonShootPullBackDistance)
            CatapultTrajectory.ShowTrajectory(_activeBall, CalculateForceForBall());
        else
            CatapultTrajectory.HideTrajectory(_activeBall);
    }

    private void OnMouseUp()
    {
        if (GetPullBackDistance() <= _nonShootPullBackDistance)
        {
            MoveBallAtCatapultPosition(_activeBall);
            CatapultTrajectory.HideTrajectory(_activeBall);
            return; 
        }

        _activeBall.MoveBall(CalculateForceForBall());
        LockCatapult();
    }

    private void OnDestroy()
    {
        GameplayEvents.OnAllFieldActionsEnd.RemoveListener(UnlockCatapult);
        GameplayEvents.OnAllFieldActionsEnd.RemoveListener(ChangeActiveBall);
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

    private void GenerateAvailableBalls(List<BallTypePrefab> ballsTypePrefabs, int countAvailableBalls)
    {
        for(int i = 0; i < countAvailableBalls; i++)
        {
            BallTypePrefab ballTypePrefab = GetRandomBallType(ballsTypePrefabs);
            Ball ball = Instantiate(ballTypePrefab.Prefab, _availableBallsHolder.transform);
            ball.TypeId = ballTypePrefab.Id;
            ball.RbBall.mass = 0f;
            ball.gameObject.SetActive(false);

            _availableBalls.Enqueue(ball);
        }
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

    
    private BallTypePrefab GetRandomBallType(List<BallTypePrefab> ballTypePrefabs)
    {
        return ballTypePrefabs[Random.Range(0, ballTypePrefabs.Count)];
    }
    
}
