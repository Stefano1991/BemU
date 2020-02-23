using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    public bool isAlive = true;
    [Space(15)]
    public Animator baseAnim;
    public Rigidbody body;
    public SpriteRenderer shadowSprite;
    public SpriteRenderer baseSprite;

    [Space(15)]
    public float maxLife = 100.0f;
    public float currentLife;
    public float speed = 2;



    protected Vector3 frontVector;

    [Space(15)]
    public bool isGrounded;

    public bool CanBeHit()
    {
        return isAlive;
    }


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

    public virtual void TakeDamage(float value, Vector3 hitVector)
    {
        FlipSprite(hitVector.x > 0);
        currentLife -= value;

        if(isAlive && currentLife <= 0)
        {
            Die();
        } else
        {
            baseAnim.SetTrigger("isHurt");
        }
    }

    protected virtual void Die()
    {
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
        if(actor != null && actor.CanBeHit())
        {
            if(collider.attachedRigidbody != null)
            {
                HitActor(actor, hitPoint, hitVector);
            }
        }
    }

    protected virtual void HitActor(Actor actor, Vector3 hitPoint, Vector3 hitVector)
    {
        actor.TakeDamage(10,hitVector);
    }
}
