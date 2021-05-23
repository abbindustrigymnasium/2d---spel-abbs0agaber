using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public Transform firePoint;
    public int damage = 50;
    public GameObject impactEffect;
    public GameObject tracerEffect;
    public Animator animator;
    public AudioSource Shot;
    public LineRenderer bullletTrail;


    public float bulletForce = 20f;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
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

    void Shoot()
    {
        // Shoots raycast from muzzle
        RaycastHit2D hitInfo = Physics2D.Raycast(firePoint.position, firePoint.right);

        SpawnBulletTrail(hitInfo.point);
        animator.SetTrigger("Shoot");
        Shot.Play(0);
        // If something has been hit Instansiate hiteffect and Makes enemy take damage
        if(hitInfo)
        {
            Instantiate(impactEffect, hitInfo.point, transform.rotation);
            Debug.Log(hitInfo.transform.name);
            hitInfo.transform.GetComponent<EnemyGeneral>().TakeDamage(damage);
        }
    }
}
