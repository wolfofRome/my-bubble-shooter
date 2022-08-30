using System.Collections.Generic;
using UnityEngine.Events;

public class GameplayEvents
{
    private readonly static UnityEvent _onGameFieldGenerated = new UnityEvent();
    private readonly static UnityEvent<Ball> _onActiveBallCollided = new UnityEvent<Ball>();
    private readonly static UnityEvent<Ball> _onActiveBallSetOnField = new UnityEvent<Ball>();
    private readonly static UnityEvent<Ball> _onActiveBallDestroyed = new UnityEvent<Ball>();
    private readonly static UnityEvent<List<Ball>> _onBallGroupDestroyed = new UnityEvent<List<Ball>>();
    private readonly static UnityEvent _onAllGameActionsEnd = new UnityEvent();
    private readonly static UnityEvent<List<Ball>> _onAvailableBallCountChanged = new UnityEvent<List<Ball>>();
    private readonly static UnityEvent _onAvailableBallsEnd = new UnityEvent();
    private readonly static UnityEvent _onGameOver = new UnityEvent();
    private readonly static UnityEvent _onGameWin = new UnityEvent();
    private readonly static UnityEvent<List<Ball>> _onGameFieldChanged = new UnityEvent<List<Ball>>();
    private readonly static UnityEvent<int> _onCountAvailableBallsChanged = new UnityEvent<int>();

    public static UnityEvent OnGameFieldGenerated { get => _onGameFieldGenerated; }
    public static UnityEvent<Ball> OnActiveBallCollided { get => _onActiveBallCollided; }
    public static UnityEvent<Ball> OnActiveBallSetOnField { get => _onActiveBallSetOnField; }
    public static UnityEvent<Ball> OnActiveBallDestroyed { get => _onActiveBallDestroyed; }
    public static UnityEvent<List<Ball>> OnBallGroupDestroyed { get => _onBallGroupDestroyed; }
    public static UnityEvent OnAllGameActionsEnd { get => _onAllGameActionsEnd; }
    public static UnityEvent OnAvailableBallsEnd { get => _onAvailableBallsEnd; }
    public static UnityEvent OnGameOver { get => _onGameOver; }
    public static UnityEvent OnGameWin { get => _onGameWin; }
    public static UnityEvent<List<Ball>> OnGameFieldChanged { get => _onGameFieldChanged; }
}
