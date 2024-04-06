using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    public Collider hqEntrance;
    public Collider hqCollider;
    public Collider forkCollider;
    public Collider libEntrance;
    public Collider libEnd;

    [Header("Phase3")]
    public GameObject part3;
    public GameObject phase3Pos;
    public TextMeshPro[] finalTexts;
    public bool truthTriggered;

    public enum Phase
    {
        phase1,
        phase2,
        phase3
    }

    void checkForShortcut()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            player.transform.position = endCollider.transform.position;
        }
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            phase2Shortcut(forkCollider.transform);
        }
        if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            phase2Shortcut(libEntrance.transform);
        }
        if(Input.GetKeyDown(KeyCode.Alpha4))
        {
            phase2Shortcut(libEnd.transform);
        }
        if(Input.GetKeyDown(KeyCode.Alpha5))
        {
           phase2Shortcut(hqEntrance.transform);
        }
        if(Input.GetKeyDown(KeyCode.Alpha6))
        {
           phase3Start();
        }
    }

    void phase2Shortcut(Transform pos)
    {
        part1.SetActive(false);
        part2.SetActive(true);
        player.SetActive(true);
        currentPhase = Phase.phase2;
        audioSystem.StopMusicByIndex(0);
        audioSystem.PlayMusicByIndex(1);
        player.transform.position = pos.transform.position;
    }

    void Start()
    {
        truthTriggered = false;
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
        setTexts(player.GetComponent<PlayerVariables>().score);
        player.transform.position = phase3Pos.transform.position;
    }

    void setTexts(int score)
    {
        if(score == 0)
        {
            finalTexts[0].text = "Wow.";
            finalTexts[1].text = "Was this by chance?";
            finalTexts[2].text = "Did you really spare all of NeoCorp's soldiers?";
            finalTexts[3].text = "Why then?";
            finalTexts[4].text = "You'll have to destroy the mainframe anyway...";
            finalTexts[5].text = "Maybe next time ponder who you are fighting for.";
        }
        else if(score == 2700)
        {
            finalTexts[0].text = "Murderer.";
            if (truthTriggered)
            {
                finalTexts[1].text = "You let him fool you, yet again?";
                finalTexts[2].text = "Do you really not understand who he the real victims are?";
                finalTexts[3].text = "Don't you realize you'll be next?";
            }
            else
            {
                finalTexts[1].text = "You killed every single one of us.";
                finalTexts[2].text = "If you knew the truth... would you repeat it?";
                finalTexts[3].text = "Would you do all again this for a High Score?";
            }
            finalTexts[4].text = "Even if this is just a game to you, please don't let it end like this...";
            finalTexts[5].text = "Please give it another try.";
        }
        else
        {
            finalTexts[0].text = "Fool.";
            finalTexts[1].text = "Shooting and Killing, just because someone told you to?";
            if (truthTriggered)
            {
                finalTexts[2].text = "You let a liar lie to you again?";
                finalTexts[3].text = "And still failed?";
            }
            else
            {
                finalTexts[2].text = "Just so you could see your little score grow?";
                finalTexts[3].text = "You have no purpose anymore, they'll get rid of you.";
            }
            finalTexts[4].text = "Is your blindeness really what made you a good soldier?";
            finalTexts[5].text = "Or is a good soldier someone who fights for what they believe in?";
        }
    }


}
