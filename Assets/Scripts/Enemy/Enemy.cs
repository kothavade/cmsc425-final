using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Tooltip("The NavMeshAgent component used for pathfinding")]
    public NavMeshAgent agent;

    [Tooltip("Reference to the player's transform for tracking")]
    public Transform player;

    [Tooltip("Reference to the player's stats component for dealing damage")]
    public PlayerStats playerStats;

    [Tooltip("Amount of damage dealt to player per attack")]
    public int damage = 10;

    [Tooltip("Maximum distance at which the enemy can attack the player")]
    public float attackRange = 2f;

    [Tooltip("Minimum distance the enemy tries to maintain from the player")]
    public float bufferRange = 0.5f;

    [Tooltip("Time in seconds between attacks")]
    public float attackCooldown = 2f;

    [Tooltip("Maximum health points of the enemy")]
    public float max_health = 5;

    [Tooltip("Current health points of the enemy")]
    public float health;
    
    [Tooltip("Reference to the EnemyDrops component")]
    private EnemyDrops enemyDrops;

    bool dead = false;
    float nextAttackTime = 0f;
    

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        enemyDrops = GetComponent<EnemyDrops>();
    }

    void OnEnable()
    {
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

    public float TakeDamage(float dmg)
    {
        print("Enemy took DMG:" + dmg);
        health -= dmg;
        if (health <= 0 && !dead)
        {
            KillEnemy();
            return -1;
        }
        return Mathf.Max(health, 0f);
    }
    
    public void KillEnemy()
    {
        dead = true;
        // Handle drops before deactivating
        if (enemyDrops != null)
        {
            enemyDrops.DropCommons();
        }
        
        GameObjectPoolManager.Deactivate(gameObject);
        // effects, sound effects
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