using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIExit : MonoBehaviour
{
    #region Cache Components
    private Button _exitButton;
    public Button ExitButton
    {
        get
        {
            if (_exitButton == null)
                _exitButton = GetComponent<Button>();
            return _exitButton;
        }
    }
    #endregion

    private void Awake()
    {
        ExitButton.onClick.AddListener(ExitFromGame);
    }

    private void OnDestroy()
    {
        ExitButton.onClick.RemoveListener(ExitFromGame);
    }

    private void ExitFromGame()
    {
        Application.Quit();
    }
}
