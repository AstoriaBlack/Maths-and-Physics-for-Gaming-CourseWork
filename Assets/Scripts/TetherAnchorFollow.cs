using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetherAnchorFollow : MonoBehaviour
{
    [SerializeField] Transform rocket;
    [SerializeField] float offsetBelow = 1.5f; // tweak this in inspector

    void LateUpdate()
    {
        // Locks anchor to exactly below the rocket in world Y, 
        // same world X, and the rocket's own world Z (so it never drifts)
        transform.position = new Vector3(
            rocket.position.x,
            rocket.position.y - offsetBelow,
            rocket.position.z          // stays flush with rocket in Z
        );
    }
}