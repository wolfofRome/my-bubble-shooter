using System.Collections.Generic;
using UnityEngine;
using Van.HexGrid;

public class WallSettings : MonoBehaviour
{
    [SerializeField] private WallPosition _position;
    [SerializeField] private WallFunction _wallFunction;
    [SerializeField] private bool _destroyOnExit;

    private Vector2 _wallNormal;
    private readonly List<Ball> _dropedBalls = new List<Ball>();

    public Vector2 WallNormal { get => _wallNormal; }

    public WallFunction WallFunctionValue { get => _wallFunction; }

    private void Awake()
    {
        switch (_position)
        {
            case WallPosition.Left:
                _wallNormal = Vector2.right;
                break;

            case WallPosition.Right:
                _wallNormal = Vector2.left;
                break;

            case WallPosition.Down:
                _wallNormal = Vector2.up;
                break;

            case WallPosition.Top:
                _wallNormal = Vector2.down;
                break;
        }

        GameplayEvents.OnBallsDropStarted.AddListener(SetDropedBallsCells);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Ball collisionBall;
        if (!collision.TryGetComponent<Ball>(out collisionBall))
            return;

        if (WallFunctionValue == WallFunction.Destroy)
        {
            collisionBall.DestroyBall();

            if (collisionBall.IsActiveBall)
            {
                GameplayEvents.OnAllFieldActionsEnd.Invoke();
            }
            else
            {
                if (_dropedBalls.Count >= 0 && _dropedBalls.Contains(collisionBall))
                    _dropedBalls.Remove(collisionBall);

                if(_dropedBalls.Count <= 0)
                {
                    GameplayEvents.OnBallsDropFinished.Invoke();
                    GameplayEvents.OnAllFieldActionsEnd.Invoke();
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Ball collisionBall;
        if (!collision.gameObject.TryGetComponent<Ball>(out collisionBall))
            return;

        if (WallFunctionValue == WallFunction.Reflect)
        {
            if (collisionBall.IsActiveBall)
                collisionBall.WallSettingsCollision = this;
        }
        else if(WallFunctionValue == WallFunction.AttachToField)
        {
            if (collisionBall.IsActiveBall)
            {
                collisionBall.IsBallCollided = true;
                GameplayEvents.OnActiveBallCollided.Invoke(collisionBall);
            }
                
        }
    }

    private void OnDestroy()
    {
        GameplayEvents.OnBallsDropStarted.RemoveListener(SetDropedBallsCells);
    }

    private void SetDropedBallsCells(List<HexCell> dropedBallsCells)
    {
        foreach(HexCell dropedBallCell in dropedBallsCells)
        {
            _dropedBalls.Add(dropedBallCell.GetBall());
        }
    }
}

public enum WallPosition
{
    Left,
    Right,
    Down,
    Top
}

public enum WallFunction
{
    Reflect,
    Destroy,
    AttachToField
}
