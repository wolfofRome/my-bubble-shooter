public sealed class UIResume : UIBack
{
    protected override void EventsSubscribe()
    {
        base.EventsSubscribe();
        UIBackButton.onClick.AddListener(ResumeClick);
    }

    protected override void EventsUnsubscribe()
    {
        base.EventsUnsubscribe();
        UIBackButton.onClick.RemoveListener(ResumeClick);
    }

    private void ResumeClick()
    {
        UIEvents.OnClickResume.Invoke();
    }
}
