using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Actor
{
    public Walker walker;
    public bool stopMovementWhenHit = true;
    public EnemyAI ai;
    public static int totalEnemies;


    public void ResgusterEnemy()
    {
        totalEnemies++;
    }

    protected override void Die()
    {
        base.Die();
        ai.enabled = false;
        walker.enabled = false;
        totalEnemies--;
    }

    public void MoveTo(Vector3 targetPosition)
    {
        walker.MoveTo(targetPosition);
    }

    public void MoveToOffset(Vector3 targetPosition, Vector3 offset)
    {
        if (!walker.MoveTo(targetPosition + offset))
        {
            walker.MoveTo(targetPosition - offset);
        }
    }

    public void Wait()
    {
        walker.StopMovement();
    }

    public override void TakeDamage(float value, Vector3 hitVector)
    {
        if(stopMovementWhenHit)
        {
            walker.StopMovement();
        }
        base.TakeDamage(value, hitVector);
    }

    public override bool CanWalk()
    {
        return !baseAnim.GetCurrentAnimatorStateInfo(0).IsName("hurt");
    }
}
