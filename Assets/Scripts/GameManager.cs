using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("References")]

    public MapGen mapGenerator;
    /*
     * for when these things are coded
    public List<EntitySpawner> entitySpawners = new List<EntitySpawner>();
    */
    public static GameManager active;

    [Header("Game Variables")]
    public LayerMask obstacleLayers;
    public float restockInterval = 30f;

    [Header("Game Data")]

    public int candy = 0;
    private GameObject[] candySpawnLocations;
    private Transform[] cashierSpawnpoints;
    private Transform[] floorClerkSpawnpoints;
    public float restockTimer = 0;

    void Awake()
    {
        active = this;

        restockTimer = 0;
    }

    void Start()
    {
        mapGenerator.GenerateMap();
        candySpawnLocations = GameObject.FindGameObjectsWithTag("candySpawnpoint");
        foreach (GameObject spawnpoint in candySpawnLocations)
            spawnpoint.GetComponent<ItemSpawnpoint>().Initialize(floorClerkSpawnpoints, cashierSpawnpoints);

        UIManager.Instance.UpdateCandy(candy);
    }

    void Update()
    {
        restockTimer += Time.deltaTime;
        if (restockTimer > restockInterval)
            restockTimer = 0;

        if (restockTimer == 0)
            foreach (GameObject spawn in candySpawnLocations)
                spawn.GetComponent<ItemSpawnpoint>().Spawn();
    }
}
