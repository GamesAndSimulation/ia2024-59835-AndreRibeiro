using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSystem : MonoBehaviour
{

    public int damage;
    public float timeBetweenShooting, spread, range, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    private int bulletsLeft, bulletsShot;

    public LineRenderer laser;

    private bool shooting, readyToShoot, reloading;

    public Camera fpsCam;
    public Transform attackPoint;
    public RaycastHit rayHit;
    public LayerMask enemyMask, targetMask;

    public CameraShaker cameraShaker;
    public float shakeDuration, shakeMagnitude;

    //public AudioSource audioSrc;
    public AudioSystem AudioSystem;

    // Start is called before the first frame update
    void Start()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;
        laser.enabled = false;
        AudioSystem = GameObject.Find("AudioSystem").GetComponent<AudioSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        MyInput();
        KeepLaserOnPoint();
    }

    void KeepLaserOnPoint()
    {
       laser.SetPosition(0, attackPoint.position);
    }


    void MyInput()
    {
        if (allowButtonHold)
        {
            shooting = Input.GetKey(KeyCode.Mouse0);
        } else { 
            shooting = Input.GetKeyDown(KeyCode.Mouse0);
        }

        if(Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading)
        {
            Reload();
        }

        if(readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = bulletsPerTap;
            Shoot();
        }

    }

    void Reload()
    {
        reloading = true;
        AudioSystem.PlaySFXByName("player_reload_sfx"); 
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

        Vector3 direction = fpsCam.transform.forward;

        laser.enabled = true;
        laser.SetPosition(0, attackPoint.position);
        laser.SetPosition(1, attackPoint.position + direction * range);

        if(Physics.Raycast(fpsCam.transform.position, direction, out rayHit, range, enemyMask))
        {

            if(rayHit.collider.CompareTag("Enemy"))
            {
                rayHit.collider.GetComponent<EnemyScript>().TakeDamage(damage);
            }

        }

        else if (Physics.Raycast(fpsCam.transform.position, direction, out rayHit, range, targetMask))
        {
            if(rayHit.collider.CompareTag("Target"))
            {
                rayHit.collider.GetComponent<TargetScript>().TakeDamage(damage);
            }
        }

        StartCoroutine(cameraShaker.Shake(shakeDuration, shakeMagnitude));
        AudioSystem.PlaySFXByName("player_shoot_sfx");

        bulletsLeft--;
        Invoke("ResetShoot", timeBetweenShooting);

        if(bulletsShot > 0 && bulletsLeft > 0)
        {
            Invoke("Shoot", timeBetweenShots);
            bulletsShot--;
        }
    }

    void ResetShoot()
    {
        readyToShoot = true;
        laser.enabled = false;
        laser.SetPosition(0, attackPoint.position);
        laser.SetPosition(1, attackPoint.position);
    }

}
