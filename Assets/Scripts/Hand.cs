using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{

    public float handZOffset = 0.4f;
    public float handViewportPosX = 0.88f;
    public float handViewportPosY = -0.08f;
    public Camera handCamera;

    void LateUpdate()
    {
        UpdateHandTransform();
    }

    void UpdateHandTransform() {
        //Camera mainCam = Camera.main;
 
        transform.position = handCamera.ViewportToWorldPoint(new Vector3(handViewportPosX, handViewportPosY, handCamera.nearClipPlane + handZOffset));
        transform.eulerAngles = handCamera.transform.eulerAngles;
    }
}
