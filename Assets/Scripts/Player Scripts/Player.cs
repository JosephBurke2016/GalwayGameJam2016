using UnityEngine;
using System.Collections;
using System;

public class Player : MonoBehaviour
{

    private int moveSpeed = 20;

    private Vector2 jumpVector;
    private Animator anim;
    private bool collideWithGhostAble = false;

    private float safeX;
    private float safeY;
    private PlayerState currentForm;

    enum PlayerState
    {
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
        anim = GetComponent<Animator>();
        collideWithGhostAble = false;
    }

    // Update is called once per frame
    void Update()
    {

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

    void OnTriggerEnter2D(Collider2D coll) {
        if (coll.gameObject.tag == "Note") {
            Note note = coll.gameObject.GetComponent<Note>();
            note.Collect();
        }
    }

    private void updatePlayer()
    {
        checkPlayerMovement();
        updateCheckpoint();
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
        if (isGrounded())
        {
            resetVelocity();
        }

        if ((Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) && isGrounded())
        {
            //jump
            anim.SetInteger("State", 3);
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
            transform.localRotation = Quaternion.Euler(0, 180, 0);
            anim.SetInteger("State", 2);
            move(-moveSpeed, 0);


        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            //move right
            transform.localRotation = Quaternion.Euler(0, 0, 0);
            anim.SetInteger("State", 1);
            move(moveSpeed, 0);
        }
        else
            Idle();

        if (Input.GetKey(KeyCode.Return)) {
            anim.SetInteger("State", 4);
            changeForm(PlayerState.Ghost);
        }

    }

    private void resetVelocity()
    {
        float xVel = GetComponent<Rigidbody2D>().velocity.x;
        float yVel = GetComponent<Rigidbody2D>().velocity.y;

        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow))
        {
            setVelocity(0.0f, yVel);
        }
        if(Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.DownArrow))
        {
            setVelocity(xVel, 0.0f);
        }
    }
    
    private void updateCheckpoint()
    {
        if (isGrounded())
        {
            resetPlayerVelocity();
            setSafePoint();
        }

        if (transform.position.y < -100)
        {
            loadSafePoint();
        }
    }

    private void setSafePoint()
    {
        safeX = transform.position.x;
        safeY = transform.position.y;
    }

    private void loadSafePoint()
    {
        setVelocity(0.0f, 0.0f);
        transform.position = new Vector3(safeX, safeY, 0.0f);
    }

    private void resetPlayerVelocity()
    {
        float xVel = GetComponent<Rigidbody2D>().velocity.x;
        float yVel = GetComponent<Rigidbody2D>().velocity.y;

        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow))
        {
            setVelocity(0.0f, yVel);
        }
        if(Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.DownArrow))
        {
            setVelocity(xVel, 0.0f);
        }
    }

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
        GetComponent<Rigidbody2D>().velocity += new Vector2(x * Time.deltaTime, y * Time.deltaTime);
    }

    private void setVelocity(float x, float y)
    {
        GetComponent<Rigidbody2D>().velocity = new Vector2(x, y);
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

    //Unity variables. Must be public, class scope, must not be set.
    public Transform grounded;
    public LayerMask walkable;
    private bool isGrounded()
    {
        float radius = 0.2f;
        return Physics2D.OverlapCircle(grounded.transform.position, radius, walkable);
    }

    private void Idle()
    {
        anim.SetInteger("State", 0);
    }

}
