using System.Collections.Generic;
using UnityEngine;
using Van.HexGrid;
using System.IO;
using System.Linq;

public class LevelField : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private HexGrid _fieldGrid;
    private readonly List<Ball> _ballsOnField = new List<Ball>();

    public HexGrid FieldGrid { get => _fieldGrid; }
    public List<Ball> BallsOnField
    {
        get
        {
            return _ballsOnField;
        }
    }


    private void Awake()
    {
        GameplayEvents.OnActiveBallCollided.AddListener(SetActiveBallAtField);
        GameplayEvents.OnAllFieldActionsEnd.AddListener(CheckCountBallsOnField);
    }

    private void Start()
    {
        GenerateGameField();
        GameplayEvents.OnGameFieldGenerated.Invoke();
    }

    private void OnDestroy()
    {
        GameplayEvents.OnActiveBallCollided.RemoveListener(SetActiveBallAtField);
        GameplayEvents.OnAllFieldActionsEnd.RemoveListener(CheckCountBallsOnField);
    }

    private void GenerateGameField()
    {
        string filePath = LevelDataHolder.LevelData.PathToLevelFile + "/" + LevelDataHolder.LevelData.LevelFile.name + ".txt";

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

            BallTypePrefab _ballPrefab = LevelDataHolder.LevelData.BallsTypeInLevel.FirstOrDefault(t => t.Id == type);

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
        ball.AddJoint(cell.transform.position);

        ball.RbBall.gravityScale = 1f;
        ball.RbBall.mass = 1f;
    }

    private void SetActiveBallAtField(Ball activeBall)
    {
        HexCell cell = _fieldGrid.GetCellFromPosition(activeBall.RbBall.position);
        if (cell == null)
        {
            HexCell leftCell = _fieldGrid.GetCellFromPosition(activeBall.RbBall.position + (Vector2.left * activeBall.BallRadius));
            HexCell rightCell = _fieldGrid.GetCellFromPosition(activeBall.RbBall.position + (Vector2.right * activeBall.BallRadius));

            if (leftCell != null)
                cell = leftCell;
            else
                cell = rightCell;

            if(leftCell == null && rightCell == null)
            {
                activeBall.DestroyBall();
                GameplayEvents.OnAllFieldActionsEnd.Invoke();
                return;
            }
        }

        activeBall.IsActiveBall = false;
        SetBallAtField(activeBall, cell);

        GameplayEvents.OnActiveBallSetOnField.Invoke(activeBall);

        TryDestroyBallGroup(cell);
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

    private void TryDestroyBallGroup(HexCell ballCell)
    {
        List<HexCell> ballGroupCells = GetBallGroupCells(ballCell);

        if(ballGroupCells == null)
        {
            Debug.LogWarning("Can't get ball group cells!");
            return;
        }

        if (ballGroupCells.Count < 3)
        {
            GameplayEvents.OnAllFieldActionsEnd.Invoke();
            GameplayEvents.OnGameFieldChanged.Invoke(_ballsOnField);
            return;
        }
            

        foreach (HexCell cell in ballGroupCells)
        {
            Ball destroyedBall = cell.GetBall();
            RemoveBallFromField(cell);
            destroyedBall.DestroyBall();
        }

        int scale = Mathf.RoundToInt(ballGroupCells.Count / LevelDataHolder.LevelData.BallDestroyScaleGroup);
        if (scale <= 0)
            scale = 1;

        int finalScore = ballGroupCells.Count * LevelDataHolder.LevelData.BallDestroyScore * scale;
        GameplayEvents.OnAddScore.Invoke(finalScore);

        GameplayEvents.OnBallGroupDestroyed.Invoke(_ballsOnField);

        TryDropBalls();
    }

    private void TryDropBalls()
    {
        List<HexCell> dropedBallsCells = new List<HexCell>();
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
            List<HexCell> checkedDropingBallsCells = new List<HexCell>();
            checkedDropingBallsCells.Add(supposedBallCells[i]);

            Queue<HexCell> checkingDropingBallsCells = new Queue<HexCell>();
            checkingDropingBallsCells.Enqueue(supposedBallCells[i]);

            while(checkingDropingBallsCells.Count > 0)
            {
                HexCell cell = checkingDropingBallsCells.Dequeue();

                HexCell checkingCellW = cell.GetNeighbor(HexDirection.W);
                if(checkingCellW != null && !checkedDropingBallsCells.Contains(checkingCellW))
                {
                    if (!supposedBallCells.Contains(checkingCellW))
                    {
                        HexCell checkingCellWNeighborNE = checkingCellW.GetNeighbor(HexDirection.NE);
                        if (checkingCellWNeighborNE != null && checkingCellWNeighborNE.GetBall() != null && !checkedDropingBallsCells.Contains(checkingCellWNeighborNE))
                        {
                            checkedDropingBallsCells.Clear();
                            break;
                        }
                    }
                    else
                    {
                        supposedBallCells.Remove(checkingCellW);
                    }

                    if (checkingCellW.GetBall() != null)
                    {
                        checkingDropingBallsCells.Enqueue(checkingCellW);
                        checkedDropingBallsCells.Add(checkingCellW);
                    }
                }

                HexCell checkingCellSW = cell.GetNeighbor(HexDirection.SW);
                if (checkingCellSW != null && checkingCellSW.GetBall() != null && !checkedDropingBallsCells.Contains(checkingCellSW))
                {
                    checkingDropingBallsCells.Enqueue(checkingCellSW);
                    checkedDropingBallsCells.Add(checkingCellSW);
                }

                HexCell checkingCellSE = cell.GetNeighbor(HexDirection.SE);
                if (checkingCellSE != null && checkingCellSE.GetBall() != null && !checkedDropingBallsCells.Contains(checkingCellSE))
                {
                    checkingDropingBallsCells.Enqueue(checkingCellSE);
                    checkedDropingBallsCells.Add(checkingCellSE);
                }

                HexCell checkingCellE = cell.GetNeighbor(HexDirection.E);
                if(checkingCellE != null && !checkedDropingBallsCells.Contains(checkingCellE))
                {
                    if (!supposedBallCells.Contains(checkingCellE))
                    {
                        HexCell checkingCellENeighborNW = checkingCellE.GetNeighbor(HexDirection.NW);

                        if (checkingCellENeighborNW != null && checkingCellENeighborNW.GetBall() != null && !checkedDropingBallsCells.Contains(checkingCellENeighborNW))
                        {
                            checkedDropingBallsCells.Clear();
                            break;
                        }
                    }
                    else
                    {
                        supposedBallCells.Remove(checkingCellE);
                    }

                    if(checkingCellE.GetBall() != null)
                    {
                        checkingDropingBallsCells.Enqueue(checkingCellE);
                        checkedDropingBallsCells.Add(checkingCellE);
                    }
                }
            }

            dropedBallsCells.AddRange(checkedDropingBallsCells);
        }

        if(dropedBallsCells.Count <= 0)
        {
            GameplayEvents.OnAllFieldActionsEnd.Invoke();
            GameplayEvents.OnGameFieldChanged.Invoke(_ballsOnField);
            return;
        }

        GameplayEvents.OnBallsDropStarted.Invoke(dropedBallsCells);

        foreach (HexCell dropedBallCell in dropedBallsCells)
        {
            DropBallFromField(dropedBallCell.GetBall());
            RemoveBallFromField(dropedBallCell);
        }

        int scale = Mathf.RoundToInt(dropedBallsCells.Count / LevelDataHolder.LevelData.BallDropScaleGroup);
        if (scale <= 0)
            scale = 1;

        int finalScore = dropedBallsCells.Count * LevelDataHolder.LevelData.BallDropScore * scale;
        GameplayEvents.OnAddScore.Invoke(finalScore);

        GameplayEvents.OnGameFieldChanged.Invoke(_ballsOnField);
    }

    private List<HexCell> GetBallGroupCells(HexCell ballCell)
    {
        HexCell cell = ballCell;

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

    private void CheckCountBallsOnField()
    {
        if (_ballsOnField.Count <= 0)
            GameplayEvents.OnGameWin.Invoke();
        else
            GameplayEvents.OnCountBallsOnFieldChecked.Invoke();
    }
}
