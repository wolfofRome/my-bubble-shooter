using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "LevelData", menuName = "Bubble Shooter/Level")]
public class LevelData : ScriptableObject
{
    [Header("Level File")]
    [SerializeField] private string _pathToLevelFile;
    [SerializeField] private TextAsset _levelFile;

    [Header("Level Settings")]
    [SerializeField] private string _levelName;
    [SerializeField] private List<BallPrefabType> _ballsTypeInLevel;


    public string PathToLevelFile { get => _pathToLevelFile; }
    public TextAsset LevelFile { get => _levelFile; }
    public string LevelName { get => _levelName; }
    public List<BallPrefabType> BallsTypeInLevel { get => _ballsTypeInLevel; }

}
