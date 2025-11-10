using UnityEngine;

public class ItemSpawnpoint : MonoBehaviour
{
    public GameObject candyPrefab;
    public float riskDistanceThreshold = 5f;
    [HideInInspector] public int riskFactor;

    public void Initialize(Transform[] floorClerkSpawns, Transform[] cashierSpawns) {
        // Calculate risk factor

        // If this spot can be seen by a cashier, gain 5 risk factor
        foreach (Transform cashier in cashierSpawns)
            if (Physics.Raycast(transform.position, cashier.position - transform.position, Vector3.Distance(transform.position, cashier.position), GameManager.active.obstacleLayers))
                riskFactor = 5;

        // Add 1 to risk factor for every nearby floor clerk spawn
        foreach (Transform floorClerk in floorClerkSpawns)
            if (Vector3.Distance(floorClerk.position, transform.position) < riskDistanceThreshold)
                riskFactor++;
    }

    public void Spawn()
    {
        ItemPickup candy = Instantiate(candyPrefab).GetComponent<ItemPickup>();
        candy.quantity = riskFactor;
    }
}