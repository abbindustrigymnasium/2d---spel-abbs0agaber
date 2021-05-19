using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{

    public GameObject hitEffect;

    void OnCollisionEnter2D(Collision2D collision)
    {
        Instantiate(hitEffect, transform.position, Quaternion.identity);
        Debug.Log("yes");
        Destroy(hitEffect, 5f);
        Destroy(gameObject);
    }
}
