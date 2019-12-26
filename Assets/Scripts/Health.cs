﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Health : MonoBehaviour
{
    public int health = 10;
    
    //public Player player;
    public float deathTime = 0.1f;
    
    // Amount of time alert
    public float alertTime = 2f;
    // Time since last hurt
    public float hurtTime = 10f;
    private int startingHealth;
    private Slider healthSlider;

    void Start()
    {
        startingHealth = health;
        healthSlider = GetComponentInChildren<Slider>();
    }
    private void Update()
    {
        if (hurtTime < alertTime)
        {
            hurtTime += Time.deltaTime;
        }
    }

    public bool Hurt(int hitPoints) {
        // Returns if death
        hurtTime = 0f;
        health -= hitPoints;
        healthSlider.value = (float)health / (float)startingHealth;
        print(name + " got hurt!");
        if (health <= 0) {
            StartCoroutine("Death");
            return true;
        }
        return false;
    }

    IEnumerator Death() {
        print("DEAD!");
        //player.isMoveable = false;
        yield return new WaitForSeconds(deathTime);
        if (gameObject.name != "Agent")
        {
            Destroy(gameObject);
        }
        else
        {
            // TODO: make agent invisible and undetectable
        }
    }
}
