using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Trajectory))]
public class BallCatapult : MonoBehaviour, IPauseble
{
    [Header("General")]
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
        Application.targetFrameRate = 70;

        GameplayEvents.OnAllFieldActionsEnd.AddListener(UnlockCatapult);
        GameplayEvents.OnAllFieldActionsEnd.AddListener(ChangeActiveBall);
        GameplayEvents.OnCountBallsOnFieldChecked.AddListener(CheckAvailableBallsCount);

        UIEvents.OnClickPause.AddListener(PauseHandle);
        UIEvents.OnClickResume.AddListener(UnpauseHandle);
    }

    private void Start()
    {
        _availableBallsHolder = new GameObject("AvailableBallsHolder").transform;
        _availableBallsHolder.SetParent(transform);

        UnpauseHandle();

        if (LevelDataHolder.LevelData == null)
        {
            Debug.LogError("Level data is not set!");
            return;
        }

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

        _activeBall.CircleCollider.enabled = true;
        _activeBall.MoveBall(CalculateForceForBall());
        LockCatapult();
    }

    private void OnDestroy()
    {
        GameplayEvents.OnAllFieldActionsEnd.RemoveListener(UnlockCatapult);
        GameplayEvents.OnAllFieldActionsEnd.RemoveListener(ChangeActiveBall);
        GameplayEvents.OnCountBallsOnFieldChecked.RemoveListener(CheckAvailableBallsCount);

        UIEvents.OnClickPause.RemoveListener(PauseHandle);
        UIEvents.OnClickResume.RemoveListener(UnpauseHandle);
    }

    public void LockCatapult()
    {
        CatapultBoxCollider.enabled = false;
    }

    public void UnlockCatapult()
    {
        CatapultBoxCollider.enabled = true;
    }

    public void PauseHandle()
    {
        LockCatapult();
        Time.timeScale = 0f;
    }

    public void UnpauseHandle()
    {
        UnlockCatapult();
        Time.timeScale = 1f;
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
        if(!_availableBalls.TryDequeue(out _activeBall))
        {
            GameplayEvents.OnAvailableBallsEnd.Invoke();
            return;
        }
            
        GameplayEvents.OnAvailableBallsCountChanged.Invoke(_availableBalls.Count);

        _activeBall.gameObject.SetActive(true);
        _activeBall.transform.SetParent(transform);
        _activeBall.transform.position = transform.position;
        _activeBall.IsActiveBall = true;
        _activeBall.CircleCollider.enabled = false;

        if (_availableBalls.Count > 0)
            GameplayEvents.OnNextBallChanged.Invoke(_availableBalls.Peek());
    }

    private void CheckAvailableBallsCount()
    {
        if (_availableBalls.Count <= 0 && _activeBall == null)
            GameplayEvents.OnGameOver.Invoke();
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
