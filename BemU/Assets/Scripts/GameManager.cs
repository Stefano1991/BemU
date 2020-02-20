using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public Hero actor;
    public bool cameraFollows = true;
    public CameraBounds cameraBounds;

    // Start is called before the first frame update
    void Start()
    {
        cameraBounds.SetXPosition(cameraBounds.minVisibleX);
    }

    // Update is called once per frame
    void Update()
    {
        if(cameraFollows)
        {
            cameraBounds.SetXPosition(actor.transform.position.x);
        }
    }
}
