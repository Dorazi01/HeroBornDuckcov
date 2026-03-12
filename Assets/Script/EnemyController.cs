using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    [Header("Status")]
    public int enemyHp = 100;

    int bulletDamage = 35;
    public Slider hpSlider;

    [Header("AI")]
    Transform player;     // 🛑 플레이어 (자동으로 찾을 것임)
    NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        // 플레이어가 있고, 살아있다면 무조건 추적
        if (player != null)
        {
            agent.SetDestination(player.position);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            enemyHp -= bulletDamage;
            if (hpSlider != null) hpSlider.value = enemyHp;

            if (enemyHp <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}