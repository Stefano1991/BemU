using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HeroDetector : MonoBehaviour
{

    public bool heroIsNearby;

    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Hero"))
        {
            heroIsNearby = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Hero"))
        {
            heroIsNearby = false;
        }
    }
}
