public sealed class UIPause : UINext
{
    protected override void EventsSubscribe()
    {
        base.EventsSubscribe();
        UINextButton.onClick.AddListener(PauseClick);
    }

    protected override void EventsUnsubscribe()
    {
        base.EventsUnsubscribe();
        UINextButton.onClick.RemoveListener(PauseClick);
    }

    private void PauseClick()
    {
        UIEvents.OnClickPause.Invoke();
    }
}
