using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using CodeMonkey.Utils;

// This is a combination of the EnemyAI and EnebyScript scripts with some additions
public class EnemyGeneral : MonoBehaviour
{

    [SerializeField] public LayerMask layerMask;

    // Variables concerning shooting
    public Transform firePoint;
    public int damage = 20;
    public GameObject impactEffect;
    public Animator FlashAnimator;
    public AudioSource Shot;
    public float FireRate = 0.5f;
    public float Spread = 10f;

    public LineRenderer bullletTrail;

    // Variables concerning the A* pathfinding
    public Transform target;
    public float speed = 20;
    public float nextWaypointDistance = 3;
    public float UpdateTime = 0.5f; 
    private Path path;
    private int currentWaypoint;
    private bool reachedEndOfPath = false;
    private Seeker seeker;

    // The distance in which the enemy can see the player
    public int viewDistance = 100;

    // General variables for behaviour and health
    public int maxHealth = 100;
    public int currentHealth;
    public bool alive = true;
    public bool hunting = false;
    private Animator animator;
    private bool CRIsRunning = false;
    private bool ShootCRIsRunning = false;
    public bool Engaging = false;
    public float EngageTime = 1;
    public bool hasLos;

    private Rigidbody2D rb;

    private bool ranLastCycle = false;

    public int EnemyFOV = 45;


    void Start()
    {
        // Asigning components, setting health to correct value and running the pathfinding script
        currentHealth= maxHealth;
        animator = GetComponentInChildren<Animator>();
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        InvokeRepeating("UpdatePath", 0, UpdateTime);
    }

    public void TakeDamage (int damage)
    {
        // Enemy will take damage and die if health is <= 0
        currentHealth -= damage;
        if(currentHealth <= 0 && alive){
            die();
        }
        // If shot, the enemy will begin to hunt the player
        StartCoroutine(SpottedPlayerCoroutine());
    }

    void die()
    {
        // disables enemy and switches animations states
        alive = false;
        animator.SetTrigger("Dead");
    }


    void OnPathComplete(Path p)
    {
        // Uppdates path and resets waypoint
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    void UpdatePath ()
    {
        // Updates the path 
        if(seeker.IsDone())
        seeker.StartPath(rb.position, target.position, OnPathComplete);
    }

    void run() 
    {
        // Checks if it has a path and if it has reached the end of that path
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

            // Calculates direction and accelerates in that direction
            Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
            Vector2 velocity = direction * speed * Time.deltaTime;
            rb.AddForce(velocity);

            // Determines distance to current waypoint
            float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

            // Determines desired rotation and sets it to that rotation.
            float desiredRot = Mathf.Atan2(rb.velocity.y, rb.velocity.x)* Mathf.Rad2Deg - 90;
            rb.rotation = UtilsClass.GetAngleFromVector(new Vector2(rb.velocity.x, rb.velocity.y));

            // If the distance to the next waypoint is less then the distance to the current waypoint, go to next waypoint
            if (distance < nextWaypointDistance)
            {
                currentWaypoint++;
            }

            // Sets the peramiter for the enemy to run
            animator.SetBool("Running", true);
    }

    void Shoot()
    {
        // Shoots a raycast with some spread from the muzzle and gives the hitInfo variable info about the hit target
        RaycastHit2D hitInfo = Physics2D.Raycast(firePoint.position, UtilsClass.GetVectorFromAngle(UtilsClass.GetAngleFromVector(firePoint.right) + Random.Range(-Spread/2, Spread/2)));
        
        SpawnBulletTrail(hitInfo.point);
        // Activates the Shoot animation
        FlashAnimator.SetTrigger("Shoot");
        // PÅlays a sound for the shot
        Shot.Play(0);

        // If something has been hit, look for the playerMovment script (I should have changed that to a more descriptive name) and activates take damage function
        if(hitInfo)
        {
            hitInfo.transform.GetComponent<PlayerMovment>().TakeDamage(damage);

            // Makes a hit effect on the target
            Instantiate(impactEffect, hitInfo.point, transform.rotation);
        }
    }

    void hasLosToPlayer(int FOV)
    {
        // Gets the angle to the player
        float targetAngle = UtilsClass.GetAngleFromVector(new Vector2(target.position.x, target.position.y)-rb.position);

        // Casts a raycast towards the player, this is for checking if there are objects in the way
        RaycastHit2D raycastHit2D = Physics2D.Raycast(rb.position, new Vector2(target.position.x, target.position.y)- rb.position, viewDistance, ~layerMask);

        // If the player is in the field of view continue
        if (rb.rotation + FOV > targetAngle && rb.rotation - FOV < targetAngle) 
        {
            // This checks if there are objects in the way, if not the enemy has Line Of Sight
            if (raycastHit2D.collider.gameObject.name == "PlayerOrigin")
            {
                hasLos = true;
            }else hasLos = false;
        } else hasLos = false;
    }



    void SpawnBulletTrail(Vector3 hitPoint)
    {
        // Instantiates the effect at muzzle
        GameObject bulletTrailEffect = Instantiate(bullletTrail.gameObject,firePoint.position, Quaternion.identity);
        // Gets the LineRenderer component
        LineRenderer lineR = bulletTrailEffect.GetComponent<LineRenderer>();

        // Sets position 0 to muzzle
        lineR.SetPosition(0, firePoint.position);
        if (hitPoint != null){
            // Sets position 1 to the hit target
            lineR.SetPosition(1, hitPoint);
        }else lineR.SetPosition(1, firePoint.right*2);
        // Destroys the effect after 0.1 seconds
        Destroy(bulletTrailEffect, 0.1f);
    }

    IEnumerator SpottedPlayerCoroutine()
    {
        CRIsRunning = true;

        yield return new WaitForSeconds(EngageTime);

        // Stops hunting and starts engaging
        Engaging = true;
        hunting = false;
        CRIsRunning = false;
    }

    IEnumerator ShootAtPlayerCoroutine()
    {
        ShootCRIsRunning = true;
        //Waits a short time so it doesnt shoot every frame
        yield return new WaitForSeconds(FireRate);

        Shoot();
        ShootCRIsRunning = false;
    }

    void FixedUpdate()
    {
        if (alive)
        {
            // If it can see the player it will run the SpottedPlayerCoroutine
            hasLosToPlayer(EnemyFOV);
            if(hasLos && !Engaging && !CRIsRunning)
            {
                StartCoroutine(SpottedPlayerCoroutine());
            }
            if (hunting){
                run();
            }

            // If engaging, do not hunt or run, turn toward player, if it also has Line Of Sight, shoot
            if (Engaging)
            {
                hunting = false;
                animator.SetBool("Running", false);
                rb.rotation = UtilsClass.GetAngleFromVector(new Vector2(target.position.x, target.position.y) - rb.position);
                if(!ShootCRIsRunning)
                {
                    StartCoroutine(ShootAtPlayerCoroutine());
                }
                hasLosToPlayer(EnemyFOV);
                // if lost Line of Sight, hunt player
                if(!hasLos)
                {
                    Engaging = false;
                    hunting = true;
                }
            }
        }
    }
}
