using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class TetherController : MonoBehaviour
{
    // Inspector variables
    [SerializeField] Transform tetherAnchor;
    [SerializeField] float pickupRadius = 4f;
    //added a text prompt to show pickup instructions rather than the tether automatically picking a boulder up
    [SerializeField] TMP_Text promptText;

    // privately used variables
    GameObject attachedBoulder;
    FixedJoint fixedJoint; //the joint that connect tether to the boulder when attached
    GameObject nearestBoulder;
    GameController gameController; //referencing to gamecontroller for picking up and delievering boulders

    void Start()
    {
        //finding the gamcontroller script in the scene
        gameController = FindObjectOfType<GameController>();
    }

    void Update()
    {
        // Resetting to empty string each frame
        if (promptText != null) promptText.text = "";
        
        //finding the nearest boulder to the rocket each frame
        nearestBoulder = FindNearestBoulder();

        // cheking if the rocket is currently carrying a boulder or not
        if (attachedBoulder == null)
        {
            if (nearestBoulder != null)
            {
                //prompting the player to pick up
                promptText.text = "Press 'E' to pick up " + nearestBoulder.tag;

                //checking if the player had pressed E to pick the boulder up
                //GetKeyDown is used instead of GetKey because we only need to pick the boulder up once per press
                if (Input.GetKeyDown(KeyCode.E))
                {
                    AttachBoulder(nearestBoulder);
                    //after attaching, it exists the Update() for the frame
                    return;
                }
            }
        }
        // if the rocket is already carrying a boulder, this will prompt the player to fly to drop zone
        else
        {
            promptText.text = "Go to the drop zone!";
        }
    }

    //returns the nearest boulder within the pickup radius, or null if there arent any
    GameObject FindNearestBoulder()
    {
        GameObject nearest = null;
        float nearestDistance = pickupRadius; //using pickup radius as the initial nearest distance
        
        //checking each type of boulder and updateing the nearest boulder if it finds one closer than the current nearest
        //ref passes nearestDistance by reference
        nearest = CheckBoulderTag("BoulderLight",  nearest, ref nearestDistance);
        nearest = CheckBoulderTag("BoulderMedium", nearest, ref nearestDistance);
        nearest = CheckBoulderTag("BoulderHeavy",  nearest, ref nearestDistance);

        return nearest;
    }

    //This method check the boulders with the given tag and returns within pickup radius
    GameObject CheckBoulderTag(string tag, GameObject currentNearest, ref float nearestDistance)
    {
        //find all boulders with the given tag and storing them in an array
        //since it only has one boulder for each type, this might feel excessive but this method is more scalable on the long run
        GameObject[] boulders = GameObject.FindGameObjectsWithTag(tag);
        
        //
        foreach (GameObject boulder in boulders)
        {
            //calculating the distance between the boulder and the tether anchor
            Vector3 direction = boulder.transform.position - tetherAnchor.position;
            //using magnitude to get the distance from the direction vector 
            float distance = direction.magnitude;

            //if the current boulder is nearer than the current nearest, updating the nearest boulder and distance
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                currentNearest = boulder;
            }
        }

        return currentNearest;
    }

    //this method is for attaching the boulder
    void AttachBoulder(GameObject boulder)
    {
        attachedBoulder = boulder;

        //adding a fixed joint component to the boulder 
        //we add the joint to boulder because when the fixedjoint is destroyed, it only affect the boulder
        fixedJoint = boulder.AddComponent<FixedJoint>();
        //connecting the joint to rocket rigidbody
        fixedJoint.connectedBody = GetComponent<Rigidbody>();

        //telling the gamecontrller which boulder is being attached
        gameController.SetCurrentBoulder(boulder.tag);

        Debug.Log("Picked up: " + boulder.tag);
    }

    //this method is for delivering the boulder to the drop zone
    public void DeliverBoulder()
    {
        //if there is no boulder attach, this will return early
        if (attachedBoulder == null) return;

        //this will kinda freeze the boulder after delivered
        Rigidbody boulderRb = attachedBoulder.GetComponent<Rigidbody>();
        if (boulderRb != null)
        {
            //setting the boulder's velocity to 0 so it stucks to the dropzone
            boulderRb.velocity = Vector3.zero;
            boulderRb.angularVelocity = Vector3.zero;

            // locks all x,y,z positions and rotation
            boulderRb.constraints = RigidbodyConstraints.FreezeAll;
        }

        ResetTether();

        Debug.Log("Boulder delivered to drop zone!");

        //telling the gamcontroller a boulder delivered to update
        gameController.BoulderDelivered();
    }

    //this method is called by gamecontroller when resetting the game, it will detach any attached boulder and clear the prompt text
    public void ResetTether()
    {
        
        if (fixedJoint != null)
        {
            //destroying the joint to detach the boulder from the rocket
            Destroy(fixedJoint);
            //resetting...
            fixedJoint = null;
        }
        
        //more resetting...
        attachedBoulder = null;

        //even more resetting...(clearing the prompt text)
        if (promptText != null) promptText.text = "";


    }

    //this method is for checking if the rocket is carrying a boulder, used in gamecontroller to determine if the player can deliver a boulder or not
    public bool HasBoulder()
    {
        return attachedBoulder != null;
    }
}