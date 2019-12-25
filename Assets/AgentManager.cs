using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentManager : MonoBehaviour
{
    public GameObject agent;
    public int numEpisodes;
    public string enemyTag = "Enemy";

    private Health agentHealth;
    private Score agentScore;
    private GameObject[] enemies;
    
    private int pointsToWin;
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
            RestartEpisode();
        }
        else if (agentScore.GetScore() >=  pointsToWin)
        {
            RestartEpisode();
        }
    }

    void RestartEpisode()
    {
        Application.LoadLevel(Application.loadedLevel);
    }
}
