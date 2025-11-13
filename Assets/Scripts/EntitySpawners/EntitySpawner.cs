using System.Collections.Generic;
using UnityEngine;

public abstract class EntitySpawner
{
    [HideInInspector] public Vector3[] locations;
    protected Dictionary<int, GameObject> spawnedEntities = new Dictionary<int, GameObject>();
    public bool continuous;
    public abstract void TrySpawn();
}
