using UnityEngine;
using System.Collections;
using System;

public class Player : MonoBehaviour {

    //private Vector2 move;
    private int moveSpeed = 10;

    private PlayerState currentForm; 

    enum PlayerState {
        Normal,
        Electric
    };

    private void changeForm()
    {
        if (currentForm == PlayerState.Normal)
        {
            currentForm = PlayerState.Electric;
        }else
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
        else if (currentForm == PlayerState.Normal)
        {
            updateEnergy();
        }
        else
        {
            throw new EntryPointNotFoundException();
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
        if (currentForm == PlayerState.Normal) {
            if (Input.GetKey(KeyCode.D))
            {
                moveRight();
            }
        }
    }



    private void moveRight()
    {
        transform.position += new Vector3(moveSpeed * Time.deltaTime, 0.0f, 0.0f);
    }

}
