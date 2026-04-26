using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetherController : MonoBehaviour
{
    //Varriables setting up for inspector to change
    [SerializeField] Transform tetherAnchor;
    [SerializeField] float pickupRadius = 2f;

    //Pivate variables
    GameObject attachedBoulder;
    FixedJoint fixedJoint;
    GameController gameController;

    // Start is called before the first frame update
    void Start()
    {
        gameController = FindObjectOfType<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        //Only try to pick up if we don't already have a boulder attached
        if (attachedBoulder == null)
        {
            TryPickupBoulder();
        }
    }

    void TryPickupBoulder()
    {
        //check rach boulder tag seperately
        CheckAndPickup("BoulderLight");
        CheckAndPickup("BoulderMedium");
        CheckAndPickup("BoulderHeavy");
    }

    void CheckAndPickup(string bouldTag)
    {
        //this will make sure tether won't attach another if already one attached
        if (attachedBoulder != null) return;

        //Getting all boulders with specific tag
        GameObject[] boulders = GameObject.FindGameObjectsWithTag(bouldTag);

        foreach (GameObject boulder in boulders)
        {
            //Get direction vector from anchor to boulder 
            Vector3 direction = boulder.transform.position - tetherAnchor.position;

            //Get distance using magnitude
            float distance = direction.magnitude;

            //if close enough, attach the boulder
            if (distance < pickupRadius)
            {
                AttachBoulder(boulder);
                return;
            }
        }
    }

    void AttachBoulder(GameObject boulder)
    {
        attachedBoulder = boulder; //store the reference to the attached boulder

        //Adding a fixedJoint to the anchor point at the bottom of the tether
        fixedJoint = tetherAnchor.gameObject.AddComponent<FixedJoint> ();

        //Connecting it to the boulder's rigibody
        fixedJoint.connectedBody = boulder.GetComponent<Rigidbody>();

        //Telling the gameController which boulder type for fuel drain rate
        gameController.setCurrentBoulder(boulder.tag);

        Debug.Log("Picked up: " + boulder.tag);
    }

    public void DetachBoulder()
    {
        if (fixedJoint != null)
        {
            Destroy(fixedJoint); //remove the joint to detach
            fixedJoint = null;
        }

        attachedBoulder = null; //clear the reference to the boulder

        //Telling the game controller boulder was delivered 

        gameController.BoulderDelivered();
    }

    public void ResetTether()
    {
        if (fixedJoint != null) Destroy(fixedJoint);
        fixedJoint = null;
        attachedBoulder = null;
    }

    public bool HasBoulder()
    {
        return attachedBoulder != null;
    }
}
