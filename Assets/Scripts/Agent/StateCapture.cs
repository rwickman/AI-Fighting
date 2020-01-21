using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class StateCapture : MonoBehaviour
{

    public int numRaycast = 20;

    public bool shouldSendState = false;
    [HideInInspector]
    public bool sentEpisodeOver = false;
    public bool showDebugRays = false;    
    public bool shouldExplore = true;
    private bool hasStarted = false;
    private bool sentExplorationSetting = false;
    private const int numSubFeatures = 9;
    private int layerMask;

   
    
    private Agent agent;
    private Health agentHealth;
    private Sword agentSword;
    private AgentManager agentManager;
    private PolicyConnection policy_con;

    private List<float> stateFeatures;

    // Limit capturing to once per frame
    //private bool capturedStateThisFrame = false;

    // Start is called before the first frame update
    void Start()
    {
        // Ignroe this AI layers
        layerMask = 1 << 8;
        layerMask = ~layerMask;
        agent = GetComponent<Agent>();
        agentHealth = GetComponent<Health>();
        agentSword = GetComponentInChildren<Sword>();
        agentManager = GetComponentInParent<AgentManager>();
        policy_con = GetComponent<PolicyConnection>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasStarted)
        {
            policy_con.StartConnection(ResetShouldSendState);
            hasStarted = true;
        }
        if (!sentExplorationSetting)
        {
            SendExplorationSetting();
            sentExplorationSetting = true;
        }
        if (shouldSendState && !sentEpisodeOver)
        {
            RaycastState();
            SendState();
            shouldSendState = false;
        }
        
    }

    void RaycastState()
    {
        stateFeatures = new List<float>();

        // Add this AI state
        AddAgentState();
        
        // Add state for AI "view"
        float angle = 0;
        for (int i = 0; i < numRaycast; i++)
        {
            float x_angle = Mathf.Cos(angle);
            float y_angle = Mathf.Sin(angle); 
            angle += 2 * Mathf.PI / numRaycast;

            Vector3 localForward = transform.forward;
            // Vector3 dir1 = new Vector3(localForward.x + x_angle, 0, localForward.y + y_angle);
            // Vector3 dir2 = new Vector3(localForward.x + x_angle, localForward.y + y_angle, 0);
            // Vector3 dir3 = new Vector3(0, localForward.y + y_angle, localForward.x + x_angle);
            // // The angled raycast directions 
            // Vector3 dir4 = Quaternion.AngleAxis(45, Vector3.right) * dir1;
            // Vector3 dir5 = Quaternion.AngleAxis(-45, Vector3.right) * dir1;
            // Vector3 dir6= Quaternion.AngleAxis(-90, Vector3.up) * dir5;
            // Vector3 dir7 = Quaternion.AngleAxis(90, Vector3.up) * dir5;
             //Vector3 dir11 = new Vector3(localForward.x + x_angle/4, localForward.y + y_angle/4, localForward.z);
            Vector3 dir8 = new Vector3(localForward.x + x_angle/4, localForward.y + y_angle/4, localForward.z);
            Vector3 dir9 = new Vector3(localForward.x + x_angle/6, localForward.y + y_angle/6, localForward.z);
            Vector3 dir10 = new Vector3(localForward.x + x_angle/10, localForward.y + y_angle/10, localForward.z);
            Vector3 dir11 = new Vector3(localForward.x + x_angle/40, localForward.y + y_angle/40, localForward.z);
             //Vector3 dir9 = new Vector3(localForward.x + x_angle/2, localForward.y + y_angle/2, localForward.z);


            // Remove duplicate rays (there is probably a better way to do this but oh well)
            Vector3[] dirs = { /*dir1, dir2, dir3, dir4, dir5, dir6, dir7, */dir8, dir9, dir10, dir11};
            /*
            List<Vector3> dirs_non_duplicates = new List<Vector3>();
            
            for (int j = 0; j < dirs.Length; j++)
            {
                bool isDuplicate = false;
                
                foreach (Vector3 dir in dirs_non_duplicates)
                {
                    if (dirs[j].Equals(dir))
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
            */
            // Raycast single ray
            foreach (Vector3 dir in dirs)
            {
                RaycastSubState(dir);
            }
        }
        //print("TOTAL: " + stateFeatures.Count);
    }

    void SendState()
    {
        var dict = new Dictionary<string, dynamic>();

        dict["state"] = stateFeatures.ToArray();
        dict["reward"] = agentManager.GetReward();
        dict["done"] = agentManager.isEpisodeOver;
        sentEpisodeOver = agentManager.isEpisodeOver;
        policy_con.isEpisodeOver = agentManager.isEpisodeOver;
        agentManager.ResetReward();
        string jsonStr = JsonConvert.SerializeObject(dict);
        policy_con.SendState(jsonStr);
        agentManager.curTimeStep += 1;
    }

    void SendExplorationSetting()
    {
        Dictionary<string, bool> dict = new Dictionary<string, bool>();
        dict["explore"] = shouldExplore;
        string jsonStr = JsonConvert.SerializeObject(dict);
        policy_con.SendExplorationSetting(jsonStr);
    }


    void RaycastSubState(Vector3 dir)
    {
        RaycastHit hit;
        if (showDebugRays)
        {
            Debug.DrawRay(transform.position, dir*30, Color.magenta);
        }
        
        if (Physics.Raycast(transform.position, dir, out hit, Mathf.Infinity, layerMask))
        {
            AddEnvironmentStateFeatures(hit.collider.gameObject);
            //Debug.Log("HIT: " + hit.collider.gameObject.name);
        }
        else
        {
            AddEmptyStateFeatures();
        }
    }

    void AddAgentState()
    {
        stateFeatures.Add(transform.position.x);
        stateFeatures.Add(transform.position.y);
        stateFeatures.Add(transform.position.z);
        //print("ROT X: " + transform.eulerAngles.x + " ROT Y: " + transform.eulerAngles.y + " ROT Z: " + transform.eulerAngles.z);
        stateFeatures.Add(transform.eulerAngles.x);
        stateFeatures.Add(transform.eulerAngles.y);
        stateFeatures.Add(transform.eulerAngles.z);
        //stateFeatures.Add(transform.rotation.w);
        stateFeatures.Add(agentHealth.health);
        stateFeatures.Add(agentSword.attackElapsedTime);
        stateFeatures.Add(agent.jumpElapsedTime);
    }

    void AddEnvironmentStateFeatures(GameObject raycastedObject)
    {
        int tagID = GetTagID(raycastedObject.tag);
        bool isHuman = tagID == 1 || tagID == 2;
        //print(raycastedObject.tag);
        stateFeatures.Add(tagID);
        if (isHuman)
        {
            stateFeatures.Add(raycastedObject.gameObject.GetComponent<Health>().health);
        }
        else
        {
            // Add zero health to objects that are not humans as they have no health
            stateFeatures.Add(0);
        }
        stateFeatures.Add(raycastedObject.transform.position.x);
        stateFeatures.Add(raycastedObject.transform.position.y);
        stateFeatures.Add(raycastedObject.transform.position.z);
        stateFeatures.Add(raycastedObject.transform.eulerAngles.x);
        stateFeatures.Add(raycastedObject.transform.eulerAngles.y);
        stateFeatures.Add(raycastedObject.transform.eulerAngles.z);
        //stateFeatures.Add(raycastedObject.transform.rotation.w);
        stateFeatures.Add(Vector3.Distance(transform.position, raycastedObject.transform.position));
    }

    void AddEmptyStateFeatures()
    {
        for (int i = 0; i < numSubFeatures; i++)
        {
            stateFeatures.Add(0);
        }
    }

    int GetTagID(string tag)
    {
        switch (tag)
        {
            case "Player":
                return 1;
            case "Enemy":
                return 2;
            case "Wall":
                return 3;
            case "Ground":
                return 4;
            case "Sword":
                return 5;
            default:
                return 0;
        }
    }

    public void ResetShouldSendState()
    {
        shouldSendState = true;
    }
}
