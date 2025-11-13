using UnityEngine;

public class Shoot : MonoBehaviour
{
    [SerializeField] private GameObject candyPrefab;
    [SerializeField] private Transform transformRoot;
    [SerializeField] private float forceMultiplier = 3;
    [SerializeField] private Vector3 baseDirection = new Vector3(0, 0.5f, 1);
    [Tooltip("Maximum rotational variance in degrees of the direction")]
    [SerializeField] private float randomizationFactor = 45;
    [SerializeField] private int count = 5;
    [SerializeField] private float fireRate = 1.5f;
    [SerializeField] private AudioClip throwSound;

    private float cooldown;

    private InputActions input;

    private void Awake()
    {
        input = new();
    }
    private void OnEnable()
    {
        input.Enable();
    }
    private void OnDisable()
    {
        input.Disable();
    }

    void Update()
    {
        if (cooldown > 0f)
            cooldown -= Time.deltaTime;
        if (input.Player.Shoot.WasPressedThisFrame())
        {
            ThrowCandy();
            cooldown = fireRate;
        }
    }
    
    private void ThrowCandy()
    {
        if (GameManager.active.candy < count || cooldown > 0f) return;

        GameManager.active.candy -= count;
        for (int i = 0; i < count; i++)
        {
            Rigidbody rb = Instantiate(candyPrefab, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
            Vector3 dir = Quaternion.Euler(
                Random.Range(-randomizationFactor, randomizationFactor),
                Random.Range(-randomizationFactor, randomizationFactor),
                0
            ) * transformRoot.rotation * baseDirection;
            rb.AddForce(dir.normalized * forceMultiplier, ForceMode.Impulse);
        }

        GameManager.active.universalSoundEffect.PlayOneShot(throwSound);

        UIManager.Instance.UpdateCandy(GameManager.active.candy);
    }

}
