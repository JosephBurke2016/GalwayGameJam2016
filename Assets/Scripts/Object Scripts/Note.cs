using UnityEngine;
using System.Collections;

public class Note : MonoBehaviour {

	public int NoteNumber = 0;

	private string[] NoteText;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Collect () {
		Debug.Log("Collect Note");
	}
}
