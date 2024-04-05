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

    [Header("Phase2")]
    public GameObject part2;
    public Collider hqCollider;
    public Collider forkCollider;
    public Collider libEntrance;
    public Collider libEnd;

    [Header("Phase3")]
    public GameObject part3;
    public GameObject phase3Pos;

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
        part2.SetActive(false);
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
        part2.SetActive(true);
        frontdoor.GetComponent<Animator>().SetTrigger("frontdoorTrigger");
        audioSystem.PlaySFXByName("door");
    }

    void triggerForkVoiceline()
    {
        forkCollider.enabled = false;
        //TODO: audioSystem.PlayVoiceLineByIndex(2); add voice line
    }

    void triggerLibEntrance()
    {
        libEntrance.enabled = false;
        //TODO: audioSystem.PlayVoiceLineByIndex(3); add voice line
    }

    void triggerLibEnd()
    {
        libEnd.enabled = false;
        //TODO: audioSystem.PlayVoiceLineByIndex(4); add voice line
    }

    public void phase2End()
    {
        audioSystem.StopMusicByIndex(1);
        part2.SetActive(false);
    }

    public void phase3Start()
    {
        phase2End();
        part3.SetActive(true);
        player.transform.position = phase3Pos.transform.position;
    }


}
