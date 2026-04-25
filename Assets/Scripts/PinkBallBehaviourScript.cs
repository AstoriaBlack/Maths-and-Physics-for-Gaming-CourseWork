using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PinkBallBehaviourScript : MonoBehaviour
{
    //Waypoints variables
    //Making waypoints gameobjects draggable from the inspector

    [SerializeField] Transform waypoint1;
    [SerializeField] Transform waypoint2;
    [SerializeField] Transform waypoint3;
    [SerializeField] Transform waypoint4;

    //Settings variables
    [SerializeField] float speed = 10f; //overall moving speed of the ball
    [SerializeField] float slowRadius = 2f; //how close before starting to slow down

    //Private states variable
    Transform[] waypoints; //array of the waypoints
    int currentIndex = 0; //index of the current waypoint
    float currentSpeed; //current speed of the ball

    // Start is called before the first frame update
    void Start()
    {
        //Building the arra of waypoints from the inspector variables
        waypoints = new Transform[] {waypoint1, waypoint2, waypoint3, waypoint4};

        //starts with the full speed
        currentSpeed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        //Getting the viewpoint we are heading towards
        Transform target = GetCurrentWaypoint();
        
        //Adjuust speed based on how close we are to the target (ease in / ease out)
        currentSpeed = GetEasedSpeed(target);

        //Move towards the target waypoint
        MoveToward(target);

        //Check if have arrived, and if so, update to the next waypoint
        if (HasArrived(target))
        {
            //move to the next index
        currentIndex++;

        //if we are gone past the last waypoint, loop back to the first one
        currentIndex = currentIndex % waypoints.Length;
        }
    }

        Transform GetCurrentWaypoint()
        {
            //Simply return whichever waypoint we are currently targetig
            return waypoints[currentIndex];
        }

        float GetEasedSpeed(Transform target)
        {
            //getting the vector to the target
            Vector3 direction = target.position - transform.position;

            //getting the distance (the length/magnitude of that direction vector)
            float distance = direction.magnitude;

            //If we are within the slow radius, we want to slow down
            if (distance < slowRadius)
            {
                //Calculate the percentage of the slow radius we are within
                float ratio = distance / slowRadius;

                //Return the speed scaled by that percentage (so it slows down as we get closer)
                return speed * ratio;
            }
            return speed; //Otherwise, we are outside the slow radius, so we can go at full speed

        }

        void MoveToward(Transform target)
        {
            //getting the direction vector toward the target (unit vector)
            Vector3 direction = GetDirection(target);

            //moving the sphere along that direction
            transform.position += direction * currentSpeed * Time.deltaTime;

        }

        Vector3 GetDirection(Transform target)
        {
            //vector from current position to target position
            Vector3 direction = target.position - transform.position;

            //normalizing to get a unit vector (since it's length = 1)
            direction = direction.normalized;

            return direction;
        }   

        bool HasArrived(Transform target)
        {
            //get distance to current waypoint
            Vector3 direction = target.position - transform.position;
            float distance = direction.magnitude;

            //if, we are very close, then we have arrived
            return distance < 0.9f; //small threshold will make sure it doesn't overshoot and miss the waypoint
        }

    
}
