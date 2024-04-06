using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardScript : MonoBehaviour
{

    public GameObject player;
    public Collider hazardCollider;
    public GameObject electrictyPE;
    private AudioSource audioSource;
    private bool active;
    private bool damagecooldown;

    //weird fix for the phase 2 hazard AudioSource error
    public LevelScript.Phase phase;
    private bool phase2Started;
    private LevelScript levelScript;

    public int resetTime;

    // Start is called before the first frame update
    void Start()
    { 

        player = GameObject.Find("Player").gameObject;
        damagecooldown = false;
        audioSource = electrictyPE.GetComponent<AudioSource>();
        levelScript = GameObject.Find("EventSystem").GetComponent<LevelScript>();
        if(phase == LevelScript.Phase.phase1)
            StartHazard();
    }

    private void Update()
    {
        if (!phase2Started && phase == LevelScript.Phase.phase2 && levelScript.currentPhase == LevelScript.Phase.phase2)
        {
            phase2Started = true;
            StartHazard();
            
        }
        if (active && hazardCollider.bounds.Contains(player.transform.position))
        {
            DamagePlayer();
        }
    }
    void StartHazard()
    {
        active = true;
        electrictyPE.GetComponent<ParticleSystem>().Play();
        audioSource.Play();
        Invoke("StopHazard", resetTime);
    }

    void StopHazard()
    {
        active = false;
        electrictyPE.GetComponent<ParticleSystem>().Stop();
        audioSource.Stop();
        if (phase == levelScript.currentPhase)
            Invoke("StartHazard", resetTime);
    }

    void DamagePlayer()
    {
        Debug.Log("Player inbouds");
        if (!damagecooldown)
        {
            player.GetComponent<PlayerVariables>().TakeDamage(5);
            damagecooldown = true;
            Invoke("ResetDamageCooldown", 0.1f);
        }
    }

    void ResetDamageCooldown()
    {
        damagecooldown = false;
    }


}
