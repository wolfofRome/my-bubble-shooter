using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameplayEvents
{
    private static UnityEvent<Ball> _onBallCollided = new UnityEvent<Ball>();

    public static UnityEvent<Ball> OnBallCollided
    {
        get
        {
            return _onBallCollided;
        }
    }
    
    
}
