using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "LevelData", menuName = "Bubble Shooter/Level")]
public class LevelData : ScriptableObject
{
    [Header("Level File")]
    [SerializeField] private string _pathToLevelFile;
    [SerializeField] private TextAsset _levelFile;

    [Header("Level Settings")]
    [SerializeField] private string _levelId;
    [SerializeField] private string _levelName;
    [SerializeField] private bool _unlockLevelOnStart;
    [SerializeField] private LevelData[] _unlockLevels;
    [SerializeField] private int _countAvailableBalls;
    [SerializeField] private List<BallTypePrefab> _ballsTypeInLevel;

    [Header("Score settings")]
    [Min(1)]
    [SerializeField] private int _ballDestroyScore;
    [Min(1)]
    [SerializeField] private int _ballDestroyScaleGroup;
    [Min(1)]
    [SerializeField] private int _ballDropScore;
    [Min(1)]
    [SerializeField] private int _ballDropScaleGroup;

    public string PathToLevelFile { get => _pathToLevelFile; }
    public TextAsset LevelFile { get => _levelFile; }
    public string LevelId { get => _levelId; }
    public string LevelName { get => _levelName; }
    public bool UnlockLevelOnStart { get => _unlockLevelOnStart; }
    public LevelData[] UnlockLevels { get => _unlockLevels; }
    public int CountAvailableBalls { get => _countAvailableBalls; }
    public List<BallTypePrefab> BallsTypeInLevel { get => _ballsTypeInLevel; }
    public int BallDestroyScore { get => _ballDestroyScore; }
    public int BallDestroyScaleGroup { get => _ballDestroyScaleGroup; }
    public int BallDropScore { get => _ballDropScore; }
    public int BallDropScaleGroup { get => _ballDropScaleGroup; }
}
