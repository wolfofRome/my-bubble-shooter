using UnityEngine;
using UnityEngine.UI;

public sealed class UILevel : UIChangeScene
{
    [SerializeField] private LevelData _levelData;
    [SerializeField] private Text _levelLabel;

    public LevelData LevelData
    {
        set
        {
            _levelData = value;
            UpdateDisplayData();
        }

        get
        {
            return _levelData;
        }
    }

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

    private void UpdateDisplayData()
    {
        if (_levelData == null)
            return;

        if (_levelLabel != null)
        {
            _levelLabel.text = _levelData.LevelName;
        }

        gameObject.name = _levelData.name;

        
        ChangeSceneButton.interactable = _levelData.UnlockLevelOnStart;
    }
}
