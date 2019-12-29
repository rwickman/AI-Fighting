using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentCameraController : MonoBehaviour
{
    public GameObject agent;

    private Vector3 offset;
    private Camera m_cam;
    private float yaw = 0.0f;
    private float pitch = 0.0f;
    public float speedH = 2.0f;
    public float speedV = 2.0f;
    public float camFollowAgentLerpTerm = 20.0f;

    public float pitchMin = -90f;
    public float pitchMax = 90f;
    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - agent.transform.position;
        m_cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    public void MoveCamera(float pitchMove, float yawMove)
    {
        // pitchMove and yawMove are one of {-1,0,1}
        transform.position = Vector3.Lerp(transform.position, agent.transform.position + offset, camFollowAgentLerpTerm * Time.deltaTime);
        yaw += speedH * yawMove;
        pitch -= speedV * pitchMove;
        
        //the rotation range
        pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);

        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
    }
}
