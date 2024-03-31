using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingBehaviour : MonoBehaviour
{

    [Header("References")]
    public Transform orientation;
    public Transform playerObj;
    private Rigidbody rb;
    private PlayerMovement playerMovement;

    [Header("Sliding")]
    public float maxSlideTime;
    public float slideForce;
    private float slideTimer;

    public float slideYScale;
    private float startYScale;

    [Header("Input")]
    public KeyCode slideKey = KeyCode.C;
    private float horizontalInput;
    private float verticalInput;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerMovement = GetComponent<PlayerMovement>();

        startYScale = playerObj.localScale.y;
    }

    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(slideKey) && (horizontalInput != 0 || verticalInput != 0) && !playerMovement.state.Equals("airborne"))
        {
            StartSlide();
        }

        if (Input.GetKeyUp(slideKey) && playerMovement.sliding) 
        {
            EndSlide();
        }
    }

    private void FixedUpdate()
    {
        if (playerMovement.sliding)
        {
            SlidingMovement();
        }
    }

    private void StartSlide()
    {
        playerMovement.sliding = true;
        
        playerObj.localScale = new Vector3(playerObj.localScale.x, slideYScale, playerObj.localScale.z);
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

        slideTimer = maxSlideTime;
    }

    private void SlidingMovement()
    {
        Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (!playerMovement.OnSlope() || rb.velocity.y > -0.1f)
        {
            rb.AddForce(inputDir * slideForce, ForceMode.Force);
            slideTimer -= Time.deltaTime;
        }
        else
        {
            rb.AddForce(playerMovement.GetSlopeMoveDirection(inputDir) * slideForce, ForceMode.Force);
        }

        if (slideTimer < 0)
        {
            EndSlide();
        }
    }


    private void EndSlide()
    {
        playerMovement.sliding = false;

        playerObj.localScale = new Vector3(playerObj.localScale.x, startYScale, playerObj.localScale.z);
    }

}
