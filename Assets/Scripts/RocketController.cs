using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketController : MonoBehaviour
{
    //Setting up variables for inspector to change
    //To control the thrust force of the rocket
    [SerializeField] float thrustForce = 5f;
    //to calculate the fuel unit drains per second while thrusting
    [SerializeField] float fuelDrainThrust = 1f;

    //private variable for the use of script
    Rigidbody rb;
    //the gameController script for tracking score and fuel
    GameController gameController;

    // Start is called before the first frame update
    void Start()
    {
        //this will get the rigidbody component of the rocket and store it in the variable
        rb = GetComponent<Rigidbody>();
        //this will find the gameController script in the scene and store it in the variable
        //so I don't have to manually assign it in the inspector
        gameController = FindObjectOfType<GameController>();

        //this will set the global gravity for the whole scene to -2 instead of -9.81 for space feels
        Physics.gravity = new Vector3(0f, -2f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        //this will returns true every frame the key is held down 
        //GetKey is used instead of GetKeyDown because the rocket needs to keep applying thrust as long as the key held down
        if (Input.GetKey("up"))
        {
            ApplyThrust(Vector3.up);
        }

        if (Input.GetKey("down"))
        {
            ApplyThrust(Vector3.down);
        }

        if (Input.GetKey("left"))
        {
            ApplyThrust(Vector3.left);
        }

        if (Input.GetKey("right"))
        {
            ApplyThrust(Vector3.right);
        }
    }

//this method will apply force to the rocket in the given direction
//also will drain fuel 
    void ApplyThrust(Vector3 direction)
    {
        //this will push the rigibody in the given direction
        rb.AddForce(direction * thrustForce, ForceMode.Force); //ForceMode.Force means the force is applied continuously over time, that consider the mass as well
        gameController.DrainFuel(fuelDrainThrust * Time.deltaTime);
        //Time.deltaTime would ensure the fuel is draned per second, not per frame
        
    }

    //gameController will call this method to reset the rocket position and velocity when the game ended or restarted
    public void ResetRocket(Vector3 startPosition)
    {
        transform.position = startPosition;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}
