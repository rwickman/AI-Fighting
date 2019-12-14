using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;

    private Vector3 offset;
    private Camera m_cam;
    private float yaw = 0.0f;
    private float pitch = 0.0f;
    public float speedH = 2.0f;
    public float speedV = 2.0f;
    public float camFollowPlayerLerpTerm = 6.4f;
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
        pitch = Mathf.Clamp(pitch, -60f, 55f);

        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
    }
}
