using UnityEngine;

[System.Serializable]
public class CappedRandomSpawner : EntitySpawner
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private double chance = 0.1;
    [SerializeField] private int max = 5;

    public override void TrySpawn()
    {
        if (prefab == null) return;
        if (locations == null || locations.Length == 0) return;

        if (spawnedEntities.Count >= max) return;

        double roll = (double)Random.value;
        if (roll < chance)
        {
            Vector3 pos = locations[Random.Range(0, locations.Length)];
            GameObject go = Object.Instantiate(prefab, pos, Quaternion.identity);
            int id = go.GetInstanceID();
            if (!spawnedEntities.ContainsKey(id))
                spawnedEntities.Add(id, go);
        }
    }
}
