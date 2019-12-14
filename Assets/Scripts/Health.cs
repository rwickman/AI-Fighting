using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{

    
    public int health = 10;
    //public Player player;
    public float deathTime = 0.1f;
    
    public void Hurt(int hitPoints) {
        health -= hitPoints;
        print(name + " got hurt!");
        if (health <= 0) {
            StartCoroutine("Death");
        }
    }

    IEnumerator Death() {
        print("DEAD!");
        //player.isMoveable = false;
        yield return new WaitForSeconds(deathTime);
        Destroy(gameObject);
    }
}
