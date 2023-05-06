using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Mob : Block
{
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private bool randomWaypointSelect;
    [SerializeField] private bool facingRight;
    private Rigidbody2D rb;
    [SerializeField] private float moveSpeed;
    [HideInInspector] public List<Waypoint> waypoints = new();
    private int currentWaypoint;
    private Vector2 prevPosition;
    private float timeInSameSpot = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        prevPosition = transform.position;
    }

    private void Update()
    {
        if (sr != null)
        {
            sr.flipX = (facingRight) ? rb.velocity.x < 0 : rb.velocity.x > 0;
        }

        if (waypoints[currentWaypoint].Reached(transform))
        {
            SelectNewWaypoint();
        }
        else
        {
            Vector2 moveDir = waypoints[currentWaypoint].transform.position - transform.position;
            moveDir.Normalize();
            rb.velocity = moveDir * moveSpeed;

            if (Vector2.Distance(transform.position, prevPosition) < 0.01f)
            {
                timeInSameSpot += Time.deltaTime;
                if (timeInSameSpot >= 0.5f)
                {
                    timeInSameSpot = 0f;
                    SelectNewWaypoint();
                }
            }
            else
            {
                timeInSameSpot = 0f;
            }

            prevPosition = transform.position;
        }
    }

    private void SelectNewWaypoint()
    {
        if (randomWaypointSelect)
        {
            currentWaypoint = Random.Range(0, waypoints.Count);
        }
        else
        {
            currentWaypoint++;
        }
        if (currentWaypoint == waypoints.Count) currentWaypoint = 0;
    }
}
