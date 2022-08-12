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
    private readonly List<BallTypePrefab> _ballPrefabs = new List<BallTypePrefab>();
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

            BallTypePrefab _ballPrefab = _ballPrefabs.FirstOrDefault(t => t.Id == type);

            if (_ballPrefab.Prefab == null)
            {
                cellIndex++;
                continue;
            }

            HexCell cell = _fieldGrid.Cells[fromIndex + cellIndex];
            Ball ball = Instantiate(_ballPrefab.Prefab, transform);

            if (fromIndex == _fieldGrid.Cells.Length - _fieldGrid.Width)
                ball.RbBall.bodyType = RigidbodyType2D.Static;

            ball.TypeId = type;
            SetBallAtField(ball, cell);

            cellIndex++;
        }
    }

    private void SetBallAtField(Ball ball, HexCell cell)
    {
        if(ball == null)
        {
            Debug.LogWarning("Attached ball is null!");
            return;
        }

        if(cell == null)
        {
            Debug.LogWarning("Cell is not found");
            return;
        }

        ball.transform.SetParent(gameObject.transform);
        cell.SetBall(ball);
        _ballsOnField.Add(ball);

        ball.transform.position = cell.transform.position;
        ball.AddJointConnection(cell.transform.position);

        ball.RbBall.gravityScale = 1f;
        ball.RbBall.mass = 1f;
    }

    private void SetActiveBallAtField(Ball activeBall)
    {
        HexCell cell = _fieldGrid.GetCellFromPosition(activeBall.RbBall.position);
        SetBallAtField(activeBall, cell);

        GameplayEvents.OnActiveBallSetOnField.Invoke(activeBall);
    }

    private void RemoveBallFromField(HexCell cell)
    {
        _ballsOnField.Remove(cell.GetBall());
        Destroy(cell.GetBall().gameObject);
        cell.RemoveBall();
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

        foreach (HexCell cell in ballGroupCells)
        {
            RemoveBallFromField(cell);
        }

        GameplayEvents.OnBallGroupDestroyed.Invoke(_ballsOnField);
    }
}
