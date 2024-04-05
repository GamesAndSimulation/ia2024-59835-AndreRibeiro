using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script is used for the levels cutscenes, events, dialogs and animations
public class LevelScript : MonoBehaviour
{
    [Header("General")]
    public AudioSystem audioSystem;
    public GameObject player;
    private Phase currentPhase;

    [Header("Phase1")]
    public GameObject part1;
    public Collider endCollider;
    public GameObject backdoor;
    public GameObject frontdoor;


    public enum Phase
    {
        phase1,
        phase2
    }

    void checkForShortcut()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            player.transform.position = endCollider.transform.position;
        }
    }

    void Start()
    {
        phase1Start();
    }

    void Update()
    {
        checkForShortcut();
    }

    void phase1Start()
    {
        currentPhase = Phase.phase1;
        part1.SetActive(true);
        audioSystem.PlayMusicByIndex(0);
        Invoke("playFirstVoiceLine", 5f);
    }

    void playFirstVoiceLine()
    {
        if(currentPhase == Phase.phase1)
            audioSystem.PlayVoiceLineByIndex(0);
    }

    public void phase1End()
    {
        currentPhase = Phase.phase2;
        endCollider.enabled = false;
        backdoor.GetComponent<Animator>().SetTrigger("backdoorTrigger");
        audioSystem.PlaySFXByName("door");
        audioSystem.StopMusicByIndex(0);
        audioSystem.PlayMusicByIndex(1);
        audioSystem.PlayVoiceLineByIndex(1);
        Invoke("phase2Start", 18f);
    }

    void phase2Start()
    {
        part1.SetActive(false);
        frontdoor.GetComponent<Animator>().SetTrigger("frontdoorTrigger");
        audioSystem.PlaySFXByName("door");
    }


}
