using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public void Awake()
    {
        
    }

}
