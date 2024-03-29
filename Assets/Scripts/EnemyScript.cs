using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyScript : MonoBehaviour
{

    private NavMeshAgent navAgent;

    public Transform playerTransform;
    public LayerMask groundMask, playerMask;
    public int sightRange;
    public int attackRange;
    public int patrolRadius;

    public int patrolSpeed;
    public int chaseSpeed;

    private bool hasPatrolPoint, foundPlayer;

    public int health;
    
    // Start is called before the first frame update
    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.Find("Player").transform;
        playerMask = LayerMask.GetMask("Player");
    }

    // Update is called once per frame
    void Update()
    {
        StateHandler();
    }

    void StateHandler()
    {
        bool playerInSight = Physics.CheckSphere(transform.position, sightRange, playerMask);
        bool playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerMask);

        if(health <= 0)
        {
            Die();
        }
        if (!playerInSight && !playerInAttackRange)
        {
            Patrol();
        }
        if (foundPlayer || (playerInSight && !playerInAttackRange))
        {
            Chase();
        }
        if (playerInSight && playerInAttackRange)
        {
            Attack();
        }

    }

    void Die()
    {
        Destroy(gameObject);
    }

    void Patrol()
    {
        navAgent.speed = patrolSpeed;
        if (!hasPatrolPoint)
        {
            getPatrolPoint();
        }
        else
        {
            goToPatrolPoint();
        }
    }

    void getPatrolPoint()
    {
        Vector3 direction = UnityEngine.Random.insideUnitSphere * patrolRadius;
        direction += transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(direction, out hit, patrolRadius, 1))
        {
            hasPatrolPoint = true;
            navAgent.SetDestination(hit.position);
        }
    }

    void goToPatrolPoint()
    {
        if (Vector3.Distance(transform.position, navAgent.destination) <= 1)
        {
            hasPatrolPoint = false;
        }
    }

    void Chase()
    {
        foundPlayer = true;
        navAgent.speed = chaseSpeed;
        navAgent.SetDestination(playerTransform.position);
    }

    void Attack()
    {
        navAgent.SetDestination(transform.position);
        // TODO: Attack player
    }

    public int TakeDamage(int damage)
    {
       foundPlayer = true;
       return health -= damage;
    }

}
