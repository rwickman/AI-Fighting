using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIView : MonoBehaviour
{

    public Transform target;
    public float viewAngle = 80.0f;

    public bool isHurt;


    private float distanceToTarget;
    private bool islookingAtTarget;
    private floats angleToTarget;
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
        distanceToTarget = Vector3.Distance(target.position, transform.position);
        angleToTarget = Vector3.Angle(targetDir, transform.forward);
        //Have it rotate toward player if within stopping distance
        //Debug.Log("Enemy Angle: " + angle);
        if (angleToTarget < viewAngle)
        {
            Move();
        }
    }


    void Move()
    {
        Debug.Log("MOVING");
        agent.destination = target.position;
    }

    public float getDistanceToTarget()
    {
        return distanceToTarget;
    }

    public float getAngleToTarget()
    {
        return angleToTarget;
    }
}