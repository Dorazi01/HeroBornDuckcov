using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // 생성할 적 프리팹
    
    public GameObject spawnPoint;

    public float spawnInterval = 3f; // 생성 간격 (초)

    public int hp = 500;
    
    float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnEnemy();
            timer = 0f;
        }
    }

    void SpawnEnemy()
    {
        // 내 위치(transform.position)에 적을 생성
        Instantiate(enemyPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
    }




    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            if (hp > 0)
            {
                hp -= 15;
            }
            else
            {
                GMBehavior.instance.gameProgress += 1;
                Destroy(gameObject);
            }
        }
    }


}
