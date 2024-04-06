using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenSphereTrigger : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(GameObject.Find("Player").GetComponent<PlayerVariables>().score == 0)
            {
                LevelScript levelScript = GameObject.Find("EventSystem").GetComponent<LevelScript>();
                levelScript.triggerGreenSphere();
            }
        }
    }

}
