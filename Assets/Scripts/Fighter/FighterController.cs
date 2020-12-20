using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

// states for easy tracking
enum ClimbInputStates
{
    UP,
    DOWN,
    OFF,
    STOP
}

[DisallowMultipleComponent]
[RequireComponent(typeof(FighterAnimator))]
[RequireComponent(typeof(Rigidbody2D))]
public class FighterController : NetworkBehaviour
{
    // facing
    [Header("Direction")]
    [SyncVar] public bool isFacingRight;

    // moving
    [Header("Running")]
    public float speed;
    [Range(0, 1)] public float airSpeedSlowFactor = 0.7f;

    // climbing a wall
    [Header("Climbing")]
    [SyncVar] public bool isOnWall;
    public float climbSpeed;
    public Vector2 leapFromWallSpeed;
    // wall jump cooldown
    [SyncVar] public double lastLeapFromWall;
    public double leapFromWallCooldown;
    public Vector2 wallCheckOffset;
    public Vector2 wallCheckBoxSize;
    public LayerMask climbableLayer;
    public float gravityScale;

    // jump
    [Header("Jumping")]
    public float leap;
    [SyncVar] public bool isGrounded;
    public Vector2 groundCheckOffset;
    public Vector2 groundCheckBoxSize;
    public LayerMask groundLayer;
    // jump cooldown
    public double jumpCooldown;
    [SyncVar] public double lastJumped;

    Rigidbody2D rbody;

