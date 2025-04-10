using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public PlayerStats playerStats;
    public int damage = 10;
    public float attackRange = 2f;
    public float bufferRange = 0.5f;
    public float attackCooldown = 2f;
    public int max_health = 5;
    public int health;
    bool dead = false;

    public AudioClip damagePlayerSound;
    float nextAttackTime = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void OnEnable(){
        health = max_health;
        dead = false;

    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= bufferRange)
        {
            agent.isStopped = true;
            AttemptAttack();
        }
        else if (distanceToPlayer <= attackRange)
        {
            agent.isStopped = false;

            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            Vector3 targetPosition = player.position - directionToPlayer * bufferRange;

            agent.SetDestination(targetPosition);
            AttemptAttack();
        }
        else
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
    }

    public int TakeDamage(int dmg){
        print("Enemy took DMG:" + dmg);
        health -= dmg;
        if (health < 1 && !dead) {
            dead = true;
            GameObjectPoolManager.Deactivate(gameObject);
            return -1;
        }
        return Mathf.Max(health, 0);
    }

    void AttemptAttack()
    {
        if (Time.time >= nextAttackTime)
        {
            playerStats.TakeDamage(damage);
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, bufferRange);
    }
}