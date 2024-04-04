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

    void Awake()
    {
        phase1Start();
    }

    void phase1Start()
    {
        currentPhase = Phase.phase1;
        part1.SetActive(true);
        //TODO: audioSystem.PlayMusicByIndex(0);
    }

    public void phase1End()
    {
        endCollider.enabled = false;
        backdoor.GetComponent<Animator>().SetTrigger("backdoorTrigger");
        //TODO: audioSystem.PlaySFXByName("door");
        //TODO: audioSystem.PlayMusicByIndex(1);
        //TODO: audioSystem.PlayVoiceLineByIndex();
        Invoke("phase2Start", 5f); //TODO: change to voice line length
    }

    void phase2Start()
    {
        part1.SetActive(false);
        frontdoor.GetComponent<Animator>().SetTrigger("frontdoorTrigger");
        currentPhase = Phase.phase2;
    }


}
