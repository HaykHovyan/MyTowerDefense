using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTile : MonoBehaviour
{
    [SerializeField] Transform arrow;

    [SerializeField] GameTile north, east, south, west, nextOnPath;
    public GameTile NextOnPath => nextOnPath;
    public Vector3 ExitPoint;
    public Direction PathDirection;
    public bool HasPath => distance != int.MaxValue;
    public bool BlocksPath => Type == GameTileContentType.Wall || Type == GameTileContentType.Tower || Type == GameTileContentType.Mortar;

    public GameTileContentType Type;

    public GameObject WallArt, DestArt, SpawnArt, TowerArt, MortarArt;

    public bool isAlternative;
    int distance;

    private void Start()
    {
    }

    static Quaternion
        northRotation = Quaternion.Euler(90, 0, 0),
        eastRotation = Quaternion.Euler(90, 90, 0),
        southRotation = Quaternion.Euler(90, 180, 0),
        westRotation = Quaternion.Euler(90, 270, 0);
    public static void MakeEastWestConnection(GameTile east, GameTile west)
    {
        west.east = east;
        east.west = west;
    }

    public static void MakeNorthSouthConnection(GameTile north, GameTile south)
    {
        north.south = south;
        south.north = north;
    }

    public void ClearPath()
    {
        distance = int.MaxValue;
        nextOnPath = null;
    }

    public void BecomeDestination()
    {
        distance = 0;
        nextOnPath = null;
        ExitPoint = transform.localPosition;
    }
     
     
    GameTile GrowPathTo(GameTile neighbor, Direction direction)
    {
        if (neighbor == null||neighbor.HasPath||BlocksPath)
            return null;
        neighbor.distance = distance + 1;
        neighbor.nextOnPath = this;
        neighbor.ExitPoint = neighbor.transform.localPosition + direction.GetHalfVector();
        neighbor.PathDirection = direction;
        return neighbor;
    }

    public GameTile GrowPathNorth() => GrowPathTo(north, Direction.South);
    public GameTile GrowPathEast() => GrowPathTo(east, Direction.West);
    public GameTile GrowPathSouth() => GrowPathTo(south, Direction.North);
    public GameTile GrowPathWest() => GrowPathTo(west, Direction.East);


    public void ShowPath()
    {
        if(distance==0)
        {
            arrow.gameObject.SetActive(false);
            return;
        }
        //arrow.gameObject.SetActive(true);
        if (nextOnPath == north)
        {
            arrow.localRotation = northRotation;
        }
        else if (nextOnPath == east)
        {
            arrow.localRotation = eastRotation;
        }
        else if (nextOnPath == south)
        {
            arrow.localRotation = southRotation;
        }
        else
            arrow.localRotation = westRotation;
    }

    public void SetType(GameTileContentType type)
    {
        Type = type;
        WallArt.SetActive(false);
        DestArt.SetActive(false);
        SpawnArt.SetActive(false);
        TowerArt.SetActive(false);
        MortarArt.SetActive(false);
        if (type == GameTileContentType.Wall)
            WallArt.SetActive(true);
        if (type==GameTileContentType.Destination) 
            DestArt.SetActive(true);
        if (type == GameTileContentType.EnemySpawn)
            SpawnArt.SetActive(true); 
        if (type == GameTileContentType.Tower)
            TowerArt.SetActive(true);
        if (type == GameTileContentType.Mortar)
            MortarArt.SetActive(true);
        FindObjectOfType<GameBoard>().OnTileChanged();
    }

    public void TogglePath(bool show)
    {
        if(Type!=GameTileContentType.Destination)
            arrow.gameObject.SetActive(show);
    }
}
