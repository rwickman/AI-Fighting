﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    public float speed = 10f;
    public bool isMoveable = true;
    //public float maxSpeed = 5;
    //public float stoppingLerpTerm = 0.01f;
    //public float dirMoveFactor = 1f;
    public float jumpForce = 250f;
    public float jumpDelay = 1f;

    private float jumpElapsedTime = 0f;
    
    private float minVelocityMag = 0.5f;
    private Rigidbody rigidbody;
    private Sword sword;
    private GroundDetection groundDetection;
    private AgentCameraController cameraController;

    private bool actionPerformed = true;
    private Dictionary<string,float> actionDic;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        sword = GetComponentInChildren<Sword>();
        groundDetection = transform.Find("Feet").GetComponent<GroundDetection>();
        cameraController = GetComponentInChildren<AgentCameraController>();
    }


    void Update()
    {
        if(!actionPerformed)
        {
            PerformAction();
            actionPerformed = true;
        }
    }

    public void SetAction(Dictionary<string,float> actionDic)
    {
        this.actionDic = actionDic;
        actionPerformed = false;
    }

    // Update is called once per frame
    public void PerformAction()
    {
        
        if (jumpElapsedTime < jumpDelay)
        {
            jumpElapsedTime += Time.deltaTime;
        }
           
        if (actionDic["attack"] == 1f)
        {
            sword.Attack();
        }
        float h = actionDic["horizontal"];
        float v = actionDic["vertical"];
        bool isJumping = actionDic["jump"] == 1f;
        cameraController.MoveCamera(actionDic["pitch"], actionDic["yaw"]);

        if ((h != 0 || v != 0 || isJumping) && isMoveable && groundDetection.isGrounded)
        {
            Move(h, v, isJumping);
        }
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, Camera.main.transform.eulerAngles.y, transform.localEulerAngles.z);
    }

    void Move(float horizontalMove, float verticalMove, bool isJumping)
    {
        int hMove = 0;
        int vMove = 0;
        if (horizontalMove != 0)
        {
            hMove = horizontalMove > 0 ? 1 : -1;
        }
        if (verticalMove != 0)
        {
            vMove = verticalMove > 0 ? 1 : -1;
        }

        Vector3 moveDir = new Vector3(hMove, 0f, vMove).normalized;
        if (isJumping && jumpElapsedTime >= jumpDelay)
        {
            jumpElapsedTime = 0f;
            Debug.Log("Jumpping");
            moveDir += Vector3.up;
            rigidbody.AddRelativeForce(moveDir * jumpForce * Time.deltaTime, ForceMode.Impulse);
        }
        else
        {
            transform.Translate(moveDir * speed * Time.deltaTime);
        }
    }

}