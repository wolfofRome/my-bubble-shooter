using UnityEngine.Events;

public class UIEvents
{
    private readonly static UnityEvent _onClickLevelStart = new UnityEvent();
    private readonly static UnityEvent _onClickPause = new UnityEvent();
    private readonly static UnityEvent _onClickResume = new UnityEvent();
    private readonly static UnityEvent _onClickRestart = new UnityEvent();
    private readonly static UnityEvent _onClickReturnToMainMenu = new UnityEvent();

    public static UnityEvent OnClickLevelStart { get => _onClickLevelStart; }
    public static UnityEvent OnClickPause { get => _onClickPause; }
    public static UnityEvent OnClickResume { get => _onClickResume; }
    public static UnityEvent OnClickRestart { get => _onClickRestart; }
    public static UnityEvent OnClickReturnToMainMenu { get => _onClickReturnToMainMenu; }
}
