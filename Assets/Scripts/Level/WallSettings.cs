using UnityEngine;

public class WallSettings : MonoBehaviour
{
    [SerializeField] private WallPosition _position;
    [SerializeField] private WallFunction _wallFunction;

    private Vector2 _wallNormal;
    private Ball _destroyedActiveBall;

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

        GameplayEvents.OnActiveBallSetOnField.AddListener(RemoveDestroyedActiveBall);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (WallFunctionValue == WallFunction.Destroy)
        {
            if (collision.GetComponent<Ball>() != null)
            {
                Ball destroyedBall = collision.GetComponent<Ball>();

                if (!destroyedBall.IsActiveBall)
                {
                    destroyedBall.DestroyBall();
                }

                if(destroyedBall.IsActiveBall && _destroyedActiveBall != null)
                {
                    _destroyedActiveBall.DestroyBall();
                    _destroyedActiveBall = null;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (WallFunctionValue == WallFunction.Destroy)
        {
            if (collision.GetComponent<Ball>())
            {
                Ball ball = collision.GetComponent<Ball>();

                if (ball.IsActiveBall)
                    _destroyedActiveBall = ball;
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
        else if(WallFunctionValue == WallFunction.Destroy)
        {
            collisionBall.DestroyBall();
        }
    }

    private void OnDestroy()
    {
        GameplayEvents.OnActiveBallSetOnField.RemoveListener(RemoveDestroyedActiveBall);
    }

    private void RemoveDestroyedActiveBall(Ball activeBall)
    {
        _destroyedActiveBall = null;
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
