using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Ball : MonoBehaviour
{
    private Rigidbody2D _rbBall;

    public Rigidbody2D RbBall
    {
        get
        {
            if (_rbBall == null)
                _rbBall = GetComponent<Rigidbody2D>();
            return _rbBall;
        }
    }
    

}
