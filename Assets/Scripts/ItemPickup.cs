using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public int quantity = 1;

    private bool playerInRange = false;
    private GameManager gameManager;

    private InputActions input;

    private void Awake()
    {
        input = new();

        gameManager = FindObjectOfType<GameManager>();
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
        if (playerInRange && input.Player.Pickup.WasPressedThisFrame())
            PickupItem();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    private void PickupItem()
    {
        // Candy stolen logic
        Debug.Log("Candy stolen!");

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 20f); // 20 units radius
        foreach (var col in hitColliders)
        {
            NPC npc;
            col.TryGetComponent<NPC>(out npc);

            if (npc != null && !Physics.Raycast(transform.position, col.transform.position - transform.position, Vector3.Distance(transform.position, col.transform.position), GameManager.active.obstacleLayers))
            {
                npc._SawCandyStolen = true;
            }
        }

        // Destroy or disable the item
        gameManager.candy += quantity;
        UIManager.Instance.UpdateCandy(gameManager.candy);
        Destroy(gameObject);
    }
}
