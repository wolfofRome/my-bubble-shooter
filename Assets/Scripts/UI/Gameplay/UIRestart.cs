public sealed class UIRestart : UIChangeScene
{
    protected override void EventSubscribe()
    {
        base.EventSubscribe();
        ChangeSceneButton.onClick.AddListener(ClickRestart);
    }

    protected override void EventUnsubscribe()
    {
        base.EventUnsubscribe();
        ChangeSceneButton.onClick.AddListener(ClickRestart);
    }

    public void ClickRestart()
    {
        UIEvents.OnClickRestart.Invoke();
    }
}
