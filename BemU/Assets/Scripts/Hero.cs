using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Hero : Actor
{
    [Space(15)]
    public InputHandler input;

    public float walkSpeed = 2;
    public float runSpeed = 5f;

    public bool isRunning;
    bool isMoving;
    float lastWalk;
    public bool canRun = true;
    float tapAgainToRunTime = 0.2f;
    Vector3 lastWalkVector;

    Vector3 currentDir;
    bool isFacingLeft;

    bool isJumpLandAnim;
    bool isJumpingAnim;

    public float jumpForce = 1750f;
    public float jumpDuration = 0.2f;
    public float lastJumpTime;


    bool isAttackingAnim;
    float lastAttackTime;
    float attackLimit = 0.14f;

    /// <summary>
    /// Hero entrance
    /// </summary>
    public Walker walker;
    public bool isAutoPiloting;
    public bool controllable = true;


    [Header("Jump Attack")]
    /// <summary>
    /// Jump Attack
    /// </summary>
    public bool canJumpAttack = true;
    private int currentAttackChain = 1;
    public int evaluatedAttackChain = 0;
    public AttackData jumpAttack;
    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        if(!isAlive)
        {
            return;
        }

        isAttackingAnim = baseAnim.GetCurrentAnimatorStateInfo(0).IsName("attack1");
        isJumpLandAnim = baseAnim.GetCurrentAnimatorStateInfo(0).IsName("jump_land");
        isJumpingAnim = baseAnim.GetCurrentAnimatorStateInfo(0).IsName("jump_rise") || baseAnim.GetCurrentAnimatorStateInfo(0).IsName("jump_fall");

        if(isAutoPiloting)
        {
            return;
        }

        float h = input.GetHorizontalAxis();
        float v = input.GetVerticalAxis();
        bool jump = input.GetJumpButtonDown();
        bool attack = input.GetAttackButtonDown();

        currentDir = new Vector3(h, 0, v);
        currentDir.Normalize();


        if (!isAttackingAnim)
        {
            if (v == 0 && h == 0)
            {
                Stop();
                isMoving = false;
            }
            else if (!isMoving && (v != 0 || h != 0))
            {
                isMoving = true;
                float dotProduct = Vector3.Dot(currentDir, lastWalkVector);



                if (canRun && Time.time < lastWalk + tapAgainToRunTime && dotProduct > 0)
                {
                    Run();
                }
                else
                {
                    Walk();

                    if (h != 0)
                    {
                        lastWalkVector = currentDir;
                        lastWalk = Time.time;
                    }
                }
            } else if (isMoving && Input.GetButton("Run") )
            {
                Run();
            } else if(isMoving && Input.GetButtonUp("Run"))
            {
                Walk();
            }

        }

        if(jump && !isJumpLandAnim && !isAttackingAnim && (isGrounded || (isJumpingAnim && Time.time < lastJumpTime + jumpDuration)))
        {
            Jump(currentDir);
        }

        if(attack && Time.time >= lastAttackTime + attackLimit)
        {
            lastAttackTime = Time.time;
            Attack();
        }

    }

   

    private void FixedUpdate()
    {
        if (!isAlive)
        {
            return;
        }
        if (!isAutoPiloting)
        {
            Vector3 moveVector = currentDir * speed;

            if (isGrounded && !isAttackingAnim)
            {
                body.MovePosition(transform.position + moveVector * Time.fixedDeltaTime);
                baseAnim.SetFloat("Speed", moveVector.magnitude);
            }

            if (moveVector != Vector3.zero)
            {
                if (moveVector.x != 0)
                {
                    isFacingLeft = moveVector.x < 0;
                }
                FlipSprite(isFacingLeft);
            }
        }
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
        if(collision.collider.name=="Floor")
        {
            canJumpAttack = true;
        }
    }


    public void Stop()
    {
        speed = 0;
        baseAnim.SetFloat("Speed", speed);
        isRunning = false;
        baseAnim.SetBool("isRunning", isRunning);
    }

    public void Walk()
    {
        speed = walkSpeed;
        isRunning = false;
        baseAnim.SetFloat("Speed", speed);
        baseAnim.SetBool("isRunning", isRunning);
    }

    public void Run()
    {
        speed = runSpeed;
        isRunning = true;
        baseAnim.SetBool("isRunning", isRunning);
        baseAnim.SetFloat("Speed", speed);
    }

    public void Jump(Vector3 direction)
    {
        if (!isJumpingAnim)
        {
            baseAnim.SetTrigger("Jump");
            lastJumpTime = Time.time;

            Vector3 horizontalVector = new Vector3(direction.x, 0, direction.z) * speed * 40f;
            body.AddForce(horizontalVector, ForceMode.Force);
        }

        Vector3 verticalVector = Vector3.up * jumpForce * Time.deltaTime;
        body.AddForce(verticalVector, ForceMode.Force);
    }

    protected override void DidLand()
    {
        base.DidLand();
        Walk();
        //Run();
    }

    public override void Attack()
    {

        if (!isGrounded)
        {
            if(isJumpingAnim && canJumpAttack)
            {
                canJumpAttack = false;
                currentAttackChain = 1;
                evaluatedAttackChain = 0;
                baseAnim.SetInteger("EvaluatedChain", evaluatedAttackChain);
                baseAnim.SetInteger("CurrentChain", currentAttackChain);

                body.velocity = Vector3.zero;
                body.useGravity = false;
            }
        } else
        {
            currentAttackChain = 1;
            evaluatedAttackChain = 0;
            baseAnim.SetInteger("EvaluatedChain", evaluatedAttackChain);
            baseAnim.SetInteger("CurrentChain", currentAttackChain);
        }
    }

    public void AnalyzeSpecialAttack(AttackData attackData, Actor actor, Vector3 hitPoints, Vector3 hitVector)
    {
        actor.EvaluateAttackData(attackData, hitVector, hitPoints);
    }

    protected override void HitActor(Actor actor, Vector3 hitPoint, Vector3 hitVector)
    {
        if(baseAnim.GetCurrentAnimatorStateInfo(0).IsName("attack1"))
        {
            base.HitActor(actor, hitPoint, hitVector);
        } else if(baseAnim.GetCurrentAnimatorStateInfo(0).IsName("jump_attack"))
        {
            AnalyzeSpecialAttack(jumpAttack, actor, hitPoint, hitVector);
        }
    }

    public void DidChain(int chain)
    {
        baseAnim.SetInteger("EvaluatedChain", 1);
    }

    public void DidJumpAttack()
    {
        body.useGravity = true;
    }




    public void AnimateTo(Vector3 position, bool shouldRun, Action callback)
    {
        if(shouldRun)
        {
            Run();
        } else
        {
            Walk();
        }

        walker.MoveTo(position, callback);
    }

    public void UseAutopilot(bool useAutopilot)
    {
        isAutoPiloting = useAutopilot;
        walker.enabled = useAutopilot;
    }
}
