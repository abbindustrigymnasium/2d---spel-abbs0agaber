using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    public Transform target;

    public float speed = 20;
    public float nextWaypointDistance = 3;
    public float UpdateTime = 0.5f; 
    
    private Path path;
    private int currentWaypoint;
    private bool reachedEndOfPath = false;

    private Seeker seeker;
    private Rigidbody2D rb;


    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        InvokeRepeating("UpdatePath", 0, UpdateTime);
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    void UpdatePath ()
    {
        if(seeker.IsDone())
        seeker.StartPath(rb.position, target.position, OnPathComplete);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(path == null)
            return;
        if(currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        } else 
        {
            reachedEndOfPath = false;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 velocity = direction * speed * Time.deltaTime;

        rb.velocity = (velocity);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }

    }
}
