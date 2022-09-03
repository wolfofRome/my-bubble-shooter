using UnityEngine;

public class LevelSave : MonoBehaviour
{
    private void Awake()
    {
        GameplayEvents.OnGameWin.AddListener(SaveLevelState);
    }

    private void OnDestroy()
    {
        GameplayEvents.OnGameWin.RemoveListener(SaveLevelState);
    }

    private void SaveLevelState()
    {
        string levelId = LevelDataHolder.LevelData.LevelId;

        if (PlayerPrefs.GetInt(levelId) > 0)
            return;

        PlayerPrefs.SetInt(levelId, 1);
        PlayerPrefs.Save();
    }
}
