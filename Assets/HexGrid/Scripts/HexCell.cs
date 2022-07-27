using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Van.HexGrid
{
    public class HexCell : MonoBehaviour
    {
        [SerializeField] private HexCoordinates _coordinates;
        [SerializeField] private HexCell[] _neighbors = new HexCell[6];

        private Ball _ball;

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

        public HexCell[] Neighbors
        {
            get { return _neighbors; }
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

        public void SetBall(Ball ball)
        {
            _ball = ball;
            _ball.RbBall.position = gameObject.transform.position;
        }

        public Ball GetBall()
        {
            return _ball;
        }
    }
}