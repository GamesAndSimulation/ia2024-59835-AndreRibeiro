using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunning : MonoBehaviour
{

    [Header("WallRunning")]
    public LayerMask wallMask;
    public LayerMask groundMask;
    public float wallRunForce;
    public float maxWallRunTime;
    public float climbSpeed;
    private float wallRunTimer;

    private float horizontalInput;
    private float verticalInput;

    [Header("Detection")]
    public float wallCheckDistance;
    public float minJumpHeight;
    
    private RaycastHit leftWH;
    private RaycastHit rightWH;
    private bool wallLeft;
    private bool wallRight;

    [Header("References")]
    public Transform orientation;
    private Rigidbody rb;
    private PlayerMovement playerMovement;

    [Header("Input")]
    public KeyCode upwards = KeyCode.LeftShift;
    public KeyCode downwards = KeyCode.C;
    private bool upwardsInput;
    private bool downwardsInput;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckForWall();
        StateMachine();
    }

    void FixedUpdate()
    {
        if(playerMovement.wallrunning)
        {
            WallRun();
        }
    }

    private void CheckForWall()
    {
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWH, wallCheckDistance, wallMask);
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWH, wallCheckDistance, wallMask);
    }

    private bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, groundMask);
    }

    private void StateMachine()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        upwardsInput = Input.GetKey(upwards);
        downwardsInput = Input.GetKey(downwards);

        if((wallLeft || wallRight) && verticalInput > 0 && AboveGround())
        {
            if(!playerMovement.wallrunning)
            {
                StartWallRun();
            }
           
        }

        else
        {
            if (playerMovement.wallrunning)
            {
                StopWallRun();
            }
        }
    }

    private void StartWallRun()
    {
        playerMovement.wallrunning = true;
    }

    private void WallRun()
    {
        rb.useGravity = false;
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        Vector3 wallNormal = wallRight ? rightWH.normal : leftWH.normal;

        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
        {
            wallForward = -wallForward;
        }

        //foward force
        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);

        if(upwardsInput)
        {
            rb.velocity = new Vector3(rb.velocity.x, climbSpeed, rb.velocity.z);
        }
        if(downwardsInput)
        {
            rb.velocity = new Vector3(rb.velocity.x, -climbSpeed, rb.velocity.z);
        }

        //pushing the player towards the wall
        if(!(wallLeft && horizontalInput > 0) && !(wallRight && horizontalInput < 0))
        {
            rb.AddForce(-wallNormal * 100, ForceMode.Force);
        }
    }

    private void StopWallRun()
    {
        playerMovement.wallrunning = false;
    }

}
