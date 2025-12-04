using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Configurações")]
    public GameObject enemyPrefab;   
    public Transform[] spawnPoints;  
    
    public float spawnInterval = 5f;
    public int maxEnemies = 10;    
    
    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            int currentEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;

            if (currentEnemies < maxEnemies)
            {
                SpawnEnemy();
            }

            timer = 0f;
        }
    }

    void SpawnEnemy()
    {
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[randomIndex];

        Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
    }
}