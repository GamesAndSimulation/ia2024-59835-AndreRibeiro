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
    public float wallJumpUpForce;
    public float wallJumpSideForce;
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
    public FirstPersonScript fpsCam;
    private Rigidbody rb;
    private PlayerMovement playerMovement;

    [Header("Exiting")]
    public float exitWallTime;
    private bool exitingWall;
    private float exitWallTimer;

    [Header("Gravity")]
    public bool useGravity;
    public float gravityCounterForce;

    [Header("Input")]
    public KeyCode jump = KeyCode.Space;

  

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

    private void StateMachine()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");


        //wallrunning
        if ((wallLeft || wallRight) && verticalInput > 0 && AboveGround() && !exitingWall)
        {
            if (!playerMovement.wallrunning)
            {
                StartWallRun();
            }

            if(wallRunTimer > 0)
            {
                wallRunTimer -= Time.deltaTime;
            }
            
            if(wallRunTimer <= 0 && playerMovement.wallrunning)
            {
                exitingWall = true;
                exitWallTimer = exitWallTime;
            }

            if (Input.GetKeyDown(jump))
            {
                WallJump();
            }
        }

        //walljumping
        else if (exitingWall)
        {
            if(playerMovement.wallrunning)
            {
                StopWallRun();
            }

            if (exitWallTimer > 0)
            {
                exitWallTimer -= Time.deltaTime;
            }
            else
            {
                exitingWall = false;
            }
        }

        //none
        else
        {
            if (playerMovement.wallrunning)
            {
                StopWallRun();
            }
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


    private void StartWallRun()
    {
        playerMovement.wallrunning = true;
        wallRunTimer = maxWallRunTime;
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        fpsCam.DoFov(fpsCam.getCurrentFOV() + 10f);
        if (wallLeft) fpsCam.DoTilt(-5f);
        if (wallRight) fpsCam.DoTilt(5f);

    }

    private void WallRun()
    {
        rb.useGravity = useGravity;

        Vector3 wallNormal = wallRight ? rightWH.normal : leftWH.normal;

        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
        {
            wallForward = -wallForward;
        }

        //foward force
        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);

        //pushing the player towards the wall
        if(!(wallLeft && horizontalInput > 0) && !(wallRight && horizontalInput < 0))
        {
            rb.AddForce(-wallNormal * 100, ForceMode.Force);
        }

        if(useGravity)
        {
            rb.AddForce(transform.up * gravityCounterForce, ForceMode.Force);
        }
    }

    private void StopWallRun()
    {
        playerMovement.wallrunning = false;

        fpsCam.DoFov(fpsCam.getCurrentFOV() - 10f);
        fpsCam.DoTilt(0f);
    }

    public void WallJump()
    {

        exitingWall = true;
        exitWallTimer = exitWallTime;

        Vector3 wallNormal = wallRight ? rightWH.normal : leftWH.normal;

        Vector3 resForce = transform.up * wallJumpUpForce + wallNormal * wallJumpSideForce;

        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(resForce, ForceMode.Impulse);
    }

}
