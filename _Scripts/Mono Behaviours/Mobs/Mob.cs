using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Mob : Block
{
    public List<Waypoint> waypoints = new();
    [SerializeField] private float moveSpeed;
    private int currentWaypoint;
    private Rigidbody2D rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        if (waypoints[currentWaypoint].Reached(transform))
        {
            currentWaypoint++;
            if (currentWaypoint == waypoints.Count) currentWaypoint = 0;
        }
        else
        {
            Vector2 moveDir = waypoints[currentWaypoint].transform.position - transform.position;
            moveDir.Normalize();
            rb.velocity = moveDir * moveSpeed;
        }
    }
}
