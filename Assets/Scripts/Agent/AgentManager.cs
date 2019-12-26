using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentManager : MonoBehaviour
{

    public enum RewardType
    {
        EnemyHurt = 1,
        EnemyKilled = 10,
        Win = 100,
        AgentHurt = -1,
        Lost = -100        
    }
    public GameObject agent;
    public int numEpisodes;
    public string enemyTag = "Enemy";
    private int currentReward;

    private Health agentHealth;
    private Score agentScore;
    private GameObject[] enemies;
    
    private int pointsToWin;

    public bool isEpisodeOver = false;
    public bool shouldRestart = false;

    void Awake()
    {
        agentHealth = agent.GetComponent<Health>();
        agentScore = agent.GetComponent<Score>();
        enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        for (int i = 0; i < enemies.Length; i++)
        {
            pointsToWin += enemies[i].GetComponent<Score>().pointsWorth;
        }
    }

    void Update()
    {
        if (agentHealth.health <= 0)
        {
            UpdateReward(RewardType.Lost);
            isEpisodeOver = true;
        }
        else if (agentScore.GetScore() >=  pointsToWin)
        {
            UpdateReward(RewardType.Win);
            isEpisodeOver = true;
        }
        if(shouldRestart)
        {
            RestartEpisode();
        }
    }

    public void RestartEpisode()
    {
        Application.LoadLevel(Application.loadedLevel);
    }

    public void UpdateReward(RewardType reward)
    {
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
}
