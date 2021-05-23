using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovment : MonoBehaviour
{
    [SerializeField] public FOV fielfOfView;
    public float moveSpeed = 5;

    public Rigidbody2D rb;
    public Camera cam;

    public int maxHealth = 100;
    public int currentHealth;
    public bool alive = true;

    Vector2 movement;
    Vector2 mousePos;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage (int damage)
    {
        currentHealth -= damage;
        Debug.Log(currentHealth);
        if(currentHealth <= 0){
            die();
            SceneManager.LoadScene(0);
        }
        
    }

    void die()
    {
        // If you die
        alive = false;
        SceneManager.LoadScene(0);
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Gets mouse positon
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }
    }
    void FixedUpdate() 
    {
        rb.velocity = (new Vector2(0,0));
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);

        // Gets angle for Player
        Vector2 lookDir = mousePos - rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;

        fielfOfView.SetAimDirection(lookDir);
        fielfOfView.SetOrigin(GetComponent<Transform>().position);
        // Rotates Player
        rb.rotation = angle;
    }
}
