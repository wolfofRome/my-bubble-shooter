public sealed class UIReturnToMainMenu : UIChangeScene
{
    protected override void EventSubscribe()
    {
        base.EventSubscribe();
        ChangeSceneButton.onClick.AddListener(ClickReturnToMainMenu);
    }

    protected override void EventUnsubscribe()
    {
        base.EventUnsubscribe();
        ChangeSceneButton.onClick.RemoveListener(ClickReturnToMainMenu);
    }

    private void ClickReturnToMainMenu()
    {
        UIEvents.OnClickReturnToMainMenu.Invoke();
        LevelDataHolder.LevelData = null;
    }
}
