using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorScript : MonoBehaviour
{
    public Transform destination;
    public float speed;
    public float waitTime;
    private bool activated,moved, ready;

    private void Start()
    {
        ready = false;
        activated = false;
        moved = false;
    }

    private void Update()
    {

        if (ready && activated && !moved)
        {
            Move();
        }
    }
    private void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, destination.position, Time.deltaTime * speed);
        if(transform.position == destination.position)
        {
            moved = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.transform.SetParent(transform);
        }

        activated = true;

        Invoke("setReady", waitTime);
    }

    void setReady()
    {
        ready = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.transform.SetParent(null);
        }
    }

}
