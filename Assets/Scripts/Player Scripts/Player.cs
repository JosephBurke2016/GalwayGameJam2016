using UnityEngine;
using System.Collections;
using System;

public class Player : MonoBehaviour
{

    private int moveSpeed = 10;

    private PlayerState currentForm;

    enum PlayerState
    {
        Normal,
        Electric,
        Ghost
    };

    private void changeForm()
    {
        if (currentForm == PlayerState.Normal)
        {
            currentForm = PlayerState.Electric;
        }
        else
        {
            currentForm = PlayerState.Normal;
        }
    }

    // Use this for initialization
    void Start()
    {
        currentForm = PlayerState.Normal;
    }

    // Update is called once per frame
    void Update()
    {

        if (currentForm == PlayerState.Normal)
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
        if (coll.gameObject.tag == "GhostWall")
        {
            if (currentForm == PlayerState.Ghost)
            {
                coll.collider.enabled = false;
            }
        }
    }

    void OnCollisionExit2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "GhostWall")
        {
            coll.collider.enabled = true;
        }
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

        if ((Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.UpArrow)) && isGrounded())
        {
            //jump
            jump(0.0f, 3.0f);
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

    }


    public Vector2 jumpVector;
    private void jump(float x, float y)
    {
        GetComponent<Rigidbody2D>().AddForce(jumpVector, ForceMode2D.Impulse);
        jumpVector.x = x;
        jumpVector.y = y;
    }

    private void move(float x, float y, float z)
    {
        transform.position += new Vector3(x * Time.deltaTime, y * Time.deltaTime, z * Time.deltaTime);
    }

    private void move(float x, float y)
    {
        transform.position += new Vector3(x * Time.deltaTime, y * Time.deltaTime, 0.0f);
    }


    //Unity variables. Must be public, class scope, must not be set.
    public Transform grounded;
    public float radius;
    public LayerMask walkable;
    private bool isGrounded()
    {
        return Physics2D.OverlapCircle(grounded.transform.position, radius, walkable);
    }

}
