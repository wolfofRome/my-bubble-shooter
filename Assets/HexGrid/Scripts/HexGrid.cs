using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Van.HexGrid
{
    public class HexGrid : MonoBehaviour
    {
        [Header("General")]
        [SerializeField] private int _width;
        [SerializeField] private int _height;
        [SerializeField] private HexCell _cellPrefab;
        
        [Header("Debug")]
        [SerializeField] private bool _debugMode = true;
        [SerializeField] private Text _cellLabelPrefab;

        private HexCell[] _cells;
        private Canvas _gridCanvas;

        public int Width
        {
            get { return _width; }
        }
        public int Height
        {
            get { return _height; }
        }

        public HexCell[] Cells
        {
            get { return _cells; }
        }
        
        
        private void Awake()
        {
            _gridCanvas = GetComponentInChildren<Canvas>();

            _cells = new HexCell[_width * _height];

            int number = 0;
            for(int y = 0; y < _height; y++)
            {
                for(int x = 0; x<_width; x++)
                {
                    CreateCell(x, y, number);
                    number++;
                }
            }
        }

        public HexCell GetCellFromPosition(Vector2 position)
        {
            position = transform.InverseTransformPoint(position.x, position.y, 0);
            HexCoordinates cellCoordinates = HexCoordinates.FromPosition(position);
            return _cells.FirstOrDefault(c => c.Coordinates.X == cellCoordinates.X && c.Coordinates.Y == cellCoordinates.Y);
        }

        private void CreateCell(int x, int y, int number)
        {
            Vector2 position = new Vector2((x + y * 0.5f - y / 2) * (HexMetrics.INNER_RADIUS * 2f), y * (HexMetrics.OUTER_RADIUS * 1.5f));

            HexCell cell = Instantiate(_cellPrefab);
            _cells[number] = cell;
            cell.transform.SetParent(transform, false);
            cell.transform.localPosition = position;
            cell.Coordinates = HexCoordinates.FromOffsetCoordinates(x, y);

            if (x > 0)
                cell.SetNeighbor(HexDirection.W, _cells[number - 1]);

            if(y > 0)
            {
                if ((y & 1) == 0)
                {
                    cell.SetNeighbor(HexDirection.SE, _cells[number - _width]);
                    if (x > 0)
                        cell.SetNeighbor(HexDirection.SW, _cells[number - _width - 1]);
                }
                else
                {
                    cell.SetNeighbor(HexDirection.SW, _cells[number - _width]);
                    if (x < _width - 1)
                        cell.SetNeighbor(HexDirection.SE, _cells[number - _width + 1]);
                }
            }

            if (_debugMode)
            {
                Text label = Instantiate(_cellLabelPrefab);
                label.rectTransform.SetParent(_gridCanvas.transform, false);
                label.rectTransform.anchoredPosition = position;
                label.text = cell.Coordinates.ToStringOnSeparateLine();
            }
        }
    }
}

