using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public float hitDelay = 0.25f;
    private float hitTime;
    private Rigidbody rb;
    private NavMeshAgent agent;
    private AIView view;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        view = GetComponent<AIView>();
    }

    void Update()
    { 
        if(rb.velocity.magnitude <= 1.0f && Time.time >=  hitTime + hitDelay)
        {
            rb.isKinematic = true;
            agent.enabled = true;
            view.isHurt = false;
        }
    }

    public void AddForce(Vector3 force, ForceMode mode = ForceMode.Force)
    {
        rb.isKinematic = false;
        agent.enabled = false;
        rb.AddForce(force, mode);
        hitTime = Time.time;
        view.isHurt = true;
    }
}
