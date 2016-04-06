using UnityEngine;
using System.Collections;

public class EndGame : MonoBehaviour {

    public GameObject explosionSprite;
    public Renderer rend;
    public bool isScaling = false;
    public float value = 5.0f;
    private Vector3 zero;

    // Use this for initialization
    void Start () {
        rend = GetComponent<Renderer>();
        rend.enabled = false;
        zero = transform.localScale;
    }
	
	// Update is called once per frame
	void Update () {
      

        if (isScaling == true) {
            Vector3 temp = transform.localScale;
            temp.x = value++;
            temp.y = value++;

            transform.localScale = temp;

        }else
        {
            transform.localScale = zero;
            rend.enabled = false;
            value = 5.0f;
        }
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            rend.enabled = true;
            isScaling = true;
   
        }
    }
}
