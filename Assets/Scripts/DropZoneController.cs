using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropZoneController : MonoBehaviour
{
    //private state variables
    TetherController tetherController;
    // Start is called before the first frame update
    void Start()
    {
        tetherController = FindObjectOfType<TetherController>();
    }

    //OnTriggerEnter is called when something enters a trigger collider
    void OnTriggerEnter(Collider other)
    {
        //Chckeing if the boulder entered the drop zone
        bool isBoulder = other.CompareTag("BoulderLight") ||
                        other.CompareTag("BoulderMedium") ||
                        other.CompareTag("BoulderHeavy");

        //only counting the delivery if rocket actually has it on tether
        if (isBoulder && tetherController.HasBoulder())
        {
            tetherController.DetachBoulder();
            Debug.Log("The boulder deleivered!");
        }
    }
}
