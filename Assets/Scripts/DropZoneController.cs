using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropZoneController : MonoBehaviour
{
    TetherController tetherController;

    // Track which boulders already delivered so trigger doesn't fire twice

    //making a list since it's more flexible than an array 
    List<GameObject> deliveredBoulders = new List<GameObject>();

    void Start()
    {
        //finding the tetherController script in the scene
        tetherController = FindObjectOfType<TetherController>();
    }

    //this method is called when the drop zone trigger collides with something presumbly a boulder
    //unity call this method automatically when any collider enters the trigger collider
    void OnTriggerEnter(Collider other)
    {
        bool isBoulder = other.CompareTag("BoulderLight") ||
                        other.CompareTag("BoulderMedium") ||
                        other.CompareTag("BoulderHeavy");
        
        //if the collider is not a boulder, it will ignore and exit the method
        if (!isBoulder) return;

        // Ignore if this boulder was already delivered
        //this prevents another count if the player tries to deliver the same boulder again by leaving and re-entering the trigger
        if (deliveredBoulders.Contains(other.gameObject)) return;

        // Only deliver if rocket is carrying this boulder
        //if the boulder rolled on it's own, it is not counted
        if (tetherController.HasBoulder())
        {
            //appends the delivered boulder to the list
            deliveredBoulders.Add(other.gameObject);
            //calling the DeliverBoulder method in tetherController 
        }
    }

    // Called by GameController on reset and clearing the delivered list
    public void ResetDropZone()
    {
        deliveredBoulders.Clear();
    }
}