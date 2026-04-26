using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketController : MonoBehaviour
{
    //Setting up variables for inspector to change
    [SerializeField] float thrustForce = 10f;
    [SerializeField] float fuelDrainThrust = 1f;

    //private variable for the use of script
    Rigidbody rb;
    GameController gameController;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        gameController = FindObjectOfType<GameController>();
        Physics.gravity = new Vector3(0f, -2f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
