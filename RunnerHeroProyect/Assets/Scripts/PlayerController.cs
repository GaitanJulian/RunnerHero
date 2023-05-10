using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float jumpForce = 1.5f;
    public float jumpTime = 0.3f;
    private float jumpTimeCounter;

    private bool isJumping = false;
    private bool isOnAir = false;
    private Rigidbody2D rb;

    // Event to handle everything when the player dies
    public delegate void PlayerDeadHandler();
    public static event PlayerDeadHandler OnPlayerDead;
    private bool isAlive;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        jumpTimeCounter = jumpTime;
        isAlive = true;
    }

    // Update is called once per frame
    
    private void Die()
    {
        isAlive = false;
        Destroy(gameObject);
        OnPlayerDead?.Invoke();
    }

    public bool isPlayerAlive()
    {
        return isAlive;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isOnAir)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isJumping = true;
            isOnAir = true;
            jumpTimeCounter = jumpTime;
        }

        if (Input.GetKey(KeyCode.Space) && isJumping)
        {
            if (jumpTimeCounter > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                jumpTimeCounter -= Time.deltaTime;
            }
           
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            isJumping = false;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isOnAir = false;
        }

        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Die();
        }

    }


}

