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
        EventsSubscribe();
    }

    private void OnDestroy()
    {
        EventsUnsubscribe();
    }

    private void Back()
    {
        if(_currentCanvas == null)
        {
            Debug.LogWarning("Current canvas is not set");
            return;
        }

        if(_backCanvas != null)
            _backCanvas.gameObject.SetActive(true);

        _currentCanvas.gameObject.SetActive(false);
    }

    protected virtual void EventsSubscribe()
    {
        UIBackButton.onClick.AddListener(Back);
    }

    protected virtual void EventsUnsubscribe()
    {
        UIBackButton.onClick.RemoveListener(Back);
    }
}