    // Start is called before the first frame update
    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
        rbody.gravityScale = gravityScale; // set the gravity initially
    }

    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer) {
            HandleClimb();
            HandleRun();
            HandleJump();
            HandlePush();
        }
    }

    void FixedUpdate()
    {
        if (isServer)
        {
            WallCheck(); // WallCheck must come before GroundCheck
            GroundCheck();
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireCube(transform.position + (Vector3)(groundCheckOffset), groundCheckBoxSize);
        Gizmos.color = isOnWall ? Color.green : Color.red;
        Gizmos.DrawWireCube(transform.position + (Vector3)(wallCheckOffset), wallCheckBoxSize);
    }

    // climbing
    [Client]
    void HandleClimb()
    {
        if(isOnWall)
        {
            float hor = Input.GetAxis("Horizontal");
            float ver = Input.GetAxis("Vertical");

            // no need to check further if there is no input
            if (!(Mathf.Abs(hor) > 0 || Mathf.Abs(ver) > 0)) {
                // to prevent gliding in case of no input, but a previous input was given
                if (Mathf.Abs(rbody.velocity.y) > 0)
                {
                    CmdClimb(ClimbInputStates.STOP);
                }

                return;
            }

            double elapsed = NetworkTime.time - lastLeapFromWall;

            if (isGrounded) // on wall and also on the ground (in a corner)
            {
                if (isFacingRight)
                {
                    if (hor > 0)
                    {
                        CmdClimb(ClimbInputStates.UP);
                    } else if (hor < 0 && elapsed >= leapFromWallCooldown)
                    {
                        CmdClimb(ClimbInputStates.OFF);
                    }
                }
                else
                {
                    if (hor > 0 && elapsed >= leapFromWallCooldown)
                    {
                        CmdClimb(ClimbInputStates.OFF);
                    } else if (hor < 0)
                    {
                        CmdClimb(ClimbInputStates.UP);
                    }
                }
            }
            else // hovering on a wall
            {
                if (Mathf.Abs(ver) > 0 && elapsed >= leapFromWallCooldown)
                {
                    CmdClimb(ClimbInputStates.OFF);
                }
                else
                {
                    if (isFacingRight)
                    {
                        if (hor > 0)
                        {
                            CmdClimb(ClimbInputStates.UP);
                        }
                        else if (hor < 0)
                        {
                            CmdClimb(ClimbInputStates.DOWN);
                        }
                    }
                    else
                    {
                        if (hor > 0)
                        {
                            CmdClimb(ClimbInputStates.DOWN);
                        } else if (hor < 0)
                        {
                            CmdClimb(ClimbInputStates.UP);
                        }
                    }
                }
            }
        }
    }

    [Command]
    void CmdClimb(ClimbInputStates c)
    {
        if (c == ClimbInputStates.STOP) {
            if (Mathf.Abs(rbody.velocity.y) > 0)
            {
                rbody.velocity = new Vector2(rbody.velocity.x, 0f);
            }
        } else if (c == ClimbInputStates.OFF) // jump off the wall depending on which direction the wall is
        {
            if (NetworkTime.time - lastLeapFromWall >= leapFromWallCooldown)
            {
                if (isFacingRight)
                {
                    // jump left
                    rbody.velocity = new Vector2(leapFromWallSpeed.x * -1, leapFromWallSpeed.y);
                }
                else
                {
                    // jump right
                    rbody.velocity = new Vector2(leapFromWallSpeed.x, leapFromWallSpeed.y);
                }

                // instant resets
                isOnWall = false;
                lastLeapFromWall = NetworkTime.time;
            }
        } else if (c == ClimbInputStates.UP)
        {
            rbody.velocity = new Vector2(rbody.velocity.x, climbSpeed);

        } else if (c == ClimbInputStates.DOWN)
        {
            rbody.velocity = new Vector2(rbody.velocity.x, -1 * climbSpeed);
        }
    }

    [Server]
    void WallCheck() // handles flipping to face the wall
    {
        /*
         * climbing takes priority over running and jumping
         * UNLESS also on the ground        
         * check a circle covering each side of the player model to see if it intersects with a wall
         * if the circle intersects with a wall, set isOnWall to true
         * when on a wall:
         * - gravity is disabled        
         * - moving towards wall (W or D) goes up
         * - moving down (S key) causes the player to descend   
         * - moving away from wall causes the player to leap off the wall        
         */
        Vector3 boxCenter = transform.position + (Vector3)(wallCheckOffset);
        if (Physics2D.OverlapBox(boxCenter, wallCheckBoxSize, 0f, climbableLayer))
        {
            Debug.Log("touched");
            isOnWall = true;
            rbody.gravityScale = 0f; // disable gravity, stop velocity

            // flip the character to face the wall
            Vector3 rightBoxPos = boxCenter + new Vector3(wallCheckBoxSize.x / 4, 0);
            Vector2 rightBoxSize = new Vector2(wallCheckBoxSize.x / 2, wallCheckBoxSize.y);
            bool isWallOnRight = Physics2D.OverlapBox(rightBoxPos, rightBoxSize, 0, climbableLayer);

            if (isWallOnRight && !isFacingRight)
            {
                isFacingRight = true;
                var t = transform.localScale;
                t.x = 1;
                transform.localScale = t;
            }
            else if (!isWallOnRight && isFacingRight)
            {
                isFacingRight = false;
                var t = transform.localScale;
                t.x = -1;
                transform.localScale = t;
            }
        }
        else
        {
            isOnWall = false;
            rbody.gravityScale = gravityScale; // enable gravity
        }
    }

    // pushing
    [Client]
    void HandlePush()
    {

    }

    // running
    [Client]
    void HandleRun()
    {
        if (!isOnWall && NetworkTime.time - lastLeapFromWall >= leapFromWallCooldown)
        {
            float hor = Input.GetAxis("Horizontal");
            if (Mathf.Abs(hor) > 0f)
            {
                CmdRun(hor);
            }
        }
    }
    [Command]
    void CmdRun(float hor)
    {
        if (!isOnWall && NetworkTime.time - lastLeapFromWall >= leapFromWallCooldown)
        {
            if (Mathf.Abs(hor) <= 1)
            {
                Vector2 vel = new Vector2(hor * speed, rbody.velocity.y);
                if (!isGrounded)
                {
                    vel.x *= airSpeedSlowFactor;
                }

                rbody.velocity = vel;

                // update sprite facing direction
                if (hor > 0 && !isFacingRight)
                {
                    Flip();
                }
                else if (hor < 0 && isFacingRight)
                {
                    Flip();
                }
            }
        }
    }
    [Server]
    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 current = transform.localScale;
        current.x *= -1;
        transform.localScale = current;
    }

    // jumping
    [Server]
    void GroundCheck()
    {
        if (Physics2D.OverlapBox((Vector2)transform.position + groundCheckOffset, groundCheckBoxSize, 0, groundLayer))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    [Client]
    void HandleJump()
    {
        if (!isOnWall && IsThereRoomAboveHead())
        {
            if (Input.GetAxis("Vertical") > 0f && isGrounded && NetworkTime.time - lastJumped >= jumpCooldown)
            {
                CmdJump();
            }
        }
    }
    [Command]
    void CmdJump()
    {
        if (!isOnWall && isGrounded && IsThereRoomAboveHead() && NetworkTime.time - lastJumped >= jumpCooldown)
        {
            rbody.velocity = new Vector2(rbody.velocity.x, leap);
            isGrounded = false;
            lastJumped = NetworkTime.time;
        }
    }

    bool IsThereRoomAboveHead()
    {
        return !Physics2D.OverlapBox((Vector2)transform.position + (groundCheckOffset * Vector2.down), groundCheckBoxSize, 0, groundLayer);
    }
}
