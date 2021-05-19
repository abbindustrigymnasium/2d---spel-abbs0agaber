using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    // Start is called before the first frame update
    public int maxHealth = 100;
    public int currentHealth;
    public bool alive = true;
    public Rigidbody2D rb;
    public Animator animator;

    void Start()
    {
        currentHealth= maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if(alive)
        {

        }
    }

    public void TakeDamage (int damage)
    {
        currentHealth -= damage;
        if(currentHealth <= 0 && alive){
            die();
        }
    }
    void die()
    {
        Debug.Log("Enemy died");
        alive = false;
        animator.SetTrigger("Dead");
    }
}
