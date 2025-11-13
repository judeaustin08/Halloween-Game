using UnityEngine;

public class ItemSpawnpoint : MonoBehaviour
{
    public GameObject candyPrefab;
    public float riskDistanceThreshold = 5f;
    public int riskFactor = 1;

    Ray ray;

    public void Initialize(Vector3[] floorClerkSpawns, Vector3[] cashierSpawns)
    {
        // Calculate risk factor
        // If this spot can be seen by a cashier, gain 5 risk factor
        foreach (Vector3 cashier in cashierSpawns)
            if (!Physics.Raycast(transform.position, cashier - transform.position, Vector3.Distance(transform.position, cashier), GameManager.active.obstacleLayers))
                riskFactor = 5;

        // Add 1 to risk factor for every nearby floor clerk spawn
        foreach (Vector3 floorClerk in floorClerkSpawns)
            if (Vector3.Distance(floorClerk, transform.position) < riskDistanceThreshold)
                riskFactor++;
    }

    public void Spawn()
    {
        ItemPickup candy = Instantiate(candyPrefab, transform.position, Quaternion.identity).GetComponent<ItemPickup>();
        candy.quantity = riskFactor;
    }

    public void Update()
    {
        Debug.DrawRay(ray.origin, ray.direction, Color.green);
    }
}