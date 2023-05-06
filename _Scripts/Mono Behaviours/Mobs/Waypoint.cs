using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    [SerializeField] private Mob waypointManager;

    private void Start()
    {
        waypointManager.waypoints.Add(this);
    }
    public bool Reached(Transform trans)
    {
        return Vector3.Distance(trans.position, transform.position) < 0.3f;
    }
}
