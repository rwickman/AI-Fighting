using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Health : MonoBehaviour
{
    public int health = 10;
    //public Player player;
    public float deathTime = 0.1f;
    
    // Amount of time alert
    public float alertTime = 2f;
    // Time since last hurt
    public float hurtTime = 10f;

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
        Destroy(gameObject);
    }
}
