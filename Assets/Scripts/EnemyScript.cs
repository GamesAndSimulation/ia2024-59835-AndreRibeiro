using UnityEngine;
using UnityEngine.AI;

public class EnemyScript : MonoBehaviour
{

    [Header("Movement")]
    private NavMeshAgent navAgent;

    public Transform playerTransform;
    public LayerMask groundMask, playerMask, wallMask;
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
    public float timeBetweenShots, timeBetweenShooting, innacuracy;    
    public LineRenderer laser;
    public Transform muzzle;

    public int health;

    [Header("Animation")]
    public Animator anim;
    public GameObject explosionPE;

    [Header("Sound")]
    public AudioSource sfxAudioSrc;
    public AudioSource voiceAudioSrc;
    public AudioClip shootSound, reloadSound, explosionSound;
    public AudioClip[] footstepSFX;
    public AudioClip[] voices;
    private float nextStepTime;
    private bool hasSpottedVoiceLine, hasAttackedVoiceLine;

    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.Find("Player").transform;
        playerMask = LayerMask.GetMask("Player");

        readyToShoot = true;
        bulletsLeft = magazineSize;
        laser.enabled = false;

        anim = GetComponent<Animator>();

        hasAttackedVoiceLine = false;
        hasSpottedVoiceLine = false;
    }

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
        GameObject explosionInstance = Instantiate(explosionPE, explosionTransform, Quaternion.identity);
        AudioSource explosionSource = explosionInstance.AddComponent<AudioSource>();
        PlayExplosionSound(explosionSource);
        Destroy(explosionInstance, explosionSound.length + 2f);
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
            PlayFootsteps("patrol");
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
            Vector3 rotation = new Vector3(0, rotationOffset, 0);
            transform.LookAt(hit.position + rotation);
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
        PlayFootsteps("chase");
        PlaySpottedVoice();
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

        PlayAttackVoice();
    }

    void Reload()
    {
        reloading = true;
        PlayReloadSound();
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
        
        direction += UnityEngine.Random.insideUnitSphere * innacuracy;

        RaycastHit hit;

        if(Physics.Raycast(muzzle.position, direction, out hit, attackRange, groundMask))
        {
            laser.enabled = true;
            laser.SetPosition(0, muzzle.position);
            laser.SetPosition(1, hit.point);
        }
        else if(Physics.Raycast(muzzle.position, direction, out hit, attackRange, wallMask))
        {
            laser.enabled = true;
            laser.SetPosition(0, muzzle.position);
            laser.SetPosition(1, hit.point);
        }
        else if (Physics.Raycast(muzzle.position, direction, out hit, attackRange, playerMask))
        {
            laser.enabled = true;
            laser.SetPosition(0, muzzle.position);
            laser.SetPosition(1, muzzle.position + direction*attackRange);
            if (hit.collider.CompareTag("Player"))
            {
                hit.transform.GetComponent<PlayerVariables>().TakeDamage(10);
            }
        }

        PlayShootSound();

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

    //Sounds

    private void PlayFootsteps(string state)
    {
        float vel;

        if (state.Equals("patrol"))
        {
            vel = patrolSpeed;
        }
        else
        {
            vel = chaseSpeed;
        }
        float stepInterval = Mathf.Lerp(0.5f, 0.2f, vel / 20);

        if (vel > 0.1f && Time.time > nextStepTime)
        {
            sfxAudioSrc.clip = footstepSFX[Random.Range(0, footstepSFX.Length)];
            sfxAudioSrc.volume = 0.05f;
            sfxAudioSrc.Play();
            nextStepTime = Time.time + stepInterval;
        }
    }

    private void PlaySpottedVoice()
    {
        if (!hasSpottedVoiceLine)
        {
            voiceAudioSrc.clip = voices[Random.Range(0, 4)];
            voiceAudioSrc.Play();
            hasSpottedVoiceLine = true;
            Invoke("ResetSpottedVoice", 30);
        }
    }

    private void ResetSpottedVoice()
    {
        hasSpottedVoiceLine = false;
    }

    private void PlayAttackVoice()
    {
        if (!hasAttackedVoiceLine)
        {
            voiceAudioSrc.clip = voices[Random.Range(5, 7)];
            voiceAudioSrc.Play();
            hasAttackedVoiceLine = true;
            Invoke("ResetAttackVoice", 30);
        }
    }

    private void ResetAttackVoice()
    {
        hasAttackedVoiceLine = false;
    }

    private void PlayShootSound()
    {
        sfxAudioSrc.clip = shootSound;
        sfxAudioSrc.volume = 0.2f;
        sfxAudioSrc.Play();
    }

    private void PlayReloadSound()
    {
        sfxAudioSrc.clip = reloadSound;
        sfxAudioSrc.volume = 0.2f;
        sfxAudioSrc.Play();
    }

    private void PlayExplosionSound(AudioSource src)
    {
        src.clip = explosionSound;
        src.volume = 0.2f;
        src.Play();
    }
}
