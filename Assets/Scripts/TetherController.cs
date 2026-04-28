using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TetherController : MonoBehaviour
{
    // --- Inspector Settings ---
    [SerializeField] Transform tetherAnchor;
    [SerializeField] float pickupRadius = 4f;
    [SerializeField] TMP_Text promptText;

    // --- Private State ---
    GameObject attachedBoulder;
    FixedJoint fixedJoint;
    GameObject nearestBoulder;
    GameController gameController;

    void Start()
    {
        gameController = FindObjectOfType<GameController>();
    }

    void Update()
    {
        // Clear prompt each frame
        if (promptText != null) promptText.text = "";

        nearestBoulder = FindNearestBoulder();

        // Case 1: no boulder — look for one to pick up
        if (attachedBoulder == null)
        {
            if (nearestBoulder != null)
            {
                promptText.text = "Press E to pick up " + nearestBoulder.tag;

                if (Input.GetKeyDown(KeyCode.E))
                {
                    AttachBoulder(nearestBoulder);
                    return;
                }
            }
        }
        // Case 2: carrying boulder — show info only
        else
        {
            promptText.text = "Fly to the drop zone!";
        }
    }

    GameObject FindNearestBoulder()
    {
        GameObject nearest = null;
        float nearestDistance = pickupRadius;

        nearest = CheckBoulderTag("BoulderLight",  nearest, ref nearestDistance);
        nearest = CheckBoulderTag("BoulderMedium", nearest, ref nearestDistance);
        nearest = CheckBoulderTag("BoulderHeavy",  nearest, ref nearestDistance);

        return nearest;
    }

    GameObject CheckBoulderTag(string tag, GameObject currentNearest, ref float nearestDistance)
    {
        GameObject[] boulders = GameObject.FindGameObjectsWithTag(tag);

        foreach (GameObject boulder in boulders)
        {
            // Direction vector from anchor to boulder — tutorial Vector3 pattern
            Vector3 direction = boulder.transform.position - tetherAnchor.position;
            float distance = direction.magnitude;

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                currentNearest = boulder;
            }
        }

        return currentNearest;
    }

    void AttachBoulder(GameObject boulder)
    {
        attachedBoulder = boulder;

        // Joint on boulder connected to rocket Rigidbody — anchor stays put
        fixedJoint = boulder.AddComponent<FixedJoint>();
        fixedJoint.connectedBody = GetComponent<Rigidbody>();

        gameController.SetCurrentBoulder(boulder.tag);

        Debug.Log("Picked up: " + boulder.tag);
    }

    // Called automatically by DropZoneController — no E press needed
    public void DeliverBoulder()
    {
        if (attachedBoulder == null) return;

        // Zero velocity so boulder stays in drop zone
        Rigidbody boulderRb = attachedBoulder.GetComponent<Rigidbody>();
        if (boulderRb != null)
        {
            boulderRb.velocity = Vector3.zero;
            boulderRb.angularVelocity = Vector3.zero;

            // Freeze so it stays put in drop zone
            boulderRb.constraints = RigidbodyConstraints.FreezeAll;
        }

        if (fixedJoint != null)
        {
            Destroy(fixedJoint);
            fixedJoint = null;
        }

        attachedBoulder = null;

        if (promptText != null) promptText.text = "";


        Debug.Log("Boulder delivered to drop zone!");

        // Tell GameController — counts toward win
        gameController.BoulderDelivered();
    }

    public void ResetTether()
    {
        if (fixedJoint != null)
        {
            Destroy(fixedJoint);
            fixedJoint = null;
        }

        attachedBoulder = null;

        if (promptText != null) promptText.text = "";
    }

    public bool HasBoulder()
    {
        return attachedBoulder != null;
    }
}