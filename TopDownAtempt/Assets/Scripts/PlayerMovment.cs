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

    //public Animator animator;

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
        if(currentHealth < 0){
            die();
            Debug.Log("it wont run here");
            SceneManager.LoadScene(0);
        }
        
    }

    // IEnumerator Died()
    // {
    //     yield return new WaitForSeconds(1);
    //     SceneManager.LoadScene(0);
    // }

    void die()
    {
        alive = false;
        //animator.SetTrigger("Dead");
        SceneManager.LoadScene(0);

    }

    // Update is called once per frame
    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

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

        Vector2 lookDir = mousePos - rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        fielfOfView.SetAimDirection(lookDir);
        fielfOfView.SetOrigin(GetComponent<Transform>().position);
        rb.rotation = angle;
    }
}
