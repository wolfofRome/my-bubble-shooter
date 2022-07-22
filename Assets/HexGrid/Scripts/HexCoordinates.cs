using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Van.HexGrid
{
    [System.Serializable]
    public struct HexCoordinates
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public HexCoordinates (int x, int y)
        {
            X = x;
            Y = y;
        }

        public static HexCoordinates FromOffsetCoordinates(int x, int y)
        {
            return new HexCoordinates(x, y);
        }

        public override string ToString()
        {
            return "(" + X.ToString() + ";" + Y.ToString() + ")";
        }

        public string ToStringOnSeparateLine()
        {
            return "X(" + X.ToString() + ")" + "\n" + "Y(" + Y.ToString() + ")";
        }
    }
}

