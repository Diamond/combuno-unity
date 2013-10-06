using UnityEngine;
using System.Collections;

public class CardScript : MonoBehaviour {
	public int  	cardFace;
	public int  	cardValue;
	public bool 	faceUp;
	public bool 	selectable;
	public bool 	selected;
	public Material cardBack;
	public Material cardFront;
	public Color    startColor;
	public bool     inGraveyard;
	public bool     inDeck;
	
	public GameObject gm;
	public GameManagerScript gs;
	
	public void Load(int aFace, int aVal, Material mat) {
		cardFace  = aFace;
		cardValue = aVal;
		cardFront = mat;
		gameObject.renderer.material = cardBack;
		gm = GameObject.FindGameObjectWithTag("GameManager");
		faceUp      = false;
		selectable  = false;
		selected    = false;
		inGraveyard = false;
		inDeck      = false;
	}

	// Use this for initialization
	void Start () {
	}
	
	public void FaceUp () {
		faceUp = true;
		_changeFace();
	}
	
	public void FaceDown () {
		faceUp = false;
		_changeFace();
	}
	
	public void Flip() {
		faceUp = !faceUp;
		_changeFace();
	}
	
	private void _changeFace() {
		gameObject.renderer.material = faceUp ? cardFront : cardBack;
		startColor = gameObject.renderer.material.color;
	}
	
	public string ToString() {
		string output = "";
		if (cardValue >= 1 && cardValue <= 10) {
			output += cardValue.ToString ();	
		} else if (cardValue == 0) {
			output += "Ace"; 	
		} else if (cardValue == 11) {
			output += "Jack";	
		} else if (cardValue == 12) {
			output += "Queen";	
		} else if (cardValue == 13) {
			output += "King";	
		}
		output += " of ";
		if (cardFace == 0) {
			output += "Clubs";	
		} else if (cardFace == 1) {
			output += "Diamonds";
		} else if (cardFace == 2) {
			output += "Hearts";	
		} else if (cardFace == 3) {
			output += "Spades";	
		}
		return output;
	}
	
	void OnMouseEnter() {
		if (selectable) {
			gameObject.transform.Translate(new Vector3(0.0f, 0.5f, 0.0f));
		}
	}
	
	void OnMouseExit() {
		if (selectable) {
			gameObject.transform.Translate(new Vector3(0.0f, -0.5f, 0.0f));
		}
	}
	
	void OnMouseDown() {
		if (selectable) {
			GameManagerScript gs = (GameManagerScript)gm.GetComponent(typeof(GameManagerScript));
			gs.registerClicked(gameObject);
		}
	}
	
}
