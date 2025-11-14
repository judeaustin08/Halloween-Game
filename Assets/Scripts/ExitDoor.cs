using UnityEngine;

public class ExitDoor : MonoBehaviour
{
    private bool playerInRange = false;
    InputActions input;

    void Awake()
    {
        input = new();
    }
    void OnEnable()
    {
        input.Enable();
    }
    void OnDisable()
    {
        input.Disable();
    }

    void Update()
    {
        if (playerInRange && input.Player.Pickup.WasPressedThisFrame())
            GameManager.active.EndGame();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }
}
