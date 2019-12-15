using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIView : MonoBehaviour
{

    public Transform target;
    public float viewAngle = 80.0f;
    public bool isHurt;

    private UnityEngine.AI.NavMeshAgent agent;
    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isHurt){
            Look();
        }
    }

    void Look()
    {
        Vector3 targetDir = target.position - transform.position;
        float angle = Vector3.Angle(targetDir, transform.forward);
        //Debug.Log("Enemy Angle: " + angle);
        if(angle < viewAngle)
        {
            Move();
        }
    }

    void Move()
    {
        Debug.Log("MOVING");
        agent.destination = target.position;
    }
}
