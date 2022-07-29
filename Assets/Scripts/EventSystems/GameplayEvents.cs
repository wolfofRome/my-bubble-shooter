using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameplayEvents
{
    private readonly static UnityEvent _onGameFieldGenerated = new UnityEvent();
    private readonly static UnityEvent<Ball> _onActiveBallCollided = new UnityEvent<Ball>();
    private readonly static UnityEvent<Ball> _onActiveBallSetOnField = new UnityEvent<Ball>();
    private readonly static UnityEvent<List<Ball>> _onBallGroupDestroyed = new UnityEvent<List<Ball>>();
    private readonly static UnityEvent<List<Ball>> _onGameFieldChanged = new UnityEvent<List<Ball>>();
    private readonly static UnityEvent<int> _onCountAvailableBallsChanged = new UnityEvent<int>();

    public static UnityEvent OnGameFieldGenerated { get => _onGameFieldGenerated; }

    public static UnityEvent<Ball> OnActiveBallCollided { get => _onActiveBallCollided; }

    public static UnityEvent<Ball> OnActiveBallSetOnField { get => _onActiveBallSetOnField; }

    public static UnityEvent<List<Ball>> OnBallGroupDestroyed { get => _onBallGroupDestroyed; }

    public static UnityEvent<List<Ball>> OnGameFieldChanged { get => _onGameFieldChanged; }

    public static UnityEvent<int> OnCountAvailableBallsChanged { get => _onCountAvailableBallsChanged; }
}
