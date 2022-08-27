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

    public HexGrid FieldGrid { get => _fieldGrid; }


    private void Awake()
    {
        _ballPrefabs.AddRange(LevelDataHolder.LevelData.BallsTypeInLevel);
        _levelAssetsPath = LevelDataHolder.LevelData.PathToLevelFile;
        _level = LevelDataHolder.LevelData.LevelFile;

        GameplayEvents.OnActiveBallCollided.AddListener(SetActiveBallAtField);
        GameplayEvents.OnActiveBallSetOnField.AddListener(TryDestroyBallGroup);
        GameplayEvents.OnBallGroupDestroyed.AddListener(FallBalls);
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
        GameplayEvents.OnBallGroupDestroyed.RemoveListener(FallBalls);
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

        if (cell.GetNeighbor(HexDirection.NW) == null && cell.GetNeighbor(HexDirection.NE) == null)
            ball.IsFirstLineBall = true;

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
        cell.RemoveBall();
    }

    private void DropBallFromField(Ball dropBall)
    {
        dropBall.CircleCollider.isTrigger = true;
        dropBall.JointConnection.enabled = false;
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
            Ball destroyedBall = cell.GetBall();
            RemoveBallFromField(cell);
            destroyedBall.DestroyBall();
        }

        GameplayEvents.OnBallGroupDestroyed.Invoke(_ballsOnField);
    }

    private void FallBalls(List<Ball> destroyedBalls)
    {
        List<HexCell> supposedBallCells = new List<HexCell>();
        foreach (Ball ballOnField in _ballsOnField)
        {
            if (ballOnField.IsFirstLineBall)
                continue;

            HexCell cell = _fieldGrid.GetCellFromPosition(ballOnField.RbBall.position);

            HexCell cellNeighborNW = cell.GetNeighbor(HexDirection.NW);
            if (cellNeighborNW != null && cellNeighborNW.GetBall() != null)
                continue;

            HexCell cellNeighborNE = cell.GetNeighbor(HexDirection.NE);
            if (cellNeighborNE != null && cellNeighborNE.GetBall() != null)
                continue;

            supposedBallCells.Add(cell);
        }

        for (int i = 0; i < supposedBallCells.Count; i++)
        {
            List<HexCell> checkedFallingBallsCells = new List<HexCell>();
            checkedFallingBallsCells.Add(supposedBallCells[i]);

            Queue<HexCell> checkedFallingBallsEdgeCells = new Queue<HexCell>();
            checkedFallingBallsEdgeCells.Enqueue(supposedBallCells[i]);

            while(checkedFallingBallsEdgeCells.Count > 0)
            {
                HexCell cell = checkedFallingBallsEdgeCells.Dequeue();

                HexCell checkingCellW = cell.GetNeighbor(HexDirection.W);
                if(checkingCellW != null && !checkedFallingBallsCells.Contains(checkingCellW))
                {
                    if (!supposedBallCells.Contains(checkingCellW))
                    {
                        HexCell checkingCellWNeighborNE = checkingCellW.GetNeighbor(HexDirection.NE);
                        if (checkingCellWNeighborNE != null && checkingCellWNeighborNE.GetBall() != null && !checkedFallingBallsCells.Contains(checkingCellWNeighborNE))
                        {
                            checkedFallingBallsCells.Clear();
                            break;
                        }
                    }
                    else
                    {
                        supposedBallCells.Remove(checkingCellW);
                    }

                    if (checkingCellW.GetBall() != null)
                    {
                        checkedFallingBallsEdgeCells.Enqueue(checkingCellW);
                        checkedFallingBallsCells.Add(checkingCellW);
                    }
                }

                HexCell checkingCellSW = cell.GetNeighbor(HexDirection.SW);
                if (checkingCellSW != null && checkingCellSW.GetBall() != null && !checkedFallingBallsCells.Contains(checkingCellSW))
                {
                    checkedFallingBallsEdgeCells.Enqueue(checkingCellSW);
                    checkedFallingBallsCells.Add(checkingCellSW);
                }

                HexCell checkingCellSE = cell.GetNeighbor(HexDirection.SE);
                if (checkingCellSE != null && checkingCellSE.GetBall() != null && !checkedFallingBallsCells.Contains(checkingCellSE))
                {
                    checkedFallingBallsEdgeCells.Enqueue(checkingCellSE);
                    checkedFallingBallsCells.Add(checkingCellSE);
                }

                HexCell checkingCellE = cell.GetNeighbor(HexDirection.E);
                if(checkingCellE != null && !checkedFallingBallsCells.Contains(checkingCellE))
                {
                    if (!supposedBallCells.Contains(checkingCellE))
                    {
                        HexCell checkingCellENeighborNW = checkingCellE.GetNeighbor(HexDirection.NW);

                        if (checkingCellENeighborNW != null && checkingCellENeighborNW.GetBall() != null && !checkedFallingBallsCells.Contains(checkingCellENeighborNW))
                        {
                            checkedFallingBallsCells.Clear();
                            break;
                        }
                    }
                    else
                    {
                        supposedBallCells.Remove(checkingCellE);
                    }

                    if(checkingCellE.GetBall() != null)
                    {
                        checkedFallingBallsEdgeCells.Enqueue(checkingCellE);
                        checkedFallingBallsCells.Add(checkingCellE);
                    }
                }
            }

            foreach(var b in checkedFallingBallsCells)
            {
                DropBallFromField(b.GetBall());
                RemoveBallFromField(b);
            }
        }
    }

    private List<HexCell> GetBallGroupCells(Ball ball)
    {
        HexCell cell = _fieldGrid.GetCellFromPosition(ball.RbBall.position);

        if (cell == null)
            return null;

        List<HexCell> ballGroupCells = new List<HexCell>();
        ballGroupCells.Add(cell);

        for (int i = 0; i < ballGroupCells.Count; i++)
        {
            foreach (HexCell neighbor in ballGroupCells[i].Neighbors)
            {
                if (neighbor == null || neighbor.GetBall() == null)
                    continue;

                if (!ballGroupCells.Contains(neighbor) && neighbor.GetBall().TypeId == cell.GetBall().TypeId)
                    ballGroupCells.Add(neighbor);
            }
        }

        return ballGroupCells;
    }
}
