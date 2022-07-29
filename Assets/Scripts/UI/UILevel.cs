using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Button))]
public class UILevel : MonoBehaviour
{
    [SerializeField] private LevelData _levelData;

    #region Cache Components
    private Button _uiLevelButton;
    public Button UILevelButton
    {
        get
        {
            if (_uiLevelButton == null)
                _uiLevelButton = GetComponent<Button>();
            return _uiLevelButton;
        }
    }
    #endregion

    private void Awake()
    {
        UILevelButton.onClick.AddListener(LevelStart);
    }

    private void OnDestroy()
    {
        UILevelButton.onClick.RemoveListener(LevelStart);
    }

    private void LevelStart()
    {
        UIEvents.OnClickLevelStart.Invoke();
        LevelDataHolder.LevelData = _levelData;
        SceneManager.LoadSceneAsync((int)GameScenes.GameplayScene);
    }

}
