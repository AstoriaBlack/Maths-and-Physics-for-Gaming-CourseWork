using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    //Varriables setting up for inspector to change
    [SerializeField] float rotationSpeed = 45f;
    [SerializeField] float oscillateHeight = 1.5f;
    [SerializeField] float oscillateSpeed = 1f;

    //private state variables
    Vector3 startPosition;
    GameController gameController;

    // Start is called before the first frame update
    void Start()
    {
        //Remember where this obstacle started (center of socillation)
        startPosition = transform.position;

        gameController = FindObjectOfType<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        RotateObstacle();
        OscillateObstacle();
    }

    void RotateObstacle()
    {
        //Rotate around Y axis every frame
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }

    void OscillateObstacle()
    {
        //Mathf.Sin creates a smooth wave between -1 and +1
        float sineWave = Mathf.Sin(Time.time * oscillateSpeed);

        //Scaling the wave by height to get the Y offset from the start position
        float Yoffset = startPosition.y + (sineWave * oscillateHeight);

        //Applying the new Y position while keeping X and Z the same
        transform.position = new Vector3(startPosition.x, Yoffset, startPosition.z);
    }

    //this is called automatically by Unity when something collides with the obstacle's collider
    //a unity built in method so no need to call it in other script
    void OnCollisionnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Rocket"))
        {
            gameController.DrainFuel(10f);
            Debug.Log("Rocket hit obstacle! -10 fuel drained!");
        }
    }
}
