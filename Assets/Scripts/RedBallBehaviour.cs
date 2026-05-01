using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedBallBehaviour : MonoBehaviour
{
    // Values show in the inspector so it can be changed in the editor
    [SerializeField] Vector3 launchVel = new Vector3(7f, 8f, 4f); //Initial velocity
    [SerializeField] float e = 0.8f; //Coefficient of restitution
    [SerializeField] float stopThreshold = 0.01f; //Threshold velocity

    // Private internal state variables
    Vector3 velocity;
    Vector3 gravity = new Vector3(0f, -9.81f, 0f); //Gravity acceleration
    bool isMoving = true; //to check if the ball is still moving
    
    // Start is called before the first frame update
    void Start()
    {
        //setting the correct gravity for task 1 scene
        Physics.gravity = new Vector3(0f, -9.81f, 0f);
        
        //set the ball starting velocity to whatever is set in the inspector
        velocity = launchVel;
    }

    // Update is called once per frame
    void Update()
    {
        //This would ensure if the ball has stopped moving, it won't continue to update its position or velocity
        if(!isMoving) return;

        //Apply gravity and move the ball
        ApplyGravity();
        MoveBall();

        //Check if the ball has hit the ground
        if (transform.position.y <= 0f)
        {
            HandleBounce();
        }
    }

    void ApplyGravity()
    {
        // Euler integration - velocity changes by gravity each frame
        // gravity * Time.deltaTime gives us the small velocity change this frame
        velocity += gravity * Time.deltaTime; //Apply gravity to velocity
    }

    void MoveBall()
    {
        //Euler integration - position changes by velocity each frame
        // velocity * Time.deltaTime gives us the small position change this frame
        transform.position += velocity * Time.deltaTime; //Move the ball according to its velocity
        //Time.deltaTime is the time in seconds since the last frame, so this makes the movement frame-rate independent

    }

    void HandleBounce()
    {
        // Snap the ball exactly to the ground so it doesn't sink below
        transform.position = new Vector3(transform.position.x, 0f, transform.position.z);

        //Calcultae what the vertical speed would be after applying restitution
        float speedBeforeBounce = Mathf.Abs(velocity.y);
        float speedAfterBounce = speedBeforeBounce * e; //Apply restitution to vertical speed
        
        // Check if the bounce is too small to matter
        // We compare it to the original launch vertical speed
        if (speedAfterBounce / Mathf.Abs(launchVel.y) < stopThreshold)
        {
            isMoving = false;
            velocity = Vector3.zero; //Stop the ball completely
            Debug.Log("Ball has come to rest.");
            return;
        }

        // Reverse the vertical velocity and reduce it by e (the bounce)
        // Horizontal velocity (x and z) stays the same — no friction
        velocity.y = speedAfterBounce;

        // // Applying restitution
        //     velocity.y = -velocity.y * e;

        //     // Stop when very small bounce
        //     if (Mathf.Abs(velocity.y) < stopThreshold)
        //     {
        //         velocity = Vector3.zero;
        //         isMoving = false;
        //     }
        //this initial version of the bounce handling snippet has been replaced by the more accurate version that compare the bounce to the original launch speed, 
        // which is more intuitive and works better for different launch speeds
            
    }
}
