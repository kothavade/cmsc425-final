using UnityEngine;

public class EnemyDrops : MonoBehaviour
{
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

    [Header("Drop Quality")]

    [Tooltip("Leave at 3, might be useful later but does nothing for now.")]
    public int maxDropsSelectionAmount = 3;

    [Tooltip("Quality of drops (0=Small, 1=Medium, 2=Big). Note this is used to index the drops array.")]
    public int dropQuality = 0;

    [Tooltip("Probablity that drops better than dropQuality are dropped. 0 is no chance, 1 is guaranteed items are at least one tier better.")]
    public float probabilityForBetterDrop = 0f;

    [Tooltip("Reduction factor for increasingly good drops. probabilityForBetterDrop is divided by this everytime it rolls for drop quality.")]
    public float reductionFactorForIncreasinglyBetterDrop = 2f;

    [Header("Drop Burst")]

    [Tooltip("Force applied to experience drops when spawned. Gets out of hand very quickly, leave around 1.")]
    public float dropExplosionForce = 1f;

    [Tooltip("Lower bounds for the random explosion vector applied when dropping items.")]
    public Vector3 dropBurstVectorLowerBounds = new Vector3(-1f, 1f, 1f);
    [Tooltip("Upper bounds for the random explosion vector applied when dropping items.")]
    public Vector3 dropBurstVectorUpperBounds = new Vector3(1f, 3f, 1f);

    void Start()
    {
        dropQuality = Mathf.Clamp(dropQuality, 0, maxDropsSelectionAmount - 1);
        probabilityForBetterDrop = Mathf.Clamp(probabilityForBetterDrop, 0f, 1f);
    }

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
        return drops[GetNumberInRangeWithCompoundingProbability(dropQuality, maxDropsSelectionAmount - 1, probabilityForBetterDrop, reductionFactorForIncreasinglyBetterDrop)];
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
                Random.Range(dropBurstVectorLowerBounds.y, dropBurstVectorUpperBounds.y),
                Random.Range(dropBurstVectorLowerBounds.z, dropBurstVectorUpperBounds.z)
            ).normalized;
            rb.isKinematic = false;
            currSpawned.GetComponent<SphereCollider>().enabled = true;
            rb.AddForce(randomDirection * dropExplosionForce, ForceMode.Impulse);
        }
    }
}