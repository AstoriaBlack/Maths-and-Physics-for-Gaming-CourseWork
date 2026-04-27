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
        promptText.text = "";

        nearestBoulder = FindNearestBoulder();

        // Case 1: no boulder attached — look for one to pick up
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
        // Case 2: carrying a boulder — offer to drop
        else
        {
            promptText.text = "Press E to drop boulder";

            if (Input.GetKeyDown(KeyCode.E))
            {
                DetachBoulder();
                return;
            }
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
            // Direction vector from anchor to boulder — uses Vector3 magnitude (module pattern)
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

        // Add FixedJoint TO the boulder, connected back to the rocket's Rigidbody.
        // This keeps the tether anchor safely parented under the rocket.
        fixedJoint = boulder.AddComponent<FixedJoint>();
        fixedJoint.connectedBody = GetComponent<Rigidbody>();

        gameController.SetCurrentBoulder(boulder.tag);

        Debug.Log("Picked up: " + boulder.tag);
    }

    public void DetachBoulder()
    {
        // Zero out boulder velocity before detaching so it doesn't
        // fly off carrying the rocket's momentum
        if (attachedBoulder != null)
        {
            Rigidbody boulderRb = attachedBoulder.GetComponent<Rigidbody>();
            if (boulderRb != null)
            {
                boulderRb.velocity        = Vector3.zero;
                boulderRb.angularVelocity = Vector3.zero;
            }
        }

        if (fixedJoint != null)
        {
            Destroy(fixedJoint);
            fixedJoint = null;
        }

        attachedBoulder = null;
        gameController.BoulderDelivered();

        Debug.Log("Boulder released!");
    }

    public void ResetTether()
    {
        if (fixedJoint != null) Destroy(fixedJoint);
        fixedJoint    = null;
        attachedBoulder = null;
        promptText.text = "";
    }

    public bool HasBoulder()
    {
        return attachedBoulder != null;
    }
}