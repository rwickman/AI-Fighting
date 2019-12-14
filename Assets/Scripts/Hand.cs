using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{

    public float handZOffset = 0.4f;
    public float handViewportPosX = 0.88f;
    public float handViewportPosY = -0.08f;

    void LateUpdate()
    {
        UpdateHandTransform();
    }

    void UpdateHandTransform() {
        Camera mainCam = Camera.main;
 
        transform.position = mainCam.ViewportToWorldPoint(new Vector3(handViewportPosX, handViewportPosY, mainCam.nearClipPlane + handZOffset));
        transform.eulerAngles = mainCam.transform.eulerAngles;
    }
}
