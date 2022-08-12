using UnityEngine;

public class WallSettings : MonoBehaviour
{
    [SerializeField] private WallPosition position;

    private Vector2 _wallNormal;
    public Vector2 WallNormal { get => _wallNormal; }

    private void Awake()
    {
        switch (position)
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
    }
}

public enum WallPosition
{
    Left,
    Right,
    Down,
    Top
}
