using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] Vector2Int boardSize = new Vector2Int(11,11);

    [SerializeField] GameBoard board;

    [SerializeField, Range(0.1f, 10f)]
    float spawnSpeed = 1f;

    [SerializeField] Enemy enemyPrefab;

    [SerializeField, FloatRangeSlider(0.5f, 2)]
    FloatRange scale = new FloatRange(1f);
    [SerializeField, FloatRangeSlider(-0.4f, 0.4f)]
    FloatRange pathOffset = new FloatRange(0f);
    [SerializeField, FloatRangeSlider(0.5f, 2)]
    FloatRange enemySpeed = new FloatRange(1f);

    List<Enemy> enemies = new List<Enemy>();

    float spawnProgress=0;

    public static bool GameStarted = false;
    private void Awake()
    {
        board.Initialize(boardSize);
    }

    private void OnValidate()
    {
        if(boardSize.x<1)
            boardSize.x = 1;
        if(boardSize.y<1)
            boardSize.y = 1;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {            
            //TODO is there a destination check
            //TODO is there a spawner check
            GameStarted = true;

            print("game start");
        }
        if (board.spawnCount == 0||!GameStarted)
            return;
        spawnProgress += spawnSpeed * Time.deltaTime;
        if(spawnProgress > 1)
        {
            spawnProgress = 0;
            SpawnEnemy();
        }
        foreach (Enemy enemy in enemies) {
            enemy.GameUpdate();
        }
    }

    void SpawnEnemy()
    {
        GameTile spawnPoint = board.GetSpawnPointByIndex(Random.Range(0,board.spawnCount));
        Enemy enemy = Instantiate(enemyPrefab);
        float randomScale = scale.RandomValueInRange;
        enemy.model.localScale = new Vector3(randomScale, randomScale, randomScale);
        enemy.transform.localPosition = spawnPoint.transform.localPosition;
        enemies.Add(enemy);
        enemy.OnSummon(spawnPoint, pathOffset.RandomValueInRange, enemySpeed.RandomValueInRange);
    }

    public void DestroyEnemy(Enemy enemy)
    {
        enemies.Remove(enemy);
        Destroy(enemy.gameObject);
    }
}
