using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Van.HexGrid
{
    public class HexCell : MonoBehaviour
    {
        [SerializeField] private HexCoordinates _coordinates;
        [SerializeField] private HexCell[] _neighbors = new HexCell[6];

        public HexCoordinates Coordinates
        {
            get
            {
                return _coordinates;
            }

            set
            {
                _coordinates = value;
            }
        }

        public HexCell GetNeighbor(HexDirection hexDirection)
        {
            return _neighbors[(int)hexDirection];
        }

        public void SetNeighbor(HexDirection hexDirection, HexCell cell)
        {
            _neighbors[(int)hexDirection] = cell;
            cell._neighbors[(int)hexDirection.Opposite()] = this;
        }
    }
}
