using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DirectionHelper
{
    static Quaternion[] rotations =
    {
        Quaternion.identity,
        Quaternion.Euler(0, 90, 0),
        Quaternion.Euler(0, 180, 0),
        Quaternion.Euler(0, 270, 0)
    };

    static Vector3[] halfVectors =
    {
        Vector3.forward * 0.5f,
        Vector3.right * 0.5f,
        Vector3.back * 0.5f,
        Vector3.left * 0.5f
    };

    public static Quaternion GetRotation(this Direction direction)
    {
        return rotations[(int)direction];
    }

    public static DirectionChange GetDirectionChangeTo(this Direction current, Direction next)
    {
        if (current == next) return DirectionChange.None;
        else if (current + 1 == next || current - 3 == next) return DirectionChange.TurnRight;
        else if (current - 1 == next || current + 3 == next) return DirectionChange.TurnLeft;
        else return DirectionChange.TurnAround;
    }

    public static float GetAngle(this Direction direction)
    {
        return (float)direction * 90;
    }

    public static Vector3 GetHalfVector(this Direction direction)
    {
        return halfVectors[(int)direction];
    }
}

public enum Direction
{
    North,
    East,
    South,
    West
}

public enum DirectionChange
{
    None,
    TurnRight,
    TurnLeft,
    TurnAround
}
