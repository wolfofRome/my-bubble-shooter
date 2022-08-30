using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UINextBall : MonoBehaviour
{
    #region Cache Components
    private Image _nextBallImage;
    public Image NextBallImage
    {
        get
        {
            if (_nextBallImage == null)
                _nextBallImage = GetComponent<Image>();
            return _nextBallImage;
        }
    }
    #endregion
}
