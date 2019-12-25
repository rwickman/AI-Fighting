using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score : MonoBehaviour
{
    // The amount of points this character is worth;
    public int pointsWorth = 1;
    private  int currentScore;

    public int GetScore()
    {
        return currentScore;
    }
    public void AddPoints(int points)
    {
        currentScore += points;
    }
}
