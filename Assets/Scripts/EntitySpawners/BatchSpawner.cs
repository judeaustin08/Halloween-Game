using UnityEngine;

[System.Serializable]
public enum SpawnMode { FIXED, PER }

[System.Serializable]
public class BatchSpawner : EntitySpawner
{
    [SerializeField] private SpawnMode mode = SpawnMode.FIXED;
    [SerializeField] private GameObject prefab;
    [SerializeField] private int count = 1;

    public override void TrySpawn()
    {
        if (prefab == null) return;
        if (locations == null || locations.Length == 0) return;

        if (mode == SpawnMode.FIXED)
        {
            for (int i = 0; i < count; i++)
            {
                Vector3 pos = locations[Random.Range(0, locations.Length)];
                GameObject go = Object.Instantiate(prefab, pos, Quaternion.identity);
                int id = go.GetInstanceID();
                if (!spawnedEntities.ContainsKey(id))
                    spawnedEntities.Add(id, go);
            }
        }
        else // PER
        {
            for (int j = 0; j < locations.Length; j++)
            {
                Vector3 loc = locations[j];
                for (int i = 0; i < count; i++)
                {
                    GameObject go = Object.Instantiate(prefab, loc, Quaternion.identity);
                    int id = go.GetInstanceID();
                    if (!spawnedEntities.ContainsKey(id))
                        spawnedEntities.Add(id, go);
                }
            }
        }
    }
}
