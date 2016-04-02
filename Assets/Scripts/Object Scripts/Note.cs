using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class Note : MonoBehaviour {

	public int NoteNumber = 0;

	private static string[] NoteText = new string[] {
		"Entry 10 \nInterferences with the machine caused this, it's broken but maybe I can use the remains to fix this. I need to undo my mistake. I started destroying my notes to ensure no one will make the same mistake again.",
		"Entry 2 \nI'm trying to create a renewable, perpetual energy source but how am I supposed to do this?! The whole concept is ludicrous, and I'd be probably breaking the laws of nature in the progress. But what else am I supposed to do? I can't just sit and do nothing while everything goes down the drain.",
		"Entry 4 \nI think my idea is not as ridiculous as I think, I think I have found a way. I just need to make sure that it works first. On paper it looks good, but that's not enough. I need to find out if it's the real deal. \nI have collected all my findings in my previous entry, I will take it to my colleagues and make it work...",
		"Entry 6 \nTests with the machine have been satisfactory, but I fear there might be unexpected issues.\nI have caught glimpses through the generator. There are other worlds than these, similar but different.",
		"Entry 7 \nOppenheimer said “I am become Death, the destroyer of words. I shall become Life, and save them all.",
		"Entry 5 \nI still can't believe this. Those idiots at the congress laughed at me! When I close my eyes I see their smug faces, calling me crazy. \nI still hear their laughs. What are those idiots doing? Nothing! I'm trying to save the world!\nI will prove them wrong, I will show them. And then we'll see who is laughing.",
		"Entry 8 \nTesting has been satisfactory. I will be starting the machine tomorrow.",
		"Entry 1 \nThings are not going well at the moment.\nYou see it everywhere in the news. War. Disasters. Our resources slowly being depleted. \nI need to do something about it, it's my duty. I need to make of this world a better place.\nIf I have the chance to try to make things better, then I must do that.",
		"Entry 9 \nWhen I started the machine, something strange happened. I saw a girl, she looked like she was made of electricity. \nShe did something to the machine, I don't know what. But it has smashed the barrier between the worlds and it has done something to Ellie too. \nWhat have I done?"
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
