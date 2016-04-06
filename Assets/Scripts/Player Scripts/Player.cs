using UnityEngine;
using System.Collections;
using System;

public class Player : MonoBehaviour
{
    public GameObject TextCanvas;

    private AudioSource source;
    public AudioClip jumpSound;
    public AudioClip walkingSound;
    public AudioClip electricSound;
    public AudioClip noteSound;


    private int moveSpeed = 30;

    private Vector2 jumpVector;
    private Animator anim;

    private float safeX;
    private float safeY;
    private PlayerState currentForm;
    private bool inAir = false;
    private bool wasJumping = false;
    private bool bounce = false;
    private CableMechanic.moveDir currentEnergyDirection;
    public int energySpeed;
    private bool pIsPressed = true;
    private Vector2 startPoint;

    
    public enum PlayerState
    {
        Normal,
        Electric,
        Ghost,
        Note
    }

    public void changeForm(PlayerState targetForm)
    {
        currentForm = targetForm;  
    }

    // Use this for initialization
    void Start()
    {
        currentForm = PlayerState.Normal;
        anim = GetComponent<Animator>();
        source = GetComponent<AudioSource>();     


        //save player start position
        startPoint.x = transform.position.x;
        startPoint.y = transform.position.y;
    }

    // Update is called once per frame
    public float keyDelay = 0.05f;
    private float timePassed = 0.0f;
    void Update()
    {
        timePassed += Time.deltaTime;

        HandleNote();

        if (currentForm == PlayerState.Normal || currentForm == PlayerState.Ghost)
        {
            updatePlayer();
        }
        else if (currentForm == PlayerState.Electric)
        {
            updateEnergy();
        }

        EndGame[] endNodes = FindObjectsOfType<EndGame>();
        foreach (EndGame terminal in endNodes)
        {
            if (terminal.value > 300)
            {
                resetWorld();
            }
        }
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "GhostWall") {
            if (currentForm == PlayerState.Ghost) {
                StartCoroutine(DeactivateGhostBlock(coll.collider));
                print("pass through");       
            } else {
                //set -x velocity as well to pervent stuck in ghostwall bug.
                setVelocity(0.0f, 0.0f);
            }
        }

        
        if (coll.gameObject.tag == "Bouncy")
        {
            print("bounce");
            bounce = true;
        }else if(coll.gameObject.tag != "Bouncy")
        {
            print("no bounce");
            bounce = false;
        }
        
    }

    void OnCollisionExit2D(Collision2D coll)
    {

    }

    void OnTriggerEnter2D(Collider2D coll) {
        switch (coll.gameObject.tag) {
            case "Note":
                CollectNote(coll);
                break;
            case "EntryBox":
                EntryCables(coll);
                break;
            case "RedPipe":
                MoveAlongCable(coll);
                break;
            case "ExitBox":
                ExitCables(coll);
                break;
            default:
                print(coll.gameObject.tag);
                break; 
        }
    }


    private void EntryCables (Collider2D coll) {
        changeForm(PlayerState.Electric);
        GetComponent<Rigidbody2D>().gravityScale = 0;
        setVelocity(0.0f, 0.0f);

        Vector3 lowerLeft = coll.gameObject.GetComponent<BoxCollider2D>().bounds.min;
        
        transform.position = new Vector3(transform.position.x, lowerLeft.y, 0.0f);

        currentEnergyDirection = CableMechanic.moveDir.RIGHT;
    }

    private void MoveAlongCable (Collider2D coll) {
        if (currentForm != PlayerState.Electric)
            return;

        CableMechanic cable = coll.gameObject.GetComponent<CableMechanic>();
        currentEnergyDirection = cable.cabledir;
        print(currentEnergyDirection);

        float center = cable.getCableMiddle();
        switch (cable.cabledir)
        {
            case CableMechanic.moveDir.UP:
            case CableMechanic.moveDir.DOWN:
                move(center, transform.position.y, 0.0f);
                break;
            case CableMechanic.moveDir.LEFT:
            case CableMechanic.moveDir.RIGHT:
                move(transform.position.x, center, 0.0f);
                break;
            default:
                throw new System.ArgumentException();
                //break;
        }
    }

    private void ExitCables (Collider2D coll) {

        if (currentForm != PlayerState.Electric)
            return;

        changeForm(PlayerState.Normal);
        GetComponent<Rigidbody2D>().gravityScale = 1;
        Vector3 center = coll.gameObject.GetComponent<BoxCollider2D>().bounds.center;
        transform.position = center;
        source.Stop();
    }

    private void CollectNote (Collider2D coll) {

        Note note = coll.gameObject.GetComponent<Note>();
        if (note.visible)
        {
            note.Collect(this);
            source.PlayOneShot(noteSound, 1.0f);
            //there should always be valid safe ground underneath a note
            setSafePoint();
        }
    }

    private void resetWorld()
    {
        safeX = startPoint.x;
        safeY = startPoint.y+1;
        loadSafePoint();
        resetAllNotes();

        EndGame[] endNodes = FindObjectsOfType<EndGame>();
        foreach (EndGame terminal in endNodes)
        {
            terminal.isScaling = false;
        }
    }
    
    private void resetAllNotes()
    {
        Note[] notes = FindObjectsOfType<Note>();
        foreach (Note note in notes)
        {
            print("note "+note.ToString());
            note.reset();
        }
    }


    int noteBlock = 0;
    private void HandleNote() {
        if (currentForm == PlayerState.Note) {
            setVelocity(0.0f, 0.0f);
            GetComponent<Rigidbody2D>().drag = 100;
            if (noteBlock < 40) {
                noteBlock++;
                return;
            }

            if (Input.anyKey) {
                currentForm = PlayerState.Normal;
                changeForm(PlayerState.Normal);
                TextCanvas.SetActive(false);
                GetComponent<Rigidbody2D>().drag = 0;
                noteBlock = 0;
            }
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
        anim.SetInteger("State", 4); 
        checkEnergyMovement();

        if (!source.isPlaying)
            source.PlayOneShot(electricSound, 1.0f);

    }

    private void checkEnergyMovement()
    {
        Vector2 movement = new Vector2(0,0);
        switch (currentEnergyDirection) {
            case CableMechanic.moveDir.LEFT:
                movement = Vector2.left;
                break;
            case CableMechanic.moveDir.RIGHT:
                movement = Vector2.right;
                break;
            case CableMechanic.moveDir.UP:
                movement = Vector2.up;
                break;
            case CableMechanic.moveDir.DOWN:
                movement = Vector2.down;
                break;
        }
        movement *= Time.deltaTime;
        movement *= energySpeed;
        transform.position += new Vector3(movement.x, movement.y, 0.0f);
    }


    
    private void checkPlayerMovement()
    {

        if (isCollidingWithWalkableGameObject())
        {
            resetVelocity();
        }
        checkLanding();

        if (ghostCheck())
        {
            return;
        }

        //print(GetComponent<Rigidbody2D>().velocity);

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
            move(-moveSpeed, -moveSpeed / 10);

             if (isCollidingWithWalkableGameObject() && !source.isPlaying) {
                source.PlayOneShot(walkingSound, 1.0f);
            }

        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            //move right
            transform.localRotation = Quaternion.Euler(0, 0, 0);
            if (!inAir)
            {
                anim.SetInteger("State", 1);
            }
            move(moveSpeed, -moveSpeed / 10);

            if (isCollidingWithWalkableGameObject() && !source.isPlaying) {
                source.PlayOneShot(walkingSound, 1.0f);
            }
        }
        else
        {
            if (isCollidingWithWalkableGameObject())
            {
                Idle();
                //inAir = false;
            }
        }
        
        if ((Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) && isCollidingWithWalkableGameObject())
        {
            //jump
            jump(0.0f, 15.0f);
        }

        if (Input.GetKey(KeyCode.R)){
            resetWorld();
        }

        if (Input.GetKey(KeyCode.P)||pIsPressed)
        {
           if(!inAir&&isCollidingWithWalkableGameObject()){
                setSafePoint();
                pIsPressed = false;
            }
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    private void checkLanding()
    {
        if (isCollidingWithWalkableGameObject()&&wasJumping)
        {
            //jump(0.0f, 0.0f);
            GetComponent<Rigidbody2D>().drag = 100;
            wasJumping = false;
            inAir = false;
            setVelocity(0f, 0f);
        }
        else if(GetComponent<Rigidbody2D>().drag > 0)
        {
            GetComponent<Rigidbody2D>().drag -= 50;
        }
    }

    private bool ghostCheck()
    {
        if (Input.GetKey(KeyCode.Return))
        {
            changeForm(PlayerState.Ghost);
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                jump(-10, 20);
            }
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                jump(10, 20);
            }
            else
            {
                jump(0, 20);
            }
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
        if (isCollidingWithWalkableGameObject() && currentForm == PlayerState.Normal && (!inAir))
        {
            resetPlayerVelocity();
        }

        if(transform.position.y < -3 && (!isCollidingWithWalkableGameObject()))
        {
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
        inAir = false;
        bounce = false;
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

    private bool ghostCharge = false;
    private void jump(float x, float y)
    {
        print(jumpVector.y + " " + inAir + " " + isCollidingWithWalkableGameObject());
        if (!inAir && timePassed >= keyDelay)
        {
            inAir = true;
            source.PlayOneShot(jumpSound, 1.0f);
            GetComponent<Rigidbody2D>().drag = 0;
            jumpVector.x = x;
            if (bounce)
            {
                jumpVector.y += 25;
                if (jumpVector.y > 50)
                {
                    jumpVector.y = 50;
                }
                anim.SetInteger("State", 5);
            }
            else
            {
                jumpVector.y = y;
            }
            GetComponent<Rigidbody2D>().AddForce(jumpVector, ForceMode2D.Impulse);
            wasJumping = true;
            timePassed = 0f;
            print("inner:" + jumpVector.y + " " + inAir + " " + isCollidingWithWalkableGameObject());
            ghostCharge = true;
        }
        else if (ghostCharge && currentForm == PlayerState.Ghost && timePassed >= keyDelay)
        {
            if (bounce)
            {
                jumpVector.y = 10;
            }else
            {
                jumpVector.y = 0;
            }
            jumpVector.x = x * 2.5f;
            GetComponent<Rigidbody2D>().AddForce(jumpVector, ForceMode2D.Impulse);
            //transform.position = new Vector3(transform.position.x+(x/5), transform.position.y, 0.0f);
            ghostCharge = false;
            timePassed = 0f;
        }
    }
    
    private void move(float x, float y, float z)
    {
        //print("deprecated - move(float x, float y, float z)");
        //transform.position += new Vector3(x * Time.deltaTime, y * Time.deltaTime, z * Time.deltaTime);
        transform.position = new Vector3(x, y, z);
    }

    private void move(float x, float y)
    {

        if (inAir)
        {
            x = x / 2;
            y = y / 2;
        }

        if (GetComponent<Rigidbody2D>().velocity.x < 50 && GetComponent<Rigidbody2D>().velocity.x > -50)
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
    private bool isCollidingWithWalkableGameObject()
    {
        float radius = 0.05f;
        return Physics2D.OverlapCircle(grounded.transform.position, radius, walkable);
    }

    private void Idle()
    {
        anim.SetInteger("State", 0);
    }

}
