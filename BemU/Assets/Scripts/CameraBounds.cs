using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Camera))]
public class CameraBounds : MonoBehaviour
{

    public float minVisibleX;
    public float maxVisibleX;
    private float minValue;
    private float maxValue;
    public float cameraHalfWidth;

    private Camera activeCamera;

    public Transform cameraRoot;

    public Transform leftBounds;
    public Transform rightBounds;


    // Start is called before the first frame update
    void Start()
    {
        activeCamera = Camera.main;

        cameraHalfWidth = Mathf.Abs(activeCamera.ScreenToWorldPoint(new Vector3(0, 0, 0)).x - activeCamera.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x) * 0.5f;

        minValue = minVisibleX + cameraHalfWidth;
        maxValue = maxVisibleX - cameraHalfWidth;

        Vector3 position;
        position = leftBounds.localPosition;
        position.x = transform.localPosition.x - cameraHalfWidth;
        leftBounds.localPosition = position;

        position = rightBounds.localPosition;
        position.x = transform.localPosition.x + cameraHalfWidth;
        rightBounds.localPosition = position;
    }

    public void SetXPosition(float x)
    {
        Vector3 trans = cameraRoot.position;
        trans.x = Mathf.Clamp(x, minValue, maxValue);
        cameraRoot.position = trans;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
