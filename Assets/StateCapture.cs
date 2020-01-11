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
            Vector3 dir1 = new Vector3(localForward.x + x_angle, 0, localForward.y + y_angle); // Used for horizontally around body (think hoola hoop)
            Vector3 dir2 = new Vector3(localForward.x + x_angle, localForward.y + y_angle, 0); // Verticall aroudn body
            Vector3 dir3 = new Vector3(0, localForward.y + y_angle, localForward.x + x_angle); 
            // The angled raycast directions 
            Vector3 dir4 = Quaternion.AngleAxis(45, Vector3.right) * dir1;
            Vector3 dir5 = Quaternion.AngleAxis(-45, Vector3.right) * dir1;
            Vector3 dir6= Quaternion.AngleAxis(-90, Vector3.up) * dir5;
            Vector3 dir7 = Quaternion.AngleAxis(90, Vector3.up) * dir5;

            // Remove duplicate rays (there is probably a better way to do this but oh well)
            Vector3[] dirs = { dir1, dir2, dir3, dir4, dir5, dir6, dir7 };

            List<Vector3> dirs_non_duplicates = new List<Vector3>();
            
            for (int j = 0; j < dirs.Length; j++)
            {
                bool isDuplicate = false;
                foreach (Vector3 dir in dirs_non_duplicates)
                {
                    if (dirs[j].x == dir.x && dirs[j].y == dir.y && dirs[j].z == dir.z)
                    {
                        isDuplicate = true;
                        break;
                    }
                }

                if (!isDuplicate)
                {
                    dirs_non_duplicates.Add(dirs[j]);
                }
            }

            // Raycast single ray
            foreach (Vector3 dir in dirs_non_duplicates)
            {
                RaycastSubState(dir);
            }


        }
    }

    void RaycastSubState(Vector3 dir)
    {
        RaycastHit hit;

        Debug.DrawRay(transform.position, dir, Color.magenta);
        if (Physics.Raycast(transform.position, dir, out hit, Mathf.Infinity, layerMask))
        {
            Debug.Log("HIT: " + hit.collider.gameObject.name);
        }
    }
}
