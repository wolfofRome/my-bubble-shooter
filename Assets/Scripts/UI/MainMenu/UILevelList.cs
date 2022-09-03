using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class UILevelList : MonoBehaviour
{
    [SerializeField] private LevelData[] _levels;
    [SerializeField] private UILevel _uiLevelPrefab;

    private readonly List<UILevel> _uiLevels = new List<UILevel>();

    private void OnEnable()
    {
        CreateLevelListMenu();
    }

    private void OnDisable()
    {
        RemoveLevelListMenu();
    }

    private void CreateLevelListMenu()
    {
        if (_levels == null || _levels.Length <= 0)
            return;

        for(int i = 0; i < _levels.Length; i++)
        {
            UILevel newUiLevel = Instantiate(_uiLevelPrefab, transform);
            newUiLevel.LevelData = _levels[i];
            _uiLevels.Add(newUiLevel);
        }


        int isLevelFinished;
        for(int i = 0; i < _levels.Length; i++)
        {
            isLevelFinished = PlayerPrefs.GetInt(_levels[i].LevelId);

            if (isLevelFinished != 1)
                continue;

            LevelData[] unlockLevels = _levels[i].UnlockLevels;
            for(int l = 0; l < unlockLevels.Length; l++)
            {
                UILevel unlockLevel = _uiLevels.FirstOrDefault(uiLevel => uiLevel.LevelData.LevelId == unlockLevels[l].LevelId);
                unlockLevel.ChangeSceneButton.interactable = true;
            }
        }
    }

    private void RemoveLevelListMenu()
    {
        for(int i = 0; i < _uiLevels.Count; i++)
        {
            Destroy(_uiLevels[i].gameObject);
        }

        _uiLevels.Clear();
    }
}
