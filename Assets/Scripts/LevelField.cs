using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Van.HexGrid;
using UnityEngine.UI;
using System.IO;
using System.Linq;

public class LevelField : MonoBehaviour
{
    [SerializeField] private HexGrid _fieldGrid;
    [Header("Prefabs")]
    [SerializeField] private List<BallPrefabType> _ballPrefabs;
    [SerializeField] private FixedJoint2D _fixedJointPrefab;
    [Header("Level")]
    [SerializeField] private string _levelAssetsPath;
    [SerializeField] private TextAsset _level;

    private List<Ball> _ballsOnField = new List<Ball>();

    private void Awake()
    {
        GameplayEvents.OnBallCollided.AddListener(AttachBallAtField);
        GameplayEvents.OnBallCollided.AddListener(TryDestroyBallGroup);
    }

    private void Start()
    {
        string filePath = _levelAssetsPath + "/" + _level.name + ".txt";

        StreamReader content = new StreamReader(filePath);
        List<string> types = new List<string>();


        while(content.Peek() >= 0)
        {
            types.AddRange(content.ReadLine().Split('-'));
        }


        int cellIndex = 0;
        int fromIndex = _fieldGrid.Cells.Length - _fieldGrid.Width;
        foreach (string type in types)
        {
            if(cellIndex == _fieldGrid.Width)
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
            _ballsOnField.Add(ball);
            ball._typeId = type;
            cell.SetBall(ball);
            ball.transform.SetParent(gameObject.transform);

            cellIndex++;
        }

        foreach(Ball ball in _ballsOnField)
        {
            HexCell cell = _fieldGrid.GetCellFromPosition(ball.RbBall.position);

            if(cell.GetNeighbor(HexDirection.SW) != null && cell.GetNeighbor(HexDirection.SW).GetBall() != null)
            {
                Ball ballSW = cell.GetNeighbor(HexDirection.SW).GetBall();
                FixedJoint2D ballJoint = Instantiate(_fixedJointPrefab, ball.RbBall.position, Quaternion.identity);

                ballJoint.gameObject.transform.SetParent(ball.transform);
                ballJoint.gameObject.name = ballSW.name + "joint";
                ballJoint.connectedBody = ballSW.RbBall;
                
            }

            if (cell.GetNeighbor(HexDirection.SE) != null && cell.GetNeighbor(HexDirection.SE).GetBall() != null)
            {
                Ball ballSE = cell.GetNeighbor(HexDirection.SE).GetBall();
                FixedJoint2D ballJoint = Instantiate(_fixedJointPrefab, ball.RbBall.position, Quaternion.identity);

                ballJoint.gameObject.transform.SetParent(ball.transform);
                ballJoint.gameObject.name = ballSE.name + "joint";
                ballJoint.connectedBody = ballSE.RbBall;
            }
        }
        
    }

    private void OnDestroy()
    {
        GameplayEvents.OnBallCollided.RemoveListener(AttachBallAtField);
    }

    public void AttachBallAtField(Ball ball)
    {
        ball.RbBall.constraints = RigidbodyConstraints2D.FreezeAll;
        HexCell cell = _fieldGrid.GetCellFromPosition(ball.RbBall.position);
        cell.SetBall(ball);
        ball.name = "Attach";
    }

    private List<HexCell> GetBallGroup(Ball ball)
    {
        HexCell cell = _fieldGrid.GetCellFromPosition(ball.RbBall.position);

        List<HexCell> ballGroupCells = new List<HexCell>();
        ballGroupCells.Add(cell);

        for (int i = 0; i < ballGroupCells.Count; i++)
        {
            foreach (HexCell neighbor in ballGroupCells[i].Neighbors)
            {
                if (neighbor == null || neighbor.GetBall() == null)
                    continue;

                if (!ballGroupCells.Contains(neighbor) && neighbor.GetBall()._typeId == cell.GetBall()._typeId)
                {
                    ballGroupCells.Add(neighbor);
                }
            }
        }

        return ballGroupCells;
    }

    private void TryDestroyBallGroup(Ball ball)
    {
        List<HexCell> ballGroupCells = GetBallGroup(ball);

        if (ballGroupCells.Count < 2)
            return;

        foreach (HexCell c in ballGroupCells)
        {
            Destroy(c.GetBall().gameObject, 0.3f);
        }
    }
}
