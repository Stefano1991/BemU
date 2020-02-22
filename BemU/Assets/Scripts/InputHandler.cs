using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{

    float horizontal;
    float vertical;
    bool jump;

    float lastJumpTime;
    bool isJumping;
    public float maxJumpDuration = 0.2f;

    bool attack;

    public float GetVerticalAxis()
    {
        return vertical;
    }
    public float GetHorizontalAxis()
    {
        return horizontal;
    }
    public bool GetJumpButtonDown()
    {
        return jump;
    }
    public bool GetAttackButtonDown()
    {
        return attack;
    }


    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        attack = Input.GetButtonDown("Attack");

        if(!jump && !isJumping && Input.GetButton("Jump"))
        {
            jump = true;
            lastJumpTime = Time.time;
            isJumping = true;
        } else if (!Input.GetButton("Jump"))
        {
            jump = false;
            isJumping = false;
        }

        if(jump && Time.time > lastJumpTime + maxJumpDuration)
        {
            jump = false;
        }
    }
}
