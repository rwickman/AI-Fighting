using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentManager : MonoBehaviour
{

    public enum RewardType
    {
        EnemyHurt = 10,
        EnemyKilled = 10,
        Win = 100,
        AgentHurt = -10,
        Lost = -100,
        GameInProgress=-1
              
    }

    private struct InitGOState
    {
        public Vector3 position;
        public Quaternion rotation;
    }
    
    public int numEpisodes;
    public string enemyTag = "Enemy";
    private int currentReward;

    private GameObject agent;
    private Health agentHealth;
    private Score agentScore;
    private StateCapture agentStateCapture;
    private InitGOState agentInitState;
    private List<GameObject> enemies;
    private List<Health> enemiesHealth;
    private List<InitGOState> enemiesInitState;
    
    //private int pointsToWin;

    public bool isEpisodeOver = false;
    public bool shouldRestart = false;
    
    private bool lastRewardApplied = false;
    private float agentFellOutOfMapThreshold;

    void Awake()
    {
        agentFellOutOfMapThreshold = transform.position.y - 10f;
        enemies = new List<GameObject>();
        enemiesHealth = new List<Health>();
        enemiesInitState = new List<InitGOState>();

        foreach (Transform child in transform)
        {
            if (child.tag == "Enemy")
            {
                enemies.Add(child.gameObject);
                enemiesHealth.Add(child.gameObject.GetComponent<Health>());
                enemiesInitState.Add(CreateInitGOState(child.gameObject));
                
            }
            else if (child.tag == "Player")
            {
                agent = child.gameObject;
                agentInitState = CreateInitGOState(agent);
            }
        }
        agentHealth = agent.GetComponent<Health>();
        agentScore = agent.GetComponent<Score>();
        agentStateCapture = agent.GetComponent<StateCapture>();
    }

    void Update()
    {    
        if(shouldRestart)
        {
            RestartEpisode();
        }
        else if (!lastRewardApplied)
        {
            if (agentHealth.health <= 0 || agent.transform.position.y < agentFellOutOfMapThreshold)
            {
                UpdateReward(RewardType.Lost);
                lastRewardApplied = true;
                isEpisodeOver = true;
            }
            else if (AllEnemiesDead())
            {
                UpdateReward(RewardType.Win);
                lastRewardApplied = true;
                isEpisodeOver = true;
            }
            else {
                // For each frame add negative 
                UpdateReward(RewardType.GameInProgress);
            }
        }
        

    }

    private bool AllEnemiesDead()
    {
        foreach (Health enemyHealth in enemiesHealth)
        {
            if (enemyHealth.health > 0)
            {
                return false;
            }
        }
        return true;
    }
    
    public void RestartEpisode()
    {
        shouldRestart = false;
        ResetReward();
        lastRewardApplied = false;
        isEpisodeOver = false;
        agentStateCapture.sentEpisodeOver = false;
        
        // Reset initial state of agent and enemies 
        agent.transform.position = new Vector3(agentInitState.position.x, agentInitState.position.y, agentInitState.position.z);
        agent.transform.rotation = new Quaternion(agentInitState.rotation.x, agentInitState.rotation.y, agentInitState.rotation.z, agentInitState.rotation.w);
        agentHealth.health  = agentHealth.startingHealth;
        
        for (int i = 0; i < enemies.Count; i++)
        {
            InitGOState curState = enemiesInitState[i];
            enemies[i].transform.position = new Vector3(curState.position.x, curState.position.y, curState.position.z);
            enemies[i].transform.rotation = new Quaternion(curState.rotation.x, curState.rotation.y, curState.rotation.z, curState.rotation.w);
            enemies[i].GetComponent<Enemy>().enabled = true;
            enemies[i].GetComponent<AIView>().enabled = true;
            enemiesHealth[i].health = enemiesHealth[i].startingHealth;
            enemiesHealth[i].ResetHealthSlider();
        }
        agentStateCapture.shouldSendState = true;
        //Application.LoadLevel(Application.loadedLevel);
    }

    public void UpdateReward(RewardType reward)
    {
        if (!lastRewardApplied)
            currentReward += (int)reward;
    }


    public int GetReward()
    {
        return currentReward;
    }

    public void ResetReward()
    {
        currentReward = 0;
    }

    private InitGOState CreateInitGOState(GameObject go)
    {
        InitGOState initGOState;

        initGOState.position = new Vector3(go.transform.position.x, go.transform.position.y, go.transform.position.z);
        initGOState.rotation = new Quaternion(go.transform.rotation.x, go.transform.rotation.y, go.transform.rotation.z, go.transform.rotation.w);
        return initGOState; 
    }
}
