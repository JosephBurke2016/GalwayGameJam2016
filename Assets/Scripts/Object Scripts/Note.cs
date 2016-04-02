using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class Note : MonoBehaviour {

	public int NoteNumber = 0;

	private static string[] NoteText = new string[] {
		"Entry 10 \n Interferences with the machine caused this, it's broken but maybe I can use the remains to fix this. I need to undo my mistake. I started destroying my notes to ensure no one will make the same mistake again.",
		"Entry 2 \n I'm trying to create a renewable, perpetual energy source but how am I supposed to do this?! The whole concept is ludicrous, and I'd be probably breaking the laws of nature in the progress. But what else am I supposed to do? I can't just sit and do nothing while everything goes down the drain.",
		"Entry 4 \n I think my idea is not as ridiculous as I think, I think I have found a way. I just need to make sure that it works first. On paper it looks good, but that's not enough. I need to find out if it's the real deal. \n I have collected all my findings in my previous entry, I will take it to my colleagues and make it work",
		"Entry 6 \n Tests with the machine have been satisfactory, but I fear there might be unexpected issues. \n I have caught glimpses through the generator. There are other worlds than these, similar but different.",
		"Entry 7 \n Oppenheimer said “I am become Death, the destroyer of words. I shall become Life, and save them all.",
		"Entry 5 \n I still can't believe this. Those idiots at the congress laughed at me! When I close my eyes I see their smug faces, calling me crazy. \n I still hear their laughs. What are those idiots doing? Nothing! I'm trying to save the world! \n I will prove them wrong, I will show them. And then we'll see who is laughing.",
		"Entry 8 \n Testing has been satisfactory. I will be starting the machine tomorrow.",
		"Entry 1 \n Things are not going well at the moment. \n You see it everywhere in the news. War. Disasters. Our resources slowly being depleted. \n I need to do something about it, it's my duty. I need to make of this world a better place. \n If I have the chance to try to make things better, then I must do that.",
		"Entry 9 \n  When I started the machine, something strange happened. I saw a girl, she looked like she was made of electricity. \n She did something to the machine, I don't know what. But it has smashed the barrier between the worlds and it has done something to Ellie too \n. What have I done?"
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
