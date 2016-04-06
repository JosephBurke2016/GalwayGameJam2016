using UnityEngine;
using System.Collections;

public class CableMechanic : MonoBehaviour {

    public enum moveDir
    {
        UP,
        DOWN,
        LEFT,
        RIGHT
    }

    public moveDir cabledir;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public float getCableMiddle () {
		Vector3 center = GetComponent<BoxCollider2D>().bounds.center;

        if (cabledir == moveDir.UP || cabledir == moveDir.DOWN)
        {
            return center.x;
        }

        else if ((cabledir == moveDir.RIGHT || cabledir == moveDir.LEFT))
        {
            return center.y;
        }
        else throw new UnassignedReferenceException();
	}

    

}
