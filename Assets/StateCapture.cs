using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateCapture : MonoBehaviour
{

    public int numRaycast = 20;

    int layerMask;
    // Start is called before the first frame update
    void Start()
    {
        // Ignroe this AI layers
        layerMask = 1 << 8;
        layerMask = ~layerMask;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastState();
    }

    void RaycastState()
    {
        float angle = 0;
        for (int i = 0; i < numRaycast; i++)
        {
            float x_angle = Mathf.Cos(angle);
            float y_angle = Mathf.Sin(angle); 
            angle += 2 * Mathf.PI / numRaycast;

            Vector3 localForward = transform.TransformDirection(transform.forward);
            Vector3 dir = new Vector3(localForward.x + x_angle, 0, localForward.y + y_angle); // Used for horizontally around body (think hoola hoop)
            
            Vector3 dir2 = new Vector3(localForward.x + x_angle, localForward.y + y_angle, 0); // Verticall aroudn body (think door maybe?)

            
            
            Vector3 dir3 = Quaternion.AngleAxis(45, Vector3.right) * dir;

            Vector3 dir4 = Quaternion.AngleAxis(-45, Vector3.right) * dir;

            RaycastHit hit;
            
            Debug.DrawRay(transform.position, dir, Color.magenta);
            if (Physics.Raycast(transform.position, dir, out hit, Mathf.Infinity, layerMask))
            {
                Debug.Log("HIT: " + hit.collider.gameObject.name);
            }

            Debug.DrawRay(transform.position, dir2, Color.magenta);
            if (Physics.Raycast(transform.position, dir2, out hit, Mathf.Infinity, layerMask))
            {
                Debug.Log("HIT: " + hit.collider.gameObject.name);
            }

            Debug.DrawRay(transform.position, dir3, Color.magenta);
            if (Physics.Raycast(transform.position, dir3, out hit, Mathf.Infinity, layerMask))
            {
                Debug.Log("HIT: " + hit.collider.gameObject.name);
            }

            Debug.DrawRay(transform.position, dir4, Color.magenta);
            if (Physics.Raycast(transform.position, dir4, out hit, Mathf.Infinity, layerMask))
            {
                Debug.Log("HIT: " + hit.collider.gameObject.name);
            }
        }
    }
}
