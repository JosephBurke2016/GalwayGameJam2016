using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class Note : MonoBehaviour {

	public int NoteNumber = 0;

	private static string[] NoteText = new string[] {
		"Note 0 Content!",
		"Note 1 Content!"
	};

	public GameObject TextCanvas;


	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Collect () {
		UnityEngine.UI.Text UIText = TextCanvas.GetComponentInChildren<Text>();
		UIText.text = NoteText[NoteNumber];

		TextCanvas.SetActive(true);
		gameObject.SetActive(false);
	}
}
