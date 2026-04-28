using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    // --- Inspector Settings ---
    [SerializeField] float rotationSpeed = 45f;
    [SerializeField] float oscillateHeight = 1.5f;
    [SerializeField] float oscillateSpeed = 1f;

    // --- Private State ---
    Vector3 startPosition;
    GameController gameController;

    void Start()
    {
        startPosition = transform.position;
        gameController = FindObjectOfType<GameController>();
    }

    void Update()
    {
        RotateObstacle();
        OscillateObstacle();
    }

    void RotateObstacle()
    {
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }

    void OscillateObstacle()
    {
        // Sine wave for smooth up/down oscillation
        float sineWave = Mathf.Sin(Time.time * oscillateSpeed);
        float newY = startPosition.y + (sineWave * oscillateHeight);
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }

    // For non-trigger colliders
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Rocket"))
        {
            gameController.DrainFuel(10f);
            Debug.Log("Rocket hit obstacle! -10 fuel");
        }
    }

    // For trigger colliders — covers both cases
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Rocket"))
        {
            gameController.DrainFuel(10f);
            Debug.Log("Rocket hit obstacle! -10 fuel");
        }
    }
}