using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Camera))]
public class CameraBounds : MonoBehaviour
{

    public float offset;

    public float minVisibleX;
    public float maxVisibleX;
    private float minValue;
    private float maxValue;
    public float cameraHalfWidth;

    public Camera activeCamera;

    public Transform cameraRoot;

    public Transform leftBounds;
    public Transform rightBounds;

    [Header("Game Intro and Ending Animation")]
    [Space(15)]
    public Transform introWalkStart;
    public Transform introWalkEnd;
    public Transform exitWalkEnd;


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

        position = introWalkStart.transform.localPosition;
        position.x = transform.localPosition.x - cameraHalfWidth - 2.0f;
        introWalkStart.transform.localPosition = position;

        position = introWalkEnd.transform.localPosition;
        position.x = transform.localPosition.x - cameraHalfWidth + 2.0f;
        introWalkEnd.transform.localPosition = position;

        position = exitWalkEnd.transform.localPosition;
        position.x = transform.localPosition.x + cameraHalfWidth + 2.0f;
        exitWalkEnd.transform.localPosition = position;
    }

    public void SetXPosition(float x)
    {
        Vector3 trans = cameraRoot.position;
        trans.x = Mathf.Clamp(x + offset, minValue, maxValue);
        cameraRoot.position = trans;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CalculateOffset(float actorPosition)
    {
        offset = cameraRoot.position.x - actorPosition;
        SetXPosition(actorPosition);
        StartCoroutine(EaseOffset());
    }

    public IEnumerator EaseOffset()
    {
        while(offset != 0)
        {
            offset = Mathf.Lerp(offset, 0, 0.1f);
            if(Mathf.Abs(offset) < 0.05f)
            {
                offset = 0;
            }
            yield return new WaitForFixedUpdate();
        }
    }

    public void EnableBounds(bool isEnabled)
    {
        rightBounds.GetComponent<Collider>().enabled = isEnabled;
        leftBounds.GetComponent<Collider>().enabled = isEnabled;
    }
}
