using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public float hitDelay = 0.01f;
    public float hitDistance = 2f;
    public float hitAngle = 20f;

    private float hitTime;
    private Rigidbody rb;
    private NavMeshAgent agent;
    private AIView view;
    private Sword sword;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        view = GetComponent<AIView>();
        sword = GetComponentInChildren<Sword>();
    }

    void Update()
    {
        
        if (rb.velocity.magnitude <= 1.0f && Time.time >=  hitTime + hitDelay)
        {
            rb.isKinematic = true;
            agent.enabled = true;
            view.isHurt = false;
        }

        if(view.getDistanceToTarget() <= hitDistance && view.getAngleToTarget() <= hitAngle) 
        {
            sword.Attack();
        }
    }

    public void AddForce(Vector3 force, ForceMode mode)
    {
        rb.isKinematic = false;
        agent.enabled = false;
        rb.AddForce(force, mode);
        Debug.Log(force);
        hitTime = Time.time;
        view.isHurt = true;
    }


}
