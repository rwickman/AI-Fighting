using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDetection : MonoBehaviour
{
    public bool isGrounded = true;

    public float minVelocityY = 0.00005f;
    public float airTime = 0f;

    private Rigidbody rigidbody;
    void Start()
    {
        rigidbody = GetComponentInParent<Rigidbody>();
    }

    private void Update()
    {
        airTime += Time.deltaTime;
       // Debug.Log(rigidbody.velocity.y);
    }

    void OnTriggerStay(Collider col)
    {
       //Debug.Log("STAY: " + col.gameObject.name);
        if (Mathf.Abs(rigidbody.velocity.y) <= minVelocityY)
        {
            isGrounded = true;
        }
    }

    void OnTriggerExit(Collider col)
    {   
        isGrounded = false;
        airTime = 0f;
     }

    void OnTriggerEnter(Collider col)
    {
        Debug.Log("AIR TIME: " + airTime);
        //Debug.Log("ENTER: " + col.gameObject.name);
        if (Mathf.Abs(rigidbody.velocity.y) <= minVelocityY)
        {
            isGrounded = true;
        }
    }
}
