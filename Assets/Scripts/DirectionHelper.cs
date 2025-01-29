using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DirectionHelper
{
    static Quaternion[] rotations =
    {
        Quaternion.identity,
        Quaternion.Euler(0, 180, 0),
        Quaternion.Euler(0, 90, 0),
        Quaternion.Euler(0, 270, 0)
    };
    public static Quaternion GetRotation(Direction direction)
    {
        return rotations[(int)direction];
    }
}

public enum Direction
{
    North,
    South,
    East,
    West
}
