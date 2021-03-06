﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    public bool isAlive = true;

    [Header("Public References")]
    [Space(15)]
    public Animator baseAnim;
    public Rigidbody body;
    public SpriteRenderer shadowSprite;
    public SpriteRenderer baseSprite;

    [Header("Stats Variables")]
    [Space(15)]
    public float maxLife = 100.0f;
    public float currentLife;
    public float speed = 2;

    protected Vector3 frontVector;

    [Header("Functionality Variables")]
    [Space(15)]
    public bool isGrounded;

    public bool CanBeHit()
    {
        return isAlive && !isKnockedOut;
    }

    public virtual bool CanWalk()
    {
        return true;
    }


    [Header("Battle Variables")]
    public AttackData normalAttack;

    protected Coroutine knockdownRoutine;
    public bool isKnockedOut;


    protected virtual void Start()
    {
        currentLife = maxLife;
        isAlive = true;
        baseAnim.SetBool("isAlive", isAlive);
    }


    public virtual void Update()
    {
        Vector3 shadowSpritePosition = shadowSprite.transform.position;
        shadowSpritePosition.y = 0;
        shadowSprite.transform.position = shadowSpritePosition;
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.name == "Floor")
        {
            isGrounded = true;
            baseAnim.SetBool("isGrounded", isGrounded);
            DidLand();
        }
    }

    protected virtual void OnCollisionExit(Collision collision)
    {
        if (collision.collider.name == "Floor")
        {
            isGrounded = false;
            baseAnim.SetBool("isGrounded", isGrounded);
        }
    }

    protected virtual void DidLand()
    {
    }


    public void FlipSprite(bool isFacingLeft)
    {
        if (isFacingLeft)
        {
            frontVector = new Vector3(-1, 0, 0);
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            frontVector = new Vector3(1, 0, 0);
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    public virtual void Attack()
    {
        baseAnim.SetTrigger("Attack");
    }

    public virtual void TakeDamage(float value, Vector3 hitVector, bool knockdown = false)
    {
        FlipSprite(hitVector.x > 0);
        currentLife -= value;

        if(isAlive && currentLife <= 0)
        {
            Die();
        } else if (knockdown)
        {
            if(knockdownRoutine == null)
            {
                Vector3 pushbackVector = (hitVector + Vector3.up * 0.75f).normalized;
                body.AddForce(pushbackVector * 250);
                knockdownRoutine = StartCoroutine(KnockdownRoutine());
            }
        } else
        {
            baseAnim.SetTrigger("isHurt");
        }
    }

    protected virtual void Die()
    {
        if(knockdownRoutine != null)
        {
            StopCoroutine(knockdownRoutine);
        }
        isAlive = false;
        baseAnim.SetBool("isAlive", isAlive);
        StartCoroutine(DeathFlicker());
    }

    protected virtual void SetOpacity(float value)
    {
        Color color = baseSprite.color;
        color.a = value;
        baseSprite.color = color;
    }

    private IEnumerator DeathFlicker()
    {
        int i = 5;
        while (i > 0)
        {
            SetOpacity(0.5f);
            yield return new WaitForSeconds(0.1f);
            SetOpacity(1.0f);
            yield return new WaitForSeconds(0.1f);
            i--;
        }
    }


    public virtual void DidHitObject(Collider collider, Vector3 hitPoint, Vector3 hitVector)
    {
        Actor actor = collider.GetComponent<Actor>();
        if(actor != null && actor.CanBeHit() && collider.tag != gameObject.tag)
        {
            if(collider.attachedRigidbody != null)
            {
                HitActor(actor, hitPoint, hitVector);
            }
        }
    }

    protected virtual void HitActor(Actor actor, Vector3 hitPoint, Vector3 hitVector)
    {
        actor.EvaluateAttackData(normalAttack, hitVector, hitPoint);
    }

    public virtual void FaceTarget(Vector3 targetPoint)
    {
        FlipSprite(transform.position.x - targetPoint.x > 0);
    }

    public virtual void EvaluateAttackData(AttackData data, Vector3 hitVector, Vector3 hitPoint)
    {
        body.AddForce(data.force * hitVector);
        TakeDamage(data.attackDamage, hitVector,data.knockdown);
    }


    public void DidGetUp()
    {
        isKnockedOut = false;
    }

    protected virtual IEnumerator KnockdownRoutine()
    {
        isKnockedOut = true;
        baseAnim.SetTrigger("Knockdown");
        yield return new WaitForSeconds(1.0f);
        baseAnim.SetTrigger("GetUp");
        knockdownRoutine = null;
    }
}

[System.Serializable]
public class AttackData
{
    public float attackDamage = 10;
    public float force = 50;
    public bool knockdown = false;
}


