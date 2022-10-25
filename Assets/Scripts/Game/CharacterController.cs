using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public float MoveSpeed;
    public float JumpForce;

    public KeyCode left;
    public KeyCode right;
    public KeyCode jump;
    //public KeyCode croutch;

    public Rigidbody2D rb;
    //public LayerMask WhatIsGround;
    //public bool isGround;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //isGround = Physics2D.OverlapCapsule()
        Movement();
    }

    void Movement()
    {
        if (Input.GetKey(left))
        {
            rb.velocity = new Vector2(-MoveSpeed, rb.velocity.y);
        } // Move Left
        else if (Input.GetKey(right))
        {
            rb.velocity = new Vector2(MoveSpeed, rb.velocity.y);
        } // Move Right
        else if (!Input.GetKey(left) && !Input.GetKey(right))
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        } // Stop moving
        if (Input.GetKeyDown(jump))
        {
            rb.velocity = new Vector2(rb.velocity.x, JumpForce);
        } // Jump
    }
}
