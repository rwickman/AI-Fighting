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
    private List<Health> enemies;
    
    //private int pointsToWin;

    public bool isEpisodeOver = false;
    public bool shouldRestart = false;
    
    private bool lastRewardApplied = false;

    void Awake()
    {
        enemies = new List<Health>();
        foreach (Transform child in transform)
        {
            if (child.tag == "Enemy")
            {
                enemies.Add(child.gameObject.GetComponent<Health>());
            }
        }
        agentHealth = agent.GetComponent<Health>();
        agentScore = agent.GetComponent<Score>();
    }

    void Update()
    {    
        
        if (agentHealth.health <= 0 && !lastRewardApplied)
        {
            UpdateReward(RewardType.Lost);
            lastRewardApplied = true;
            isEpisodeOver = true;
        }
        else if (!lastRewardApplied && AllEnemiesDead())
        {
            UpdateReward(RewardType.Win);
            lastRewardApplied = true;
            isEpisodeOver = true;
        }
        if(shouldRestart)
        {
            RestartEpisode();
        }
    }

    private bool AllEnemiesDead()
    {
        foreach (Health enemyHealth in enemies)
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
