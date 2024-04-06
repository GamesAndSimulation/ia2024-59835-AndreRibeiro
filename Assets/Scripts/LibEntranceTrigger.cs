using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LibEntranceTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LevelScript levelScript = GameObject.Find("EventSystem").GetComponent<LevelScript>();
            levelScript.triggerLibEntrance();
        }
    }
}
