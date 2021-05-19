using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using CodeMonkey.Utils;

public class EnemyGeneral : MonoBehaviour
{

    [SerializeField] public LayerMask layerMask;

    public Transform firePoint;
    public int damage = 20;
    public GameObject impactEffect;
    public Animator FlashAnimator;
    public AudioSource Shot;
    public float FireRate = 0.5f;
    public float Spread = 10f;

    public LineRenderer bullletTrail;

    public Transform target;
    public float speed = 20;
    public float nextWaypointDistance = 3;
    public float UpdateTime = 0.5f; 

    public int viewDistance = 100;

    public int maxHealth = 100;
    public int currentHealth;
    public bool alive = true;
    public bool hunting = false;
    private Animator animator;

    private Path path;
    private int currentWaypoint;
    private bool reachedEndOfPath = false;

    private Seeker seeker;
    private Rigidbody2D rb;

    private bool ranLastCycle = false;

    public int EnemyFOV = 45;
    public float EngageTime = 1;
    public bool hasLos;
    private bool CRIsRunning = false;
    private bool ShootCRIsRunning = false;
    public bool Engaging = false;


    void Start()
    {
        currentHealth= maxHealth;
        animator = GetComponentInChildren<Animator>();
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        InvokeRepeating("UpdatePath", 0, UpdateTime);
    }

    public void TakeDamage (int damage)
    {
        currentHealth -= damage;
        if(currentHealth <= 0 && alive){
            die();
        }
        StartCoroutine(SpottedPlayerCoroutine());
    }

    void die()
    {
        alive = false;
        animator.SetTrigger("Dead");
    }


    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    void run() 
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

            rb.AddForce(velocity);

            float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

            float desiredRot = Mathf.Atan2(rb.velocity.y, rb.velocity.x)* Mathf.Rad2Deg - 90;

            rb.rotation = UtilsClass.GetAngleFromVector(new Vector2(rb.velocity.x, rb.velocity.y));

            if (distance < nextWaypointDistance)
            {
                currentWaypoint++;
            }

            animator.SetBool("Running", true);
    }

    void Shoot()
    {
        ShootCRIsRunning = false;
        RaycastHit2D hitInfo = Physics2D.Raycast(firePoint.position, UtilsClass.GetVectorFromAngle(UtilsClass.GetAngleFromVector(firePoint.right) + Random.Range(-Spread/2, Spread/2)));
        SpawnBulletTrail(hitInfo.point);
        FlashAnimator.SetTrigger("Shoot");
        Shot.Play(0);

        if(hitInfo)
        {
            hitInfo.transform.GetComponent<PlayerMovment>().TakeDamage(damage);

            Instantiate(impactEffect, hitInfo.point, transform.rotation);
        }
    }

    void hasLosToPlayer(int FOV)
    {
        float targetAngle = UtilsClass.GetAngleFromVector(new Vector2(target.position.x, target.position.y)-rb.position);

        RaycastHit2D raycastHit2D = Physics2D.Raycast(rb.position, new Vector2(target.position.x, target.position.y)- rb.position, viewDistance, ~layerMask);

        if (rb.rotation + FOV > targetAngle && rb.rotation - FOV < targetAngle) 
        {

            if (raycastHit2D.collider.gameObject.name == "PlayerOrigin")
            {
                hasLos = true;
            }else hasLos = false;
        } else hasLos = false;
    }

    void UpdatePath ()
    {
        if(seeker.IsDone())
        seeker.StartPath(rb.position, target.position, OnPathComplete);
    }

    void SpawnBulletTrail(Vector3 hitPoint)
    {
        GameObject bulletTrailEffect = Instantiate(bullletTrail.gameObject,firePoint.position, Quaternion.identity);

        LineRenderer lineR = bulletTrailEffect.GetComponent<LineRenderer>();

        lineR.SetPosition(0, firePoint.position);
        if (hitPoint != null){
            lineR.SetPosition(1, hitPoint);
        }else lineR.SetPosition(1, firePoint.right*2);

        Destroy(bulletTrailEffect, 0.1f);
    }

    IEnumerator SpottedPlayerCoroutine()
    {
        CRIsRunning = true;

        yield return new WaitForSeconds(EngageTime);

        Engaging = true;
        hunting = false;
        CRIsRunning = false;
    }

    IEnumerator ShootAtPlayerCoroutine()
    {
        ShootCRIsRunning = true;
        yield return new WaitForSeconds(FireRate);

        Shoot();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //rb.velocity = new Vector2(0,0);
        if (alive)
        {
            hasLosToPlayer(EnemyFOV);
            if(hasLos && !Engaging && !CRIsRunning)
            {
                StartCoroutine(SpottedPlayerCoroutine());
            }
            if (hunting){
                run();
            }
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
                if(!hasLos)
                {
                    Engaging = false;
                    hunting = true;
                }
            }
        }
    }
}
