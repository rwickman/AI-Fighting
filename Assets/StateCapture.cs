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
            Vector3 dir = new Vector3(localForward.x + x_angle, 0, localForward.y + y_angle); // Used for horizontally around body (think hoola hoop) x dir
            
            Vector3 dir2 = new Vector3(localForward.x + x_angle, localForward.y + y_angle, 0); // Verticall aroudn body y dir

            Vector3 dir3 = new Vector3(0, localForward.y + y_angle, localForward.x + x_angle); // Verticall aroudn body y dir
            
            // The angled raycast directions 
            Vector3 dir4 = Quaternion.AngleAxis(45, Vector3.right) * dir;

            Vector3 dir5 = Quaternion.AngleAxis(-45, Vector3.right) * dir;
            
            Vector3 dir6= Quaternion.AngleAxis(-90, Vector3.up) * dir5;

            Vector3 dir7 = Quaternion.AngleAxis(90, Vector3.up) * dir5;


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

            Debug.DrawRay(transform.position, dir5, Color.magenta);
            if (Physics.Raycast(transform.position, dir5, out hit, Mathf.Infinity, layerMask))
            {
                Debug.Log("HIT: " + hit.collider.gameObject.name);
            }

            Debug.DrawRay(transform.position, dir6, Color.magenta);
            if (Physics.Raycast(transform.position, dir6, out hit, Mathf.Infinity, layerMask))
            {
                Debug.Log("HIT: " + hit.collider.gameObject.name);
            }

            Debug.DrawRay(transform.position, dir7, Color.magenta);
            if (Physics.Raycast(transform.position, dir7, out hit, Mathf.Infinity, layerMask))
            {
                Debug.Log("HIT: " + hit.collider.gameObject.name);
            }



        }
    }
}
