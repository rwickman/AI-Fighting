using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIView : MonoBehaviour
{

    public Transform target;
    public float viewAngle = 20.0f;

    private UnityEngine.AI.NavMeshAgent agent;
    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        Look();
    }

    void Look()
    {
        Vector3 targetDir = target.position - transform.position;
        float angle = Vector3.Angle(targetDir, transform.forward);
        Debug.Log("Enemy Angle: " + angle);
        if(angle < viewAngle)
        {
            Move();
        }
    }

    void Move()
    {
        agent.destination = target.position;
    }
}
