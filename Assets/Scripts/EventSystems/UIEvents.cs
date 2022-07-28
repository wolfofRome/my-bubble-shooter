using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIEvents
{
    private static UnityEvent _onButtonPlayPressed = new UnityEvent();

    public static UnityEvent OnButtonPlayPressed { get => _onButtonPlayPressed; }
}
