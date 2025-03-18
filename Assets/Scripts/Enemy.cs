using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{ 
    public Transform model;
    GameTile tileFrom, tileTo;
    Vector3 positionFrom, positionTo;
    float progress = 0;
    float progressFactor = 1;
    float speed = 1;

    Direction direction;
    DirectionChange directionChange;
    float directionAngleFrom, directionAngleTo;
    float pathOffset;

    [SerializeField]
    Image healthBar;
    float health;
    float maxHealth;

    public void GameUpdate()
    {
        healthBar.fillAmount = health / maxHealth;
        if (health <= 0)
        {
            FindObjectOfType<Game>().DestroyEnemy(this);
            return;
        }
        progress += Time.deltaTime * progressFactor * speed;
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

    public void OnSummon(GameTile tile, float pathOffset, float speed)
    {
        this.pathOffset = pathOffset;
        this.speed = speed;

        tileFrom = tile;
        tileTo = tile.NextOnPath;
        
        PrepareIntro();
        progress = 0;
        health = transform.localScale.x * 100;
        maxHealth = health;
    }

    void PrepareIntro()
    {
        positionFrom = tileFrom.transform.localPosition;
        positionTo = tileFrom.ExitPoint;
        direction = tileFrom.PathDirection;
        directionChange = DirectionChange.None;
        model.localPosition = new Vector3(pathOffset, 0, 0);
        directionAngleFrom = directionAngleTo = direction.GetAngle();
        transform.localRotation = direction.GetRotation();
        progressFactor = 2;
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
        model.localPosition = new Vector3(pathOffset, 0, 0);
        progressFactor = 1;
    }
    void PrepareTurnRight()
    {
        directionAngleTo = directionAngleFrom + 90;
        model.localPosition = new Vector3(pathOffset - 0.5f, 0, 0);
        transform.localPosition = positionFrom + direction.GetHalfVector();
        progressFactor = 1 / (Mathf.PI * 0.5f * (0.5f - pathOffset));
    }
    void PrepareTurnLeft()
    {
        directionAngleTo = directionAngleFrom - 90;
        model.localPosition = new Vector3(pathOffset + 0.5f, 0, 0);
        transform.localPosition = positionFrom + direction.GetHalfVector();
        progressFactor = 1 / (Mathf.PI * 0.5f * (0.5f - pathOffset));
    }
    void PrepareTurnAround()
    {
        directionAngleTo = directionAngleFrom + 180;
        model.localPosition = new Vector3(pathOffset - 0.5f, 0, 0);
        transform.localPosition = positionFrom;
    }

    public void ApplyDamage(float damage)
    {
        health -= damage;
    }
}
