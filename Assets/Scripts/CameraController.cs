﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;

    private Vector3 offset;
    private Camera m_cam;
    private float yaw = 0.0f;
    private float pitch = 0.0f;
    public float speedH = 5.0f;
    public float speedV = 5.0f;
    public float camFollowPlayerLerpTerm = 6.4f;

    public float pitchMin = -80f;
    public float pitchMax = 80f;
    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - player.transform.position;
        m_cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        //print(camFollowPlayerLerpTerm * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, player.transform.position + offset, camFollowPlayerLerpTerm * Time.deltaTime);

        yaw += speedH * Input.GetAxis("Mouse X");
        pitch -= speedV * Input.GetAxis("Mouse Y");       
        //the rotation range
        pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);
        //Debug.Log("ptich: " + pitch);
        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
    }
}
