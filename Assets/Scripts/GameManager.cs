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
    public string projectileTag = "Projectile";
    public float restockInterval = 30f;
    public string candySpawnTag = "candySpawnpoint";
    public string floorClerkSpawnTag = "fcSpawnpoint";
    public string cashierSpawnTag = "caSpawnpoint";
    public AudioSource universalSoundEffect;
    public AudioSource universalMusic;

    [Header("Game Data")]

    public int candy = 0;
    private GameObject[] candySpawnLocations;
    private Vector3[] cashierSpawnpoints;
    private Vector3[] floorClerkSpawnpoints;
    public float restockTimer = 0;

    public BatchSpawner cashierSpawner;
    public CappedRandomSpawner floorClerkSpawner;

    void Awake()
    {
        active = this;

        restockTimer = 0;
    }

    void Start()
    {
        mapGenerator.GenerateMap();

        GameObject[] temp;

        // Floor clerk spawn
        int i = 0;
        floorClerkSpawnpoints = new Vector3[(temp = GameObject.FindGameObjectsWithTag(floorClerkSpawnTag)).Length];
        foreach (GameObject spawn in temp)
            floorClerkSpawnpoints[i++] = spawn.transform.position;
        floorClerkSpawner.locations = floorClerkSpawnpoints;

        // Cashier spawn
        i = 0;
        cashierSpawnpoints = new Vector3[(temp = GameObject.FindGameObjectsWithTag(cashierSpawnTag)).Length];
        foreach (GameObject spawn in temp)
            cashierSpawnpoints[i++] = spawn.transform.position;
        cashierSpawner.locations = cashierSpawnpoints;

        // Candy spawn
        candySpawnLocations = GameObject.FindGameObjectsWithTag(candySpawnTag);
        foreach (GameObject spawnpoint in candySpawnLocations)
            spawnpoint.GetComponent<ItemSpawnpoint>().Initialize(floorClerkSpawnpoints, cashierSpawnpoints);

        // Spawn cashiers
        cashierSpawner.TrySpawn();
    }

    void Update()
    {
        if (restockTimer == 0)
            foreach (GameObject spawn in candySpawnLocations)
                spawn.GetComponent<ItemSpawnpoint>().Spawn();

        restockTimer += Time.deltaTime;
        if (restockTimer > restockInterval)
            restockTimer = 0;
    }

    void FixedUpdate()
    {
        if (floorClerkSpawner.continuous)
            floorClerkSpawner.TrySpawn();
    }
}
