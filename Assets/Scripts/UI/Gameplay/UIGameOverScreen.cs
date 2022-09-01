using UnityEngine;

public class UIGameOverScreen : MonoBehaviour
{
    [SerializeField] private bool _hideOnAwake;

    private void Awake()
    {
        if (_hideOnAwake)
            gameObject.SetActive(false);

        GameplayEvents.OnGameOver.AddListener(ShowGameOverScreen);
    }

    private void OnDestroy()
    {
        GameplayEvents.OnGameOver.RemoveListener(ShowGameOverScreen);
    }

    private void ShowGameOverScreen()
    {
        gameObject.SetActive(true);
        UIEvents.OnClickPause.Invoke();
    }
}
