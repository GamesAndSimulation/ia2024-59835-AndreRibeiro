using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableSurfaceBehaviour : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public Transform nextPosition;
    public float speed;
    public float changeTimer;

    void Start()
    {
        nextPosition = pointB;
        ChangeState();
    }

    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, nextPosition.position, Time.deltaTime*speed);
    }

    void ChangeState()
    {
        if(nextPosition == pointA)
        {
            nextPosition = pointB;
        }

        else if(nextPosition == pointB)
        {
            nextPosition = pointA;
        }

        Invoke("ChangeState", changeTimer);
    }

}
