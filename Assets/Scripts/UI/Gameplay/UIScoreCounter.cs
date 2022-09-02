using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScoreCounter : MonoBehaviour
{
    [SerializeField] private Text _scoreCounter;

    private int _score = 0;

    private void Awake()
    {
        if(_scoreCounter == null)
        {
            Debug.LogError("Score counter is not set!");
            return;
        }

        _scoreCounter.text = _score.ToString();

        GameplayEvents.OnAddScore.AddListener(AddScore);
    }

    private void OnDestroy()
    {
        GameplayEvents.OnAddScore.RemoveListener(AddScore);
    }

    private void AddScore(int score)
    {
        if(score < 1)
        {
            Debug.LogError("Score value is less then 1");
            return;
        }

        _score += score;
        _scoreCounter.text = _score.ToString();
    }
}
