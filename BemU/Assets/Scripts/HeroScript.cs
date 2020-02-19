using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroScript : MonoBehaviour
{

    public Animator baseAnim;
    public Rigidbody body;
    public SpriteRenderer shadowSprite;

    public float speed = 2;
    public float walkSpeed = 2;

    Vector3 currentDir;
    bool isFacingLeft;
    protected Vector3 frontVector;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");


        currentDir = new Vector3(h, 0, v);

        if (v == 0 && h == 0)
        {
            Stop();
        } else if (v != 0 || h != 0)
        {
            Walk();
        }
    }

    public void Stop()
    {
        speed = 0;
        baseAnim.SetFloat("Speed", speed);
    }

    public void Walk()
    {
        speed = walkSpeed;
        baseAnim.SetFloat("Speed", speed);
    }


    private void FixedUpdate()
    {
        Vector3 moveVector = currentDir * speed;
        body.MovePosition(transform.position + moveVector * Time.fixedDeltaTime);
        baseAnim.SetFloat("Speed", moveVector.magnitude);

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
