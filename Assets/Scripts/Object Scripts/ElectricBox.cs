using UnityEngine;
using System.Collections;

public class ElectricBox : MonoBehaviour {

    public GameObject player;
    public Animator anim;
    public bool playerDetected = false;

    // Use this for initialization
    void Start()
    {
        anim = player.GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            playerDetected = true;
            anim = player.GetComponent<Animator>();
            Debug.Log("hit");

        }
    }

}
