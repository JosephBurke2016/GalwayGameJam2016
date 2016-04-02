﻿using UnityEngine;
using System.Collections;
using System;

public class Player : MonoBehaviour {

    private int moveSpeed = 10;
    public Transform grounded;
    public float radius;
    public LayerMask walkable;
    public bool isGrounded = false;
    public bool collideWithGhostAble = false;
    public Vector2 jumpVector;

    private PlayerState currentForm; 

    enum PlayerState {
        Normal,
        Electric,
        Ghost
    };

    private void changeForm(PlayerState targetForm)
    {
        if (targetForm == PlayerState.Ghost && collideWithGhostAble) {
            currentForm = PlayerState.Ghost;
        }

        if (targetForm == PlayerState.Normal) {
            currentForm = PlayerState.Normal;
        }
       
    }

    // Use this for initialization
    void Start()
    {
        currentForm = PlayerState.Normal;
        collideWithGhostAble = false;
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(grounded.transform.position, radius, walkable);

        if (currentForm == PlayerState.Normal || currentForm == PlayerState.Ghost)
        {
            updatePlayer();
        }
        else if (currentForm == PlayerState.Electric)
        {
            updateEnergy();
        }
        else
        {
            throw new EntryPointNotFoundException();
        }

    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "GhostWall") {
          
            StartCoroutine(EnableAllowGhosting()); 
            if (currentForm == PlayerState.Ghost) {
                StartCoroutine(DeactivateGhostBlock(coll.collider));          
            }
        }
    }

    void OnCollisionExit2D(Collision2D coll)
    {

    }

    private void updatePlayer()
    {
        checkPlayerMovement();
    }

    private void updateEnergy()
    {
        checkEnergyMovement();
    }



    private void checkEnergyMovement()
    {
        throw new NotImplementedException();
    }

    private void checkPlayerMovement()
    {

        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.UpArrow) && isGrounded == true)
            {
            //jump
            GetComponent<Rigidbody2D>().AddForce(jumpVector, ForceMode2D.Impulse);
            jumpVector.y = 0.5f;
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            //move down
            move(0, -moveSpeed);
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            //move left
            move(-moveSpeed, 0);
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
            //move right
            move(moveSpeed, 0);
        }

        if (Input.GetKey(KeyCode.Return)) {
            changeForm(PlayerState.Ghost);
        }

    }

    private void move(float x, float y, float z)
    {
        transform.position += new Vector3(x * Time.deltaTime, y * Time.deltaTime, z * Time.deltaTime);
    }

    private void move(float x, float y)
    {
        transform.position += new Vector3(x * Time.deltaTime, y * Time.deltaTime, 0.0f);
    }

    IEnumerator DeactivateGhostBlock(Collider2D collider)
    {
      //  print(Time.time);
        collider.enabled = false; 
        yield return new WaitForSeconds(0.2f);
        collider.enabled = true; 
        print(collider.enabled);
        changeForm(PlayerState.Normal);
    }

    IEnumerator EnableAllowGhosting()
    {
      //  print(Time.time);
        collideWithGhostAble = true; 
        yield return new WaitForSeconds(0.25f);
        collideWithGhostAble = false; 
    }

}
