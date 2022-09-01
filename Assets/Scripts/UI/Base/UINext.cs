using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UINext : MonoBehaviour
{
    [SerializeField] private Canvas _currentCanvas;
    [SerializeField] private Canvas _nextCanvas;

    #region Cache Components
    private Button _uiNextButton;
    public Button UINextButton
    {
        get
        {
            if (_uiNextButton == null)
                _uiNextButton = GetComponent<Button>();
            return _uiNextButton;
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

    private void Next()
    {
        if (_nextCanvas == null)
        {
            Debug.LogWarning("Next canvas variable is not set");
            return;
        }

        _nextCanvas.gameObject.SetActive(true);

        if(_currentCanvas != null)
            _currentCanvas.gameObject.SetActive(false);
    }

    protected virtual void EventsSubscribe()
    {
        UINextButton.onClick.AddListener(Next);
    }

    protected virtual void EventsUnsubscribe()
    {
        UINextButton.onClick.RemoveListener(Next);
    }
}
