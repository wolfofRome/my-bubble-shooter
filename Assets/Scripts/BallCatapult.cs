using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class BallCatapult : MonoBehaviour
{ 
    [SerializeField] private float _maxPullBackDistance = 2f;
    [SerializeField] private float _nonShootPullBackDistance = 0.5f;

    [SerializeField] private Transform _nextBallPosition;
    [SerializeField] private Trajectory _mainTrajectory;

    private int _countAvailableBalls = 1;
    private Ball _nextBall;
    private Ball _activeBall;

    private Camera _cameraMain;
    private List<BallPrefabType> _activeBallPrefabs;

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
    #endregion

    private void Awake()
    {
        _cameraMain = Camera.main;

        _activeBallPrefabs = LevelDataHolder.LevelData.BallsTypeInLevel;
        _countAvailableBalls = LevelDataHolder.LevelData.CountAvailableBalls;

        GameplayEvents.OnActiveBallSetOnField.AddListener(ChangeActiveBall);
    }

    private void Start()
    {
        SetNextBall();
        _activeBall = _nextBall;
        _activeBall.IsActiveBall = true;
        _activeBall.transform.position = transform.position;
        SetNextBall();
    }

    private void OnMouseDrag()
    {
        PullBackBall();
        _mainTrajectory.ShowTrajectory(_activeBall, CalculateForceForBall());
    }

    private void OnMouseUp()
    {
        if (GetPullBackDistance() <= _nonShootPullBackDistance)
        {
            SetBallAtCatapultPosition(_activeBall);
            return;
        }

        _activeBall.MoveBall(CalculateForceForBall());
    }

    private void OnDestroy()
    {
        GameplayEvents.OnActiveBallSetOnField.RemoveListener(ChangeActiveBall);
    }

    private void PullBackBall()
    {
        Vector2 mousePosition = GetMousePosition();
        float currentPullBackDistance = GetPullBackDistance();

        if (currentPullBackDistance >= _maxPullBackDistance)
        {
            _activeBall.RbBall.MovePosition(CatapultRb.position + (mousePosition - CatapultRb.position).normalized * _maxPullBackDistance);

        }
        else
        {
            _activeBall.RbBall.MovePosition(mousePosition);
        }

        //Debug.Log(CalculateForceForBall());
        //Debug.Log(Mathf.Round(currentPullBackDistance / _maxPullBackDistance * 100));
    }

    private void SetNextBall()
    {
        if(_countAvailableBalls == 0)
        {
            return;
        }

        BallPrefabType prefabType = GetRandomBallType();

        Ball nextBall = Instantiate(prefabType.Prefab, _nextBallPosition.position, Quaternion.identity);
        nextBall.TypeId = prefabType.Id;
        nextBall.transform.SetParent(_nextBallPosition);
        nextBall.RbBall.mass = 0.05f;
        _nextBall = nextBall;
    }

    private void SetBallAtCatapultPosition(Ball ball)
    {
        ball.RbBall.position = transform.position;
    }

    private void ChangeActiveBall(Ball activeBall)
    {
        activeBall.IsActiveBall = false;
        _activeBall = _nextBall;
        _activeBall.IsActiveBall = true;
        _activeBall.transform.position = transform.position;

        CountAvailableBalls -= 1;

        SetNextBall();
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
        return _cameraMain.ScreenToWorldPoint(Input.mousePosition);
    }

    private Vector2 CalculateForceForBall()
    {
        return ((GetMousePosition() - CatapultRb.position).normalized * -1) * GetPullBackDistance();
    }

    private BallPrefabType GetRandomBallType()
    {
        return _activeBallPrefabs[Random.Range(0, _activeBallPrefabs.Count)];
    }
}
