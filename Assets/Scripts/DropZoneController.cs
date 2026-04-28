using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropZoneController : MonoBehaviour
{
    TetherController tetherController;

    // Track which boulders already delivered so trigger doesn't fire twice
    List<GameObject> deliveredBoulders = new List<GameObject>();

    void Start()
    {
        tetherController = FindObjectOfType<TetherController>();
    }

    void OnTriggerEnter(Collider other)
    {
        bool isBoulder = other.CompareTag("BoulderLight") ||
                         other.CompareTag("BoulderMedium") ||
                         other.CompareTag("BoulderHeavy");

        if (!isBoulder) return;

        // Ignore if this boulder was already delivered
        if (deliveredBoulders.Contains(other.gameObject)) return;

        // Only deliver if rocket is carrying this boulder
        if (tetherController.HasBoulder())
        {
            deliveredBoulders.Add(other.gameObject);
            tetherController.DeliverBoulder();
        }
    }

    // Called by GameController on reset — clear the delivered list
    public void ResetDropZone()
    {
        deliveredBoulders.Clear();
    }
}