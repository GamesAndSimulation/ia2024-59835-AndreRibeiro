using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackToHQTrigger : MonoBehaviour
{ 
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            LevelScript levelScript = GameObject.Find("EventSystem").GetComponent<LevelScript>();
            levelScript.backToHQ();
        }
    }
}
