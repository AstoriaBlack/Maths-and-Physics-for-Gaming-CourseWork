using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    // variables for inspector to change
    [SerializeField] float rotationSpeed = 45f; //we go with45 deg per second
    [SerializeField] float oscillateHeight = 1.5f;
    [SerializeField] float oscillateSpeed = 1f; //sine wave frequency

    //private state variables
    Vector3 startPosition;
    GameController gameController;

    void Start()
    {
        
        // recording the start position of the obstacle
        startPosition = transform.position;
        //finding the gameController script in the scene
        gameController = FindObjectOfType<GameController>();
    }

    void Update()
    {
        RotateObstacle();
        OscillateObstacle();
    }

    //method to rotate the obstacle around the y axis
    void RotateObstacle()
    {
        //transfrm.rotate will rotate the object by Y axis for here
        //time.deltaTime is to make the rotation frame rate independant since it will rotate per second
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }
    
    //method to make the obstacle oscillate up and down
    void OscillateObstacle()
    
    {
        //sine wave will oscillate between -1 and 1
        //Time.time is the time since the start of the game, so it will only increase as the game goes on
        //so sine wave will continue to oscillate as time goes on
        //oscillateSpeed will control how many rad per seconds the sinewave goes through, so it controls speed of oscillation
        //multiplied by oscillate height to set the range of the sine wave to be between the two values
        float sineWave = Mathf.Sin(Time.time * oscillateSpeed);
        float newY = startPosition.y + (sineWave * oscillateHeight);
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }

    //this method will be called by unity when the obstacle collides with the rocket
    void OnCollisionEnter(Collision collision)
    {
        //checking if the object that collided with the obstacle is the rocket
        if (collision.gameObject.CompareTag("Rocket"))
        {
            gameController.DrainFuel(10f);
            Debug.Log("Rocket hit obstacle! -10 fuel");
        }
    }

    //there is also a trigger collider on the obstacle to make sure the player can't just avoid the collision by flying above or below it

    //this method will be called when the obstacle's trigger collider is entered by the rocket
    void OnTriggerEnter(Collider other)
    {
        //checking if the object that entered the trigger is the rocket
        if (other.CompareTag("Rocket"))
        {
            gameController.DrainFuel(10f);
            Debug.Log("Rocket hit obstacle! -10 fuel");
        }
    }
}