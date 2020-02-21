using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{

    public Animator baseAnim;
    public Rigidbody body;
    public SpriteRenderer shadowSprite;

    public float speed = 2;
    public float walkSpeed = 2;
    public float runSpeed = 5f;

    bool isRunning;
    bool isMoving;
    float lastWalk;
    public bool canRun = true;
    float tapAgainToRunTime = 0.2f;
    Vector3 lastWalkVector;


    Vector3 currentDir;
    bool isFacingLeft;
    protected Vector3 frontVector;


    bool isJumpLandAnim;
    bool isJumpingAnim;

    public InputHandler input;

    public float jumpForce = 1750f;
    public float jumpDuration = 0.2f;
    public float lastJumpTime;

    public bool isGrounded;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        isJumpLandAnim = baseAnim.GetCurrentAnimatorStateInfo(0).IsName("jump_land");
        isJumpingAnim = baseAnim.GetCurrentAnimatorStateInfo(0).IsName("jump_rise") || baseAnim.GetCurrentAnimatorStateInfo(0).IsName("jump_fall");

        float h = input.GetHorizontalAxis();
        float v = input.GetVerticalAxis();

        bool jump = input.GetJumpButtonDown();


        currentDir = new Vector3(h, 0, v);
        currentDir.Normalize();

        if (v == 0 && h == 0)
        {
            Stop();
            isMoving = false;
        } else if (!isMoving && (v != 0 || h != 0))
        {
            isMoving = true;
            float dotProduct = Vector3.Dot(currentDir, lastWalkVector);
            
            if(canRun && Time.time < lastWalk + tapAgainToRunTime && dotProduct > 0)
            {
                Run();
            } else
            {
                Walk();

                if(h != 0)
                {
                    lastWalkVector = currentDir;
                    lastWalk = Time.time;
                }
            }
        }

        if (jump && !isJumpLandAnim && (isGrounded || (isJumpingAnim && Time.time < lastJumpTime +jumpDuration)))
        {
            Jump(currentDir);
        }


        Vector3 shadowSpritePosition = shadowSprite.transform.position;
        shadowSpritePosition.y = 0;
        shadowSprite.transform.position = shadowSpritePosition;
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
        baseAnim.SetFloat("Speed", speed);
    }

    public void Run()
    {
        speed = runSpeed;
        isRunning = true;
        baseAnim.SetBool("isRunning", isRunning);
        baseAnim.SetFloat("Speed", speed);
    }

    void Jump(Vector3 direction)
    {
        if (!isJumpingAnim) {     baseAnim.SetTrigger ("Jump");
            lastJumpTime = Time.time;
 
            Vector3 horizontalVector = new Vector3(direction.x, 0, direction.z) * speed * 40;
            body.AddForce(horizontalVector,ForceMode.Force);
        }
        Vector3 verticalVector = Vector3.up * jumpForce * Time.deltaTime;
        body.AddForce(verticalVector,ForceMode.Force); 
    }


    void OnCollisionEnter(Collision collision) {
        if (collision.collider.name == "Floor")
        {
            isGrounded = true;
            baseAnim.SetBool("isGrounded", isGrounded);
            DidLand();
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.collider.name == "Floor")
        {
            isGrounded = false;
            baseAnim.SetBool("isGrounded", isGrounded);
        }
    } 

    void DidLand()
    {
        Walk();
    }

    private void FixedUpdate()
    {
        Vector3 moveVector = currentDir * speed;

        if (isGrounded)
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


    public void FlipSprite(bool isFacingLeft)
    {
        if(isFacingLeft)
        {
            frontVector = new Vector3(-1, 0, 0);
            transform.localScale = new Vector3(-1, 1, 1);
        } else
        {
            frontVector = new Vector3(1, 0, 0);
            transform.localScale = new Vector3(1, 1, 1);
        }
    }
}
