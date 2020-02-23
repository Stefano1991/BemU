using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitForwarder : MonoBehaviour
{

    public Actor actor;
    public Collider triggerCollider;

    private void OnTriggerEnter(Collider other)
    {
        Vector3 direction = new Vector3(other.transform.position.x - actor.transform.position.x, 0, 0);
        direction.Normalize();

        BoxCollider collider = triggerCollider as BoxCollider;
        Vector3 centerPoint = this.transform.position;

        if (collider)
        {
            centerPoint = transform.TransformPoint(collider.center);
        }
        Vector3 startPoint = other.ClosestPointOnBounds(centerPoint);
        actor.DidHitObject(other, startPoint, direction);
    }
}
