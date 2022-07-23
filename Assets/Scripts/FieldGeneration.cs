using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Van.HexGrid;
using UnityEngine.UI;
using System.IO;
using System.Linq;

public class FieldGeneration : MonoBehaviour
{
    [SerializeField] private HexGrid _fieldGrid;
    [SerializeField] private Ball _ballPrefab;
    [SerializeField] private List<BallPrefabType> _ballPrefabs;

    [SerializeField] private string _levelAssetsPath;
    [SerializeField] private TextAsset _level;

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
            Debug.Log(cell.Coordinates);

            Ball ball = Instantiate(_ballPrefab.Prefab, cell.transform.position, Quaternion.identity);
            ball.transform.SetParent(gameObject.transform);

            cellIndex++;
        }
    }
}
