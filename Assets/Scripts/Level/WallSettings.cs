using UnityEngine;

public class WallSettings : MonoBehaviour
{
    [SerializeField] private WallPosition _position;
    [SerializeField] private bool _isDestroyingWall;

    private Vector2 _wallNormal;
    private Ball _destroyedActiveBall;

    public Vector2 WallNormal { get => _wallNormal; }

    public bool IsDestroyingWall { get => _isDestroyingWall; }

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
        if (IsDestroyingWall)
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
        if (IsDestroyingWall)
        {
            if (collision.GetComponent<Ball>())
            {
                Ball ball = collision.GetComponent<Ball>();

                if (ball.IsActiveBall)
                    _destroyedActiveBall = ball;
            }
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
