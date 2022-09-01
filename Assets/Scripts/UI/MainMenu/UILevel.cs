using UnityEngine;

public sealed class UILevel : UIChangeScene
{
    [SerializeField] private LevelData _levelData;
    protected override void EventSubscribe()
    {
        base.EventSubscribe();
        ChangeSceneButton.onClick.AddListener(LevelStart);
    }

    protected override void EventUnsubscribe()
    {
        base.EventUnsubscribe();
        ChangeSceneButton.onClick.RemoveListener(LevelStart);
    }

    private void LevelStart()
    {
        UIEvents.OnClickLevelStart.Invoke();
        LevelDataHolder.LevelData = _levelData;
    }
}
