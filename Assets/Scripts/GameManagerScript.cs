using UnityEngine;
using System.Collections;

public class GameManagerScript : MonoBehaviour {
	
	public GameObject[] hand;
	public GameObject[] cards;
	public GameObject   card;

	public Material[]   cardFaces;
	public Material     cardBack;
	
	public ArrayList    deck;
	public ArrayList    newhand;
	public ArrayList    graveyard;
	public ArrayList    shuffled;
	
	public CardScript   selectedHand;
	public GUIText      scoreText;
	public GUIText		multiplierText;
	public GUIText      gameOverText;
	public GUIText      finalScoreText;
	
	public int score;
	
	public int handSize;
	public float multiplier;
	
	// Use this for initialization
	void Start () {
		hand = new GameObject[5];
		deck = new ArrayList();
		graveyard = new ArrayList();
		newhand = new ArrayList();
		shuffled = new ArrayList();
		buildDeck ();
		buildHand ();
		buildGraveyard ();
		score = 0;
		multiplier = 0.0f;
	}
	
	void buildDeck() {
		GameObject acard;
		int faceVal = 0;
		int cardVal = 0;
		string face = "";
		
		for (int i = 0; i < 52; i++) {
			faceVal = i / 13;
			cardVal = i % 13;
			if (faceVal == 0) {
				face = "c";
			} else if (faceVal == 1) {
				face = "d";
			} else if (faceVal == 2) {
				face = "h";
			} else if (faceVal == 3) {
				face = "s";
			}

			acard = buildCard ();
			CardScript cs = _getScriptForCard(acard);
			cs.Load(faceVal, cardVal, cardFaces[i]);
			deck.Add(acard);
		}
	
		ArrayList deckCopy = (ArrayList)deck.Clone();
		while (deckCopy.Count > 0) {
			int i = Random.Range(0, deckCopy.Count);
			shuffled.Add(deckCopy[i]);
			deckCopy.RemoveAt (i);
		}
		
		_verifyShuffle();
	}
	
	void buildGraveyard() {
		GameObject gCard = _draw ();
		gCard.transform.Translate(new Vector3(-2.0f, 0.0f, 0.0f));
		CardScript cs = _getScriptForCard(gCard);
		cs.FaceUp();
		graveyard.Add (gCard);
	}
	
	public GameObject buildCard() {
		return Instantiate(card, new Vector3(6.0f, 0.0f, -2.0f), Quaternion.identity) as GameObject;
	}
	
	public void putBackInDeck(GameObject aCard) {
		CardScript cs = _getScriptForCard(aCard);
		cs.FaceDown();
		cs.selectable = false;
		aCard.transform.position = new Vector3(6.0f, 0.0f, -2.0f);
		shuffled.Add(aCard);	
	}
	
	public void registerClicked(GameObject aCard) {
		CardScript cs = _getScriptForCard(aCard);
		if (newhand.Contains(aCard)) {
			// Card is currently in the player's hand
			selectedHand = cs;
			
			GameObject gCard = (GameObject)graveyard[graveyard.Count-1];
			CardScript gCardCs = _getScriptForCard(gCard);
			
			// Match Suit and Increment or Decrement = 300 points, Match Value = 200 points, Match Face = 100 points
			if (cs.cardFace == gCardCs.cardFace && (cs.cardValue == gCardCs.cardValue - 1 || cs.cardValue == gCardCs.cardValue + 1)) {
				multiplier += 0.3f;
				score += Mathf.RoundToInt(300.0f * multiplier);
			} else if (cs.cardValue == gCardCs.cardValue) {
				multiplier += 0.2f;
				score += Mathf.RoundToInt(200.0f * multiplier);
			} else if (cs.cardFace == gCardCs.cardFace) {
				multiplier += 0.1f;
				score += Mathf.RoundToInt(100.0f * multiplier);
			} else {
				return;
			}
			updateText ();
			aCard.transform.position = gCard.transform.position;
			cs.selectable = false;
			newhand.Remove(aCard);
			foreach (GameObject nCard in graveyard) {
				nCard.SetActive(false);	
			}
			graveyard.Add (aCard);
			buildHand();
		}
	}
	
	public void updateText() {
		multiplierText.text = "Multiplier: " + multiplier.ToString() + "X";
		scoreText.text = "Score: " + score.ToString();
	}
	
	public void buildHand() {
		if (shuffled.Count == 0) { return; }
		for (int i = 0; i < handSize; i++) {
			GameObject acard;
			if (newhand.Count > i) {
				acard = (GameObject)newhand[i];
			} else {
				acard = _draw ();
				newhand.Add(acard);
			}
			acard.transform.position = new Vector3((i * 2.0f) - 6.0f, 0.1f, -6.0f);
			CardScript cs = _getScriptForCard(acard);
			cs.FaceUp();
			cs.selectable = true;
		}
	}
	
	private void _verifyShuffle() {
		foreach (GameObject card in shuffled) {
			CardScript cs = _getScriptForCard(card);
		}
	}
	
	private GameObject _draw() {
		if (shuffled.Count > 0) {
			GameObject acard = (GameObject)shuffled[0];
			shuffled.RemoveAt (0);
			acard.SetActive(true);
			return acard;
		} else {
			return null;
		}
	}
	
	private CardScript _getScriptForCard (GameObject aCard) {
		return getScriptForCard(aCard);	
	}
	
	public CardScript getScriptForCard(GameObject acard) {
		return (CardScript)acard.GetComponent(typeof(CardScript));
	}
	
	public void OnGUI () {
		if (GUI.Button(new Rect(10, 70, 100, 60), "Draw")) {
			score -= 200;
			multiplier = 0.0f;
			foreach (GameObject aCard in newhand) {
				putBackInDeck(aCard);
			}
			newhand.Clear();
			buildHand ();
			updateText();
		}
		
		if (GUI.Button(new Rect(Screen.width - 120, 70, 100, 60), "End Game")) {
			score -= (newhand.Count * 500);
			multiplier = 0.0f;
			updateText();
			finalScoreText.text = "Final Score: " + score.ToString();
			gameOverText.gameObject.SetActive(true);
			finalScoreText.gameObject.SetActive(true);
		}
	}
}