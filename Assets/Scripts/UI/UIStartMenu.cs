using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIStartMenu : MonoBehaviour
{
    [Header("Main menu buttons")]
    [SerializeField] private Button _buttonPlay;
    [SerializeField] private Button _buttonInformation;

    private void Start()
    {
        if(_buttonPlay != null)
        {
            // TODO
        }
            
        if (_buttonInformation != null)
            _buttonInformation.onClick.AddListener(GoToInformationScene);
    }

    private void OnDestroy()
    {
        if (_buttonInformation != null)
            _buttonInformation.onClick.RemoveListener(GoToInformationScene);
    }

    private void GoToInformationScene()
    {
        SceneManager.LoadSceneAsync((int)GameScenes.InformationScene);
    }
}
