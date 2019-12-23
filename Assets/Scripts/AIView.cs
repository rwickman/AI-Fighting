using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIView : MonoBehaviour
{

    public Transform target;
    public float viewAngle = 80.0f;
    public float rotationSpeed = 5f;
    public float maxViewDistance = 20f;
    public bool isHurt;


    private float distanceToTarget;
    private bool islookingAtTarget;
    private float angleToTarget;
    private UnityEngine.AI.NavMeshAgent agent;


    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.updateRotation = false;
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
        //Debug.Log("Distance: " + distanceToTarget);
        if (angleToTarget < viewAngle && distanceToTarget <= maxViewDistance)
        {
            Move();
        }
    }


    void Move()
    {
        Debug.Log("MOVING");
        agent.destination = target.position;
        RotateTowards(target);
    }

    private void RotateTowards(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        Debug.Log(Time.deltaTime * rotationSpeed);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
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