using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Transform orientation;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;

    [Header("Movement")]
    public float walkSpeed;
    public float sprintSpeed;
    public float slideSpeed;
    public float wallRunSpeed;
    private float moveSpeed;


    //for momentum calc
    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;

    public float speedIncreaseMultiplier;
    public float slopeIncreaseMultiplier;

    public float groundDrag;
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("Ground Detection")]
    public float playerHeight;
    public LayerMask groundMask;
    bool grounded;

    [Header("Slope Detection")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;

    private float horizontalInput;
    private float verticalInput;

    private Vector3 moveDirection;

    Rigidbody rb;

    public MovementState state;

    public bool sliding;

    private float defaultSprintDiff;

    public bool wallrunning;

    [Header("Sound")]
    private Sound_PlayerMovement audioHandler;

    

    public enum MovementState
    {
        walking,
        sprinting,
        airborne,
        sliding,
        wallrunning
    }


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
        defaultSprintDiff = Mathf.Abs(sprintSpeed - walkSpeed);
        audioHandler = GetComponent<Sound_PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        groundCheck();
        MyInput();
        SpeedControl();
        StateHandler();

        if (grounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
        }
        //debug position
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void StateHandler()
    {

        //Mode - Wallrunning
        if (wallrunning)
        {
            state = MovementState.wallrunning;
            desiredMoveSpeed = wallRunSpeed;
            audioHandler.PlayFootsteps();
        }

        // Mode - Sliding
        if (sliding)
        {
            state = MovementState.sliding;
            audioHandler.PlaySlide();

            if (OnSlope() && rb.velocity.y < 0.1f)
                desiredMoveSpeed = slideSpeed;

            else
                desiredMoveSpeed = sprintSpeed;
        }

        /* Mode - Sprinting
        else if (grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            desiredMoveSpeed = sprintSpeed;
        }*/

        // Mode - Walking
        else if (grounded)
        { 
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
            audioHandler.PlayFootsteps();
        }

        // Mode - Air
        else
        {
            state = MovementState.airborne;
        }

        if (!wallrunning)
        {
            rb.useGravity = !OnSlope();
        }
        
        if (Mathf.Abs(desiredMoveSpeed - lastDesiredMoveSpeed) > defaultSprintDiff && moveSpeed != 0)
        {
            StopAllCoroutines();
            StartCoroutine(SmoothLerpMoveSpeed());
        }
        else
        {
            moveSpeed = desiredMoveSpeed;
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;
    }

    private IEnumerator SmoothLerpMoveSpeed()
    {
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        while(time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);

            if (OnSlope())
            {
                float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                float slopeAngleIncrease = 1 + (slopeAngle/ 90f);
                time += Time.deltaTime * speedIncreaseMultiplier * slopeIncreaseMultiplier * slopeAngleIncrease;
            }
            else
            {
                time += Time.deltaTime * speedIncreaseMultiplier;
            }

            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        if(Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;


        if(OnSlope())
        {
            rb.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed * 20f, ForceMode.Force);

            if(rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }

        else if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }

        else
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }

        rb.useGravity = !OnSlope();
    }

    private void groundCheck()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundMask);
    }

    private void SpeedControl()
    {

        if (OnSlope())
        {
            if(rb.velocity.magnitude > moveSpeed)
            {
                rb.velocity = rb.velocity.normalized * moveSpeed;
            }
        }

        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);

            if(flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }

        }
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }


    public bool OnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.2f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }

}
