using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Van.HexGrid
{
    public enum HexDirection
    {
        NE,
        E,
        SE,
        SW,
        W,
        NW
    }

    public static class HexDirectionExtensions
    {
        public static HexDirection Opposite(this HexDirection hexDirection)
        {
            return (int)hexDirection < 3 ? (hexDirection + 3) : (hexDirection - 3);
        }
    }
}
