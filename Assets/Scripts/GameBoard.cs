using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    [SerializeField] Transform ground;
    [SerializeField] GameTile tilePrefab;


    Vector2Int _size;

    GameTile[] tiles;

    Queue<GameTile> SearchFrontier = new Queue<GameTile>();

    List<GameTile> spawnPoints = new List<GameTile>();
    public int spawnCount => spawnPoints.Count;

    Ray TouchRay => Camera.main.ScreenPointToRay(Input.mousePosition);
    public LayerMask tileMask;
    public void Initialize(Vector2Int size)
    {
        this._size = size;
        ground.localScale = new Vector3(size.x, size.y, 1f);

        Vector2 offset = new Vector2((size.x - 1) * 0.5f, (size.y - 1) * 0.5f);

        tiles = new GameTile[size.x * size.y];
        for (int i = 0, y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++, i++)
            {
                GameTile tile = Instantiate(tilePrefab);
                tiles[i] = tile;
                if (x > 0)
                {
                    GameTile.MakeEastWestConnection(tile, tiles[i - 1]);
                }
                if (y > 0)
                {
                    GameTile.MakeNorthSouthConnection(tile, tiles[i - size.x]);
                }
                tile.transform.SetParent(transform);
                tile.transform.localPosition = new Vector3(x - offset.x, 0, y - offset.y);

                tile.isAlternative = (x & 1) == 0;
                if((y&1)==0)
                {
                    tile.isAlternative = !tile.isAlternative; 
                }
            }
        }
        //tiles[0].SetType(GameTileContentType.EnemySpawn);
        //spawnPoints.Add(tiles[0]);
        //tiles[tiles.Length / 2].SetType(GameTileContentType.Destination);
        //tiles[tiles.Length / 2].BecomeDestination();
        FindPaths();
    }


    public void OnTileChanged()
    {
        FindPaths();
    }

    void FindPaths()
    {
        foreach (GameTile _tile in tiles)
        {
            _tile.ClearPath();
            if (_tile.Type == GameTileContentType.Destination)
            {
                _tile.BecomeDestination();
                SearchFrontier.Enqueue(_tile);
            }
        }
        //int r = Random.Range(0, tiles.Length);
        //tiles[tiles.Length / 2].SetType(GameTileContentType.Destination);
        //tiles[tiles.Length/2].BecomeDestination();
        //SearchFrontier.Enqueue(tiles[tiles.Length / 2]);

        while (SearchFrontier.Count > 0)
        {
            GameTile tile = SearchFrontier.Dequeue();
            if(tile != null)
            {
                if(tile.isAlternative)
                {
                    SearchFrontier.Enqueue(tile.GrowPathNorth());
                    SearchFrontier.Enqueue(tile.GrowPathSouth());
                    SearchFrontier.Enqueue(tile.GrowPathEast());
                    SearchFrontier.Enqueue(tile.GrowPathWest());
                }
                else
                {
                    SearchFrontier.Enqueue(tile.GrowPathWest());
                    SearchFrontier.Enqueue(tile.GrowPathEast());
                    SearchFrontier.Enqueue(tile.GrowPathSouth());
                    SearchFrontier.Enqueue(tile.GrowPathNorth());
                }

            }
        }
        foreach (var tile in tiles)
        {
            tile.ShowPath();
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleTouch();
        }
        if (Input.GetMouseButtonDown(1))
        {
            HandleTouch2();
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            foreach (GameTile _tile in tiles)
            {
                _tile.TogglePath(false);
            }
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            foreach (GameTile _tile in tiles)
            {
                _tile.TogglePath(true);
            }
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            HandleTower();
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            HandleMortar();
        }
    }

    void HandleTouch()
    {
        if(Physics.Raycast(TouchRay, out RaycastHit hit,Mathf.Infinity,tileMask))
        {
            GameTile tile = hit.collider.GetComponent<GameTile>();
            if (tile != null)
            {
                if (tile.Type == GameTileContentType.Wall)
                    tile.SetType(GameTileContentType.Empty);
                else if(tile.Type == GameTileContentType.Empty)
                    tile.SetType(GameTileContentType.Wall);
            }
        }
        else
        {
            print("no touch");
        }
    }
    void HandleTouch2()
    {
        if (Physics.Raycast(TouchRay, out RaycastHit hit, Mathf.Infinity, tileMask))
        {
            GameTile tile = hit.collider.GetComponent<GameTile>();
            if (tile != null)
            {
                if(Input.GetKey(KeyCode.LeftShift))
                {
                    if (tile.Type == GameTileContentType.EnemySpawn)
                    {
                        tile.SetType(GameTileContentType.Empty);
                        spawnPoints.Remove(tile);
                    }
                    else if (tile.Type == GameTileContentType.Empty)
                    {
                        tile.SetType(GameTileContentType.EnemySpawn);
                        spawnPoints.Add(tile);
                    }
                }
                else
                {
                    if (tile.Type == GameTileContentType.Destination)
                        tile.SetType(GameTileContentType.Empty);
                    else if (tile.Type == GameTileContentType.Empty)
                        tile.SetType(GameTileContentType.Destination);
                }
            }
        }
        else
        {
            print("no touch");
        }
    }

    void HandleTower()
    {
        if (Physics.Raycast(TouchRay, out RaycastHit hit, Mathf.Infinity, tileMask))
        {
            GameTile tile = hit.collider.GetComponent<GameTile>();
            if (tile != null)
            {
                if (tile.Type == GameTileContentType.Tower)
                    tile.SetType(GameTileContentType.Empty);
                else if (tile.Type == GameTileContentType.Empty)
                    tile.SetType(GameTileContentType.Tower);
                else if (tile.Type == GameTileContentType.Wall)
                    tile.SetType(GameTileContentType.Tower);
            }
        }
        else
        {
            print("no touch");
        }
    }

    void HandleMortar()
    {
        if (Physics.Raycast(TouchRay, out RaycastHit hit, Mathf.Infinity, tileMask))
        {
            GameTile tile = hit.collider.GetComponent<GameTile>();
            if (tile != null)
            {
                if (tile.Type == GameTileContentType.Tower)
                    tile.SetType(GameTileContentType.Empty);
                else if (tile.Type == GameTileContentType.Empty)
                    tile.SetType(GameTileContentType.Mortar);
                else if (tile.Type == GameTileContentType.Wall)
                    tile.SetType(GameTileContentType.Mortar);
            }
        }
        else
        {
            print("no touch");
        }
    }

    public List<GameTile> GetSpawnPoints()
    {
        return spawnPoints; 
    }

    public GameTile GetSpawnPointByIndex(int index)
    {
        return spawnPoints[index];
    }
}


public enum GameTileContentType
{
    Empty,
    Wall,
    Destination,
    EnemySpawn,
    Tower,
    Mortar
}
