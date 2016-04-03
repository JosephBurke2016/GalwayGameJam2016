using UnityEngine;
using System.Collections;

public class DropPlatform : MonoBehaviour {

    public GameObject player;
    public bool onPlatform = false;
    private Animator anim;

    // Use this for initialization
    void Start () {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        if (onPlatform == true)
        {
            GetComponent<Rigidbody2D>().isKinematic = false;
            anim.SetInteger("ColourChange", 1);

        }
    }

        void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            onPlatform = true;
        }
    }
}
