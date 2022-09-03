using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(FpsCounter))]
public class UIFps : MonoBehaviour
{
    [Min(30)]
    [SerializeField] private int _maxFps;
    [SerializeField] private Text _fpsText;

    private FpsCounter _fpsCounter;
    private string[] _fpsStringsValue;


    private void Awake()
    {
        _fpsCounter = GetComponent<FpsCounter>();

        _fpsStringsValue = new string[_maxFps + 1];
        
        for(int i = 0; i <= _maxFps; i++)
        {
            _fpsStringsValue[i] = i.ToString();
        }
    }

    private void Update()
    {
        _fpsText.text = _fpsStringsValue[Mathf.Clamp(_fpsCounter.AverageFPS, 0, _maxFps)];
    }
}
