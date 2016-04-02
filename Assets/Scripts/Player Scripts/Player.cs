using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    //private Vector2 move;
    private int moveSpeed = 10;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        checkMovement();
    }

    private void checkMovement()
    {
        if (Input.GetKey(KeyCode.D))
        {
            moveRight();
        }
    }



    private void moveRight()
    {
        transform.position += new Vector3(moveSpeed * Time.deltaTime, 0.0f, 0.0f);
    }

}
