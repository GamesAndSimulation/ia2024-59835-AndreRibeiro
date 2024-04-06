using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LevelScript levelScript = GameObject.Find("EventSystem").GetComponent<LevelScript>();
            levelScript.triggerFinalLine();
        }
    }
}
