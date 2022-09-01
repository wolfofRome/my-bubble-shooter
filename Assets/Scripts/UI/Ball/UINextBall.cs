using UnityEngine;
using UnityEngine.UI;

public class UINextBall : MonoBehaviour
{
    [SerializeField] private Image _nextBallImage;
    [SerializeField] private Text _availableBallsCount;

    private void Awake()
    {
        if(_nextBallImage == null)
        {
            Debug.LogError("Next ball image is not set!");    
        }

        if(_availableBallsCount == null)
        {
            Debug.LogError("Available balls count text is not set!");
        }

        GameplayEvents.OnNextBallChanged.AddListener(ChangeNextBallImage);
        GameplayEvents.OnAvailableBallsCountChanged.AddListener(ChangeAvailableBallsCount);
    }

    private void OnDestroy()
    {
        GameplayEvents.OnNextBallChanged.RemoveListener(ChangeNextBallImage);
        GameplayEvents.OnAvailableBallsCountChanged.RemoveListener(ChangeAvailableBallsCount);
    }

    private void ChangeNextBallImage(Ball nextBall)
    {
        _nextBallImage.color = nextBall.BallSpriteRenderer.color;        
    }

    private void ChangeAvailableBallsCount(int availableBallsCount)
    {
        if (availableBallsCount <= 0)
            _nextBallImage.enabled = false;

        _availableBallsCount.text = availableBallsCount.ToString();
    }
}
