using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SocialPlatforms;

public class EnemyScript : MonoBehaviour
{

    [Header("Movement")]
    private NavMeshAgent navAgent;

    public Transform playerTransform;
    public LayerMask groundMask, playerMask;
    public int sightRange;
    public int attackRange;
    public int patrolRadius;
    public int rotationSpeed;
    public int rotationOffset;

    public int patrolSpeed;
    public int chaseSpeed;

    private bool hasPatrolPoint, foundPlayer;

    [Header("Combat")]
    private bool shooting, reloading, readyToShoot;
    private int bulletsLeft, bulletsShot;
    public int magazineSize, reloadTime;
    public float timeBetweenShots, timeBetweenShooting, inaccuracy;    
    public LineRenderer laser;
    public Transform muzzle;

    public int health;

    [Header("Animation")]
    public Animator anim;
    public GameObject explosionPE;
    
    // Start is called before the first frame update
    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.Find("Player").transform;
        playerMask = LayerMask.GetMask("Player");

        readyToShoot = true;
        bulletsLeft = magazineSize;
        laser.enabled = false;

        anim = GetComponent<Animator>();
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

        if (health <= 0)
        {
            Die();
        }
        else
        {
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

    }

    void Die()
    {
        Vector3 explosionTransform = new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z);
        Instantiate(explosionPE, explosionTransform, Quaternion.identity);
        Destroy(gameObject);
    }

    void Patrol()
    {
        anim.SetBool("Patrolling", true);
        anim.SetBool("Chasing", false);
        anim.SetBool("Attacking", false);
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
        anim.SetBool("Patrolling", false);
        anim.SetBool("Chasing", true);
        anim.SetBool("Attacking", false);

        navAgent.speed = chaseSpeed;
        navAgent.SetDestination(playerTransform.position);
    }

    void Attack()
    {
        anim.SetBool("Patrolling", false);
        anim.SetBool("Chasing", false);
        anim.SetBool("Attacking", true);
        //stops
        navAgent.SetDestination(transform.position);

        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
        lookRotation *= Quaternion.Euler(0, rotationOffset, 0);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

        shooting = true;

        if (bulletsLeft == 0 && !reloading)
        {
            Reload();
        }

        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot++;
            Shoot();
        }

    }

    void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }

    void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        reloading = false;
    }

    void Shoot()
    {
        readyToShoot = false;

        Vector3 direction = playerTransform.position - muzzle.position;

        direction += UnityEngine.Random.insideUnitSphere * inaccuracy;

        RaycastHit hit;

        laser.enabled = true;
        laser.SetPosition(0, muzzle.position);
        laser.SetPosition(1, muzzle.position + direction * attackRange);

        if (Physics.Raycast(muzzle.position, direction, out hit, playerMask))
        {
            if (hit.collider.CompareTag("Player"))
            {
                //hit.transform.GetComponent<PlayerMovement>().TakeDamage();
            }
        }

        bulletsLeft--;
        Invoke("ResetShot", timeBetweenShooting);
        if (bulletsShot > 0 && bulletsLeft > 0)
        {
            Invoke("Shoot", timeBetweenShots);
            bulletsShot--;
        }
    }

    void ResetShot()
    {
        readyToShoot = true;
        laser.enabled = false;
        laser.SetPosition(0, muzzle.position);
        laser.SetPosition(1, muzzle.position);
    }

    public int TakeDamage(int damage)
    {
       foundPlayer = true;
       return health -= damage;
    }

}
