using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    Transform model;

    GameTile tileFrom, tileTo;
    Vector3 positionFrom, positionTo;
    float progress = 0;

    Direction direction;
    DirectionChange directionChange;
    float directionAngleFrom, directionAngleTo;
    public void GameUpdate()
    {
        progress += Time.deltaTime;
        if (progress > 1)
        {
            if (tileTo.NextOnPath == null)
            {
                FindObjectOfType<Game>().DestroyEnemy(this);
                return;
            }
            tileFrom = tileTo;
            tileTo = tileFrom.NextOnPath;
            progress = 0;
            PrepareNextState();
        }
        if (directionChange == DirectionChange.None)
        {
            transform.localPosition = Vector3.Lerp(positionFrom, positionTo, progress);
        }
        else
        {
            float angle = Mathf.LerpUnclamped(directionAngleFrom, directionAngleTo, progress);
            transform.localRotation = Quaternion.Euler(0, angle, 0);
        }
    }

    public void OnSummon(GameTile tile)
    {
        tileFrom = tile;
        tileTo = tile.NextOnPath;
        
        PrepareIntro();
        progress = 0;
    }

    void PrepareIntro()
    {
        positionFrom = tileFrom.transform.localPosition;
        positionTo = tileFrom.ExitPoint;
        direction = tileFrom.PathDirection;
        directionChange = DirectionChange.None;
        directionAngleFrom = directionAngleTo = direction.GetAngle();
        transform.localRotation = direction.GetRotation();
    }

    void PrepareNextState()
    {
        positionFrom = positionTo;
        positionTo = tileFrom.ExitPoint;
        directionChange = direction.GetDirectionChangeTo(tileFrom.PathDirection);
        direction = tileFrom.PathDirection;
        directionAngleFrom = directionAngleTo;

        switch (directionChange)
        {
            case DirectionChange.None: PrepareForward(); break;
            case DirectionChange.TurnRight: PrepareTurnRight(); break;
            case DirectionChange.TurnLeft: PrepareTurnLeft(); break;
            default: PrepareTurnAround(); break;
        }
    }
    void PrepareForward()
    {
        transform.localRotation = direction.GetRotation();
        directionAngleTo = direction.GetAngle();
        model.localPosition = Vector3.zero;
    }
    void PrepareTurnRight()
    {
        directionAngleTo = directionAngleFrom + 90;
        model.localPosition = new Vector3(-0.5f, 0, 0);
        transform.localPosition = positionFrom + direction.GetHalfVector();
    }
    void PrepareTurnLeft()
    {
        directionAngleTo = directionAngleFrom - 90;
        model.localPosition = new Vector3(0.5f, 0, 0);
        transform.localPosition = positionFrom + direction.GetHalfVector();
    }
    void PrepareTurnAround()
    {
        directionAngleTo = directionAngleFrom + 180;
        model.localPosition = Vector3.zero;
        transform.localPosition = positionFrom;
    }
}
