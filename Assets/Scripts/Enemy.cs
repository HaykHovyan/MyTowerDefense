using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    GameTile tileFrom, tileTo;
    Vector3 positionFrom, positionTo;
    float progress = 0;
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
            positionFrom = positionTo;
            positionTo = tileFrom.ExitPoint;
            transform.localRotation = DirectionHelper.GetRotation(tileFrom.PathDirection);
            progress = 0;
        }
        transform.localPosition = Vector3.Lerp(positionFrom, positionTo, progress);
    }
    public void OnSummon(GameTile tile)
    {
        tileFrom = tile;
        tileTo = tile.NextOnPath;
        positionFrom = tileFrom.transform.localPosition;
        positionTo = tileFrom.ExitPoint;
        transform.localRotation = DirectionHelper.GetRotation(tileFrom.PathDirection);
        progress = 0;
    }
}
