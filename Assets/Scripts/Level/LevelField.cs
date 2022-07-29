using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Van.HexGrid;
using System.IO;
using System.Linq;

public class LevelField : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private HexGrid _fieldGrid;

    private string _levelAssetsPath;
    private TextAsset _level;
    private readonly List<BallPrefabType> _ballPrefabs = new List<BallPrefabType>();
    private readonly List<Ball> _ballsOnField = new List<Ball>();

    private void Awake()
    {
        _ballPrefabs.AddRange(LevelDataHolder.LevelData.BallsTypeInLevel);
        _levelAssetsPath = LevelDataHolder.LevelData.PathToLevelFile;
        _level = LevelDataHolder.LevelData.LevelFile;

        GameplayEvents.OnActiveBallCollided.AddListener(SetActiveBallAtField);
        GameplayEvents.OnActiveBallSetOnField.AddListener(TryDestroyBallGroup);
    }

    private void Start()
    {
        GenerateGameField();
        GameplayEvents.OnGameFieldGenerated.Invoke();

        foreach(Ball ball in _ballsOnField)
        {
            HexCell cell = _fieldGrid.GetCellFromPosition(ball.RbBall.position);
            
            foreach(HexCell neighborCell in cell.Neighbors)
            {
                if(neighborCell != null && neighborCell.GetBall() != null)
                {
                    neighborCell.GetBall().AddJointConnection(ball);
                }
            }            
        }
    }

    private void OnDestroy()
    {
        GameplayEvents.OnActiveBallSetOnField.RemoveListener(TryDestroyBallGroup);
        GameplayEvents.OnActiveBallCollided.RemoveListener(SetActiveBallAtField);
    }

    private void GenerateGameField()
    {
        string filePath = _levelAssetsPath + "/" + _level.name + ".txt";

        StreamReader content = new StreamReader(filePath);
        List<string> types = new List<string>();

        while (content.Peek() >= 0)
        {
            types.AddRange(content.ReadLine().Split('-'));
        }

        int cellIndex = 0;
        int fromIndex = _fieldGrid.Cells.Length - _fieldGrid.Width;
        foreach (string type in types)
        {
            if (cellIndex == _fieldGrid.Width)
            {
                cellIndex = 0;
                fromIndex -= _fieldGrid.Width;
            }

            BallPrefabType _ballPrefab = _ballPrefabs.FirstOrDefault(t => t.Id == type);

            if (_ballPrefab.Prefab == null)
            {
                cellIndex++;
                continue;
            }

            HexCell cell = _fieldGrid.Cells[fromIndex + cellIndex];
            Ball ball = Instantiate(_ballPrefab.Prefab, cell.transform.position, Quaternion.identity);

            if (fromIndex == _fieldGrid.Cells.Length - _fieldGrid.Width)
                ball.RbBall.bodyType = RigidbodyType2D.Static;

            _ballsOnField.Add(ball);
            ball.TypeId = type;
            cell.SetBall(ball);
            ball.transform.SetParent(gameObject.transform);
            cellIndex++;
        }
    }

    private void SetActiveBallAtField(Ball activeBall)
    {
        HexCell cell = _fieldGrid.GetCellFromPosition(activeBall.RbBall.position);

        if(cell == null)
        {
            Debug.LogWarning("Cell is not found");
            return;
        }

        cell.SetBall(activeBall);
        activeBall.RbBall.constraints = RigidbodyConstraints2D.FreezePosition;
        activeBall.transform.SetParent(transform);
        
        foreach(HexCell neighborCell in cell.Neighbors)
        {
            if (neighborCell == null || neighborCell.GetBall() == null)
                continue;
            
            activeBall.AddJointConnection(neighborCell.GetBall());
            neighborCell.GetBall().AddJointConnection(activeBall);
        }

        activeBall.RbBall.constraints = RigidbodyConstraints2D.None;
        activeBall.RbBall.constraints = RigidbodyConstraints2D.FreezeRotation;

        _ballsOnField.Add(activeBall);

        GameplayEvents.OnActiveBallSetOnField.Invoke(activeBall);
    }

    private List<HexCell> GetBallGroupCells(Ball ball)
    {
        HexCell cell = _fieldGrid.GetCellFromPosition(ball.RbBall.position);

        if(cell == null)
        {
            return null;
        }

        List<HexCell> ballGroupCells = new List<HexCell>();
        ballGroupCells.Add(cell);

        for (int i = 0; i < ballGroupCells.Count; i++)
        {
            foreach (HexCell neighbor in ballGroupCells[i].Neighbors)
            {
                if (neighbor == null || neighbor.GetBall() == null)
                    continue;

                if (!ballGroupCells.Contains(neighbor) && neighbor.GetBall().TypeId == cell.GetBall().TypeId)
                {
                    ballGroupCells.Add(neighbor);
                }
            }
        }

        return ballGroupCells;
    }

    private void TryDestroyBallGroup(Ball ball)
    {
        List<HexCell> ballGroupCells = GetBallGroupCells(ball);

        if(ballGroupCells == null)
        {
            Debug.LogWarning("Can't get ball group cells!");
            return;
        }

        if (ballGroupCells.Count < 3)
            return;

        foreach (HexCell c in ballGroupCells)
        {
            _ballsOnField.Remove(c.GetBall());
            Destroy(c.GetBall().gameObject);
            c.RemoveBall();
        }

        var t = _fieldGrid.Cells.Reverse().ToArray();

        int lastLineBallCount = 0;
        for(int i = 0; i < 10; i++)
        {
            if (t[i].GetBall() != null)
                lastLineBallCount++;
                
        }

        Debug.Log(lastLineBallCount);

        GameplayEvents.OnBallGroupDestroyed.Invoke(_ballsOnField);
    }
}
