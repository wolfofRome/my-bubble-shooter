using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Button))]
public class UIBack : MonoBehaviour
{
    [SerializeField] private Canvas _currentCanvas;
    [SerializeField] private Canvas _backCanvas;

    #region Cache Components
    private Button _uiBackButton;
    public Button UIBackButton
    {
        get
        {
            if (_uiBackButton == null)
                _uiBackButton = GetComponent<Button>();
            return _uiBackButton;
        }
    }
    #endregion

    private void Awake()
    {
        UIBackButton.onClick.AddListener(Back);
    }

    private void OnDestroy()
    {
        UIBackButton.onClick.RemoveListener(Back);
    }

    private void Back()
    {
        if(_backCanvas == null)
        {
            Debug.LogWarning("Next canvas variable is not set");
            return;
        }

        _backCanvas.gameObject.SetActive(true);
        _currentCanvas.gameObject.SetActive(false);
    }
}
