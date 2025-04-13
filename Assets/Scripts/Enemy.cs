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

    [Header("Drop System")]
    [Tooltip("Exp drops that can be dropped. Should be size 3 (unless changed later).")]
    public GameObject[] expDrops = new GameObject[3];
    [Tooltip("Probability that the enemy will also drop a exp pickup. When maxPotentialExpDrops is more than one, the probability is applied multiplicatively with every subsequent drop.")]
    public float expDropProbability = 1f;
    [Tooltip("Reduction factor for increasingly good drops. probabilityForBetterDrop is divided by this everytime it rolls for drop quality.")]
    public float reductionFactorForSubsequentExpDrops = 2f;
    [Tooltip("Max number of exp drops that may be dropped. Note: exp drop size depends on drop quality field.")]
    public int maxPotentialExpDrops = 3;

    [Tooltip("Min number of exp drops that may be dropped. Note: exp drop size depends on drop quality field.")]
    public int minPotentialExpDrops = 3;

    [Tooltip("Health drops that can be dropped. Should be size 3 (unless changed later).")]
    public GameObject[] healthDrops = new GameObject[3];

    [Tooltip("Probability that the enemy will also drop a health pickup. When maxPotentialHealthDrops is more than one, the probability is applied multiplicatively with every subsequent drop.")]
    public float healthDropProbability = .5f;
    [Tooltip("Reduction factor for increasingly good drops. probabilityForBetterDrop is divided by this everytime it rolls for drop quality.")]
    public float reductionFactorForSubsequentHealthDrops = 2f;
    [Tooltip("Max number of health drops that may be dropped. Note: health drop size depends on drop quality field.")]
    public int maxPotentialHealthDrops = 3;

    [Tooltip("Min number of health drops that may be dropped. Note: health drop size depends on drop quality field.")]
    public int minPotentialHealthDrops = 0;

    [Tooltip("Number of experience drops spawned when enemy dies")]
    public int numberOfExpDrops = 3;

    [Tooltip("Leave at 3, might be useful later but does nothing for now.")]
    public int maxDropsSelectionAmount = 3;

    [Tooltip("Quality of drops (1=Small, 2=Medium, 3=Big)")]
    public int dropQuality = 1;

    [Tooltip("Probablity that drops better than dropQuality are dropped. 0 is no chance, 1 is guaranteed items are at least one tier better.")]
    public float probabilityForBetterDrop = 0f;

    [Tooltip("Reduction factor for increasingly good drops. probabilityForBetterDrop is divided by this everytime it rolls for drop quality.")]
    public float reductionFactorForIncreasinglyBetterDrop = 2f;

    [Tooltip("Force applied to experience drops when spawned")]
    public float dropExplosionForce = 1f;

    [Tooltip("Lower bounds for the random explosion vector applied when dropping items.")]
    public Vector3 dropBurstVectorLowerBounds = new Vector3(-1f, .5f, 1f);
    [Tooltip("Upper bounds for the random explosion vector applied when dropping items.")]
    public Vector3 dropBurstVectorUpperBounds = new Vector3(1f, 2f, 1f);

    bool dead = false;
    float nextAttackTime = 0f;
    public GameObject[] drops;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        Mathf.Clamp(dropQuality, 1, maxDropsSelectionAmount);
        Mathf.Clamp(probabilityForBetterDrop, 0f, 1f);
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
        GameObjectPoolManager.Deactivate(gameObject);
        // effects, sound effects, drops
        DropCommons();

    }
    void AttemptAttack()
    {
        if (Time.time >= nextAttackTime)
        {
            playerStats.TakeDamage(damage);
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    #region drops
    public void DropCommons()
    {
        DropExp();
        DropHealth();

    }
    private void DropExp()
    {

        GameObject drop;
        int amountToDrop = GetNumberInRangeWithCompoundingProbability(minPotentialExpDrops, maxPotentialExpDrops, expDropProbability, reductionFactorForSubsequentExpDrops);
        for (int i = 0; i < amountToDrop; i++)
        {
            drop = GetDropByQuality(expDrops);
            BurstSpawnDrop(drop);
        }
    }
    private void DropHealth()
    {
        GameObject drop;
        int amountToDrop = GetNumberInRangeWithCompoundingProbability(minPotentialHealthDrops, maxPotentialHealthDrops, healthDropProbability, reductionFactorForSubsequentHealthDrops);
        for (int i = 0; i < amountToDrop; i++)
        {
            drop = GetDropByQuality(healthDrops);
            BurstSpawnDrop(drop);
        }
    }
    private int GetNumberInRangeWithCompoundingProbability(int min, int max, float probability, float probabilityReductionFactor)
    {

        int result = min;
        if (max - min >= 1)
        {
            float p = probability;
            while (result < max)
            {
                if (Random.Range(0f, 1f) > p)
                {
                    break;
                }
                p /= probabilityReductionFactor;
                result += 1;
            }
        }
        return result;

    }
    private GameObject GetDropByQuality(GameObject[] drops)
    {
        /*
        int quality = dropQuality;
        if (probabilityForBetterDrop > 0f)
        {
            float p = probabilityForBetterDrop;
            for (int i = 1; i <= maxDropsSelectionAmount - dropQuality; i++)
            {
                if (Random.Range(0f, 1f) > p)
                {
                    break;
                }

                p /= reductionFactorForIncreasinglyBetterDrop;
                quality += 1;
                i += 1;
            }
        }
        quality = Mathf.Clamp(quality, 0, drops.Length - 1);
        return drops[quality];
        */
        return drops[GetNumberInRangeWithCompoundingProbability(dropQuality, maxDropsSelectionAmount, probabilityForBetterDrop, reductionFactorForIncreasinglyBetterDrop)];
    }
    private void BurstSpawnDrop(GameObject drop)
    {
        GameObject currSpawned = GameObjectPoolManager.SpawnObject(drop,
            transform.position,
            Quaternion.identity);
        Rigidbody rb = currSpawned.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Add random upward direction to make pickups fly out
            Vector3 randomDirection = new Vector3(
                Random.Range(dropBurstVectorLowerBounds.x, dropBurstVectorUpperBounds.x),
                Random.Range(dropBurstVectorLowerBounds.y, dropBurstVectorUpperBounds.x),
                Random.Range(dropBurstVectorLowerBounds.z, dropBurstVectorUpperBounds.x)
            ).normalized;
            rb.isKinematic = false;
            currSpawned.GetComponent<SphereCollider>().enabled = true;
            rb.AddForce(randomDirection * dropExplosionForce, ForceMode.Impulse);
        }
    }

    #endregion
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, bufferRange);
    }
}