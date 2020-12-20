using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

enum FighterState
{
    IDLE = 0,
    RUN = 1,
    AIR = 2,
    PUSH = 3,
    CLIMB_IDLE = 4,
    CLIMB_MOVE = 5
}

[DisallowMultipleComponent]
[RequireComponent(typeof(FighterController))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class FighterAnimator : NetworkBehaviour
{
    [SerializeField] string animStateName = "state";

    FighterController controller;
    Animator anim;
    Rigidbody2D rbody;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<FighterController>();
        anim = GetComponent<Animator>();
        rbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isServer)
        {
            HandleAnimation();
        }
    }

    [Server]
    void HandleAnimation()
    {
        /*
         * on wall --> climbing
         * in air? --> air
         * not in air and velocityX > 0 --> run
         * not in air and velocityX = 0 --> idle
         * not in air and velocity X = 0 and pushing --> push        
         * 
         */
        float epsilon = 0.01f;
        if (controller.isOnWall)
        {
            if (Mathf.Abs(rbody.velocity.y) > epsilon)
            {
                ChangeState(FighterState.CLIMB_MOVE);
            }
            else
            {
                ChangeState(FighterState.CLIMB_IDLE);
            }
        } 
        else 
        {
            if (!controller.isGrounded) // air
            {
                ChangeState(FighterState.AIR);
            } else {
                if (true) // is the character pushing
                {
                    if (System.Math.Abs(rbody.velocity.x) >= epsilon) // run
                    {
                        ChangeState(FighterState.RUN);
                    }
                    else if (System.Math.Abs(rbody.velocity.x) < epsilon) // idle
                    {
                        ChangeState(FighterState.IDLE);
                    }
                }
                else
                {

                }
            }
        }
    }

    [Server]
    void ChangeState(FighterState s)
    {
        if (anim.GetInteger(animStateName) != (int)s)
        {
            anim.SetInteger(animStateName, (int)s);
        }
    }

}
