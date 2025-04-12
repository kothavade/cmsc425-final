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
    public float max_health = 5;
    public float health;
    bool dead = false;

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

    public float TakeDamage(float dmg){
        print("Enemy took DMG:" + dmg);
        health -= dmg;
        if (health <= 0 && !dead) {
            KillEnemy();
            return -1;
        }
        return Mathf.Max(health, 0f);
    }
    public void KillEnemy()
    {
        dead = true;
        GameObjectPoolManager.Deactivate(gameObject);
        // effects, sound effects, drops
        DropExp();

    }

    public void DropExp()
    {
        
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