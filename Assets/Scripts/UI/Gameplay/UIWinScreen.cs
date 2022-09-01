using UnityEngine;

public class UIWinScreen : MonoBehaviour
{
    [SerializeField] private bool _hideOnAwake; 

    private void Awake()
    {
        if (_hideOnAwake)
            gameObject.SetActive(false);

        GameplayEvents.OnGameWin.AddListener(ShowWinScreen);
    }

    private void OnDestroy()
    {
        GameplayEvents.OnGameWin.RemoveListener(ShowWinScreen);
    }

    public void ShowWinScreen()
    {
        gameObject.SetActive(true);
        UIEvents.OnClickPause.Invoke();
    }
}
