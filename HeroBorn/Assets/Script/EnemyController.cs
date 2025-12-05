using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using Unity.VisualScripting;

public class EnemyController : MonoBehaviour
{

    [SerializeField] Transform player;
    NavMeshAgent agent;

    int index = 0;

    int enemyHp = 7;

    public List<Transform> patrolPoints = new List<Transform>();

    bool isAttacking = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        //index Clear
        if (index == 3)
        {
            index = 0;
        }



        if (isAttacking)
        {
            agent.SetDestination(player.position);
        }
        else
        {
            //agent.pathPending : 네비메시가 목적지까지 경로를 계산중인지 여부
            //메모리 절약을 위한 경로계산은 한번만.
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                Patrol();
            }
            
            if (Vector3.Distance(transform.position, patrolPoints[index].position) < 1f)
            {
                index++;
                Debug.Log("인덱스 증가 ");
            }
        }
    }


    void Patrol()
    {
        agent.SetDestination(patrolPoints[index].position);
    }
    

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Attack!");
            isAttacking = true;
        }
        if (other.CompareTag("Bullet"))
        {
            enemyHp--;
            //Debug.LogFormat("Enemy Hit! HP: {0}", enemyHp);
            if (enemyHp <= 0)
            {
                //Debug.Log("Enemy Dead!");
                Destroy(gameObject);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Stop Attacking!");
            isAttacking = false;
        }
    } 
}
