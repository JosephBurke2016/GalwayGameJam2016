using UnityEngine;
using System.Collections;
using System;

public class Player : MonoBehaviour
{
    public GameObject TextCanvas;

    private int moveSpeed = 30;

    private Vector2 jumpVector;
    private Animator anim;

    private float safeX;
    private float safeY;
    private PlayerState currentForm;
    private bool inAir = false;
    private int noteBlock;
    private bool falling = false;
    private bool wasJumping = false;
    private bool startPointSet = false;
    private Vector2 startPoint;


    enum PlayerState
    {
        Normal,
        Electric,
        Ghost,
        Note
    };

    private void changeForm(PlayerState targetForm)
    {
        if (targetForm == PlayerState.Ghost) {
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

        //save player start position
        startPoint.x = transform.position.x;
        startPoint.y = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        HandleNote();

        if (currentForm == PlayerState.Normal || currentForm == PlayerState.Ghost)
        {
            updatePlayer();
        }
        else if (currentForm == PlayerState.Electric)
        {
            updateEnergy();
        }
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "GhostWall") {
          
            if (currentForm == PlayerState.Ghost) {
                StartCoroutine(DeactivateGhostBlock(coll.collider));          
            } else {
                setVelocity(0.0f, -15);
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
            currentForm = PlayerState.Note;
            noteBlock = 0;
        }
    }

    private void resetWorld()
    {
        safeX = startPoint.x;
        safeY = startPoint.y;
        loadSafePoint();
    }

    private void resetNote(String noteName)
    {
        GameObject go;
        go = GameObject.Find(noteName);
        go.SetActive(true);
    }

    private void resetAllNotes()
    {
        int numberOfNotes = 8;
        for(int i = 0; i < numberOfNotes; i++)
        {
            resetNote("Note" + i);
        }
    }


    private void HandleNote() {
        if (currentForm == PlayerState.Note) {
            setVelocity(0.0f, 0.0f);
            GetComponent<Rigidbody2D>().drag = 100;
            if (noteBlock < 50) {
                noteBlock++;
                return;
            }


            if (Input.anyKey) {
                currentForm = PlayerState.Normal;
                TextCanvas.SetActive(false);
                GetComponent<Rigidbody2D>().drag = 0;
            }
            else
                return;
        }
    }

    private void updatePlayer()
    {
        checkPlayerMovement();
        updateCheckpoint();

        if (currentForm == PlayerState.Ghost) 
            anim.SetInteger("State", 4); 
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

        checkLanding();
        
        if (ghostCheck())
        {
            return;
        }

        print(GetComponent<Rigidbody2D>().velocity);

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            //move down
            move(0, -moveSpeed);
        }
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            //move left
            transform.localRotation = Quaternion.Euler(0, 180, 0);
            if (!inAir)
            {
                anim.SetInteger("State", 2);
            }
            move(-moveSpeed, 0);

        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            //move right
            transform.localRotation = Quaternion.Euler(0, 0, 0);
            if (!inAir)
            {
                anim.SetInteger("State", 1);
            }
            move(moveSpeed, 0);
        }
        else
        {
            if (isGrounded())
            {
                Idle();
                inAir = false;
            }
        }

        if ((Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) && isGrounded())
        {
            //jump
            inAir = false;
            //disabled air animation for now
            //inAir = true;
            anim.SetInteger("State", 3);
            jump(0.0f, 15.0f);
        }

        if (Input.GetKey(KeyCode.R)){
            resetWorld();
        }

    }

    private void checkLanding()
    {
        if (isGrounded()&&wasJumping)
        {
            //jump(0.0f, 0.0f);
            GetComponent<Rigidbody2D>().drag = 100;
            wasJumping = false;
        }
        else if(GetComponent<Rigidbody2D>().drag > 0)
        {
            GetComponent<Rigidbody2D>().drag -= 20;
        }
    }

    private bool ghostCheck()
    {
        if (Input.GetKey(KeyCode.Return))
        {
            changeForm(PlayerState.Ghost);
            return true;
        }
        else if (Input.GetKeyUp(KeyCode.Return))
        {
            changeForm(PlayerState.Normal);
            return false;
        }
        return false;
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
        if (isGrounded() && currentForm == PlayerState.Normal)
        {
            resetPlayerVelocity();
            setSafePoint();
        }

        if(transform.position.y < -3 && (!isGrounded()))
        {
            falling = true;
            anim.SetInteger("State", 5);
        }

        if (transform.position.y < -25)
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
        falling = false;
        inAir = false;
        anim.SetInteger("State", 0);
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
        GetComponent<Rigidbody2D>().drag = 0;
        GetComponent<Rigidbody2D>().AddForce(jumpVector, ForceMode2D.Impulse);
        jumpVector.x = x;
        jumpVector.y = y;
        wasJumping = true;
    }

    private void move(float x, float y, float z)
    {
        //transform.position += new Vector3(x * Time.deltaTime, y * Time.deltaTime, z * Time.deltaTime);
    }

    private void move(float x, float y)
    {
     
        if(GetComponent<Rigidbody2D>().velocity.x < 50 && GetComponent<Rigidbody2D>().velocity.x > -50)
        {
            GetComponent<Rigidbody2D>().velocity += new Vector2(x * Time.deltaTime, y * Time.deltaTime);
        }

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
