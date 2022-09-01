using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Button))]
public class UIChangeScene : MonoBehaviour
{
    [SerializeField] private GameScenes _changingScene;

    #region Cache Component
    private Button _changeSceneButton;
    public Button ChangeSceneButton
    {
        get
        {
            if (_changeSceneButton == null)
                _changeSceneButton = GetComponent<Button>();
            return _changeSceneButton;
        }
    }
    #endregion

    private void Awake()
    {
        EventSubscribe();
    }

    private void OnDestroy()
    {
        EventUnsubscribe();
    }

    private void LoadChangingScene()
    {
        StartCoroutine(LoadChangingSceneCoroutine());
    }

    private IEnumerator LoadChangingSceneCoroutine()
    {
        yield return SceneManager.LoadSceneAsync((int)_changingScene, LoadSceneMode.Single);
        yield break;
    }

    protected virtual void EventSubscribe()
    {
        ChangeSceneButton.onClick.AddListener(LoadChangingScene);
    }

    protected virtual void EventUnsubscribe()
    {
        ChangeSceneButton.onClick.RemoveListener(LoadChangingScene);
    }
}
