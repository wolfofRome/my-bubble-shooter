using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIEvents
{
    private static UnityEvent _onClickLevelStart = new UnityEvent();

    public static UnityEvent OnClickLevelStart { get => _onClickLevelStart; }
}
