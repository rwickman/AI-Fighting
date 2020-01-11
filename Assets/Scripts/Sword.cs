using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Sword : MonoBehaviour
{
    public string swordAnimName = "SwordAttack";
    public float hitRate = 0.5f;
    public float hitForce = 1.4f;
    public int hitDamage = 2;
    public float attackElapsedTime = 0f;

    public string enemyTag = "Enemy"; 
    private Animator m_anim;
    private Collider m_coll;
    private float animElapsedTime = 0f;
    private float hitAnimTime;
    private HashSet<int> objectsHitAlready = new HashSet<int>();

    private Score score;

    private AgentManager agentManager;
    // Start is called before the first frame update
    void Start()
    {
        m_anim = GetComponent<Animator>();
        m_coll = GetComponent<Collider>();
        score = GetComponentInParent<Score>();
        agentManager = GameObject.Find("GameManager").GetComponent<AgentManager>();
        RuntimeAnimatorController ac = m_anim.runtimeAnimatorController;
        for (int i = 0; i < ac.animationClips.Length; i++) {
            if (ac.animationClips[i].name == swordAnimName) {
                hitAnimTime = ac.animationClips[i].length;
            }
        }   
    }

    // Update is called once per frame
    void Update()
    {
        attackElapsedTime += Time.deltaTime;
        animElapsedTime += Time.deltaTime;

        if (animElapsedTime > hitAnimTime && objectsHitAlready.Count > 0) {
            objectsHitAlready = new HashSet<int>();
        }
    }

    public void Attack()
    {
        if(attackElapsedTime >= hitRate) {
            m_anim.SetTrigger("isAttacking");
            attackElapsedTime = 0f;
            animElapsedTime = 0f;
        }
    }

    private void OnTriggerStay(Collider other) {
        Health otherHealth = other.gameObject.GetComponent<Health>();
        
        if(otherHealth != null && 
           animElapsedTime < hitAnimTime && 
           !m_coll.transform.IsChildOf(other.transform) &&
           !objectsHitAlready.Contains(other.GetInstanceID())) {
            if (otherHealth.Hurt(hitDamage))
            {
                score.AddPoints(other.gameObject.GetComponent<Score>().pointsWorth);
                if (other.gameObject.tag == enemyTag)
                {
                    agentManager.UpdateReward(AgentManager.RewardType.EnemyKilled);
                }
            }
            objectsHitAlready.Add(other.GetInstanceID());
            
            Vector3 dir = other.transform.position - transform.position;
            dir.y = 0f;
            dir = dir.normalized;
            dir += Vector3.up;
            if (other.gameObject.tag != "Player")
            {
                Debug.DrawLine(transform.position, dir * hitForce, Color.white, 10f);
                Enemy enemy = other.gameObject.GetComponent<Enemy>();
                enemy.AddForce(dir * hitForce, ForceMode.Impulse);
                if (other.gameObject.tag == enemyTag)
                {
                    agentManager.UpdateReward(AgentManager.RewardType.EnemyHurt);
                }
            }
            else
            {
                agentManager.UpdateReward(AgentManager.RewardType.AgentHurt);
                other.gameObject.GetComponent<Rigidbody>().AddForce(dir * hitForce, ForceMode.Impulse);
                Debug.DrawLine(transform.position, dir * hitForce, Color.red, 10f);
            }
        }
    }
}
