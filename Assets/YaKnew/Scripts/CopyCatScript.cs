using UnityEngine;
using UnityEngine.UI;
using VRTK;
using System.Collections;
using System.Collections.Generic;

public class CopyCatScript : MonoBehaviour {

	// Based off of https://www.danpurdy.co.uk/tutorial/simon-says-game-in-jquery-tutorial/

	private int level = 1;
	private int turn = 0;
	private float difficulty = 1;
	private int score = 0;
	private GameState stateOfGame = GameState.LoadingScript;
	private List<int> genSequence;
	private List<int> plaSequence;

	[Header("Buttons", order = 1)]
	public GameObject redButton;
	public GameObject blueButton;
	public GameObject yellowButton;
	public GameObject greenButton;

	[Header("Text Labels", order = 2)]
	public GameObject levelValue;
	public GameObject scoreValue;

	[Header("Controls", order = 3)]
	public GameObject startButton;

	private enum GameState
	{
		LoadingScript,
		ReadyToStart,
		ComputerActive,
		PlayerActive,
		ValidatingPlayerChoice,
		GameLost,
		LevelWon
	}

	private void setScore(int newScore) {		
		score = newScore;
		foreach (TextMesh value in scoreValue.GetComponents<TextMesh>()) {
			value.text = score.ToString ();
		}
	}

	private void setLevel(int newLevel) {
		level = newLevel;
		foreach (TextMesh value in levelValue.GetComponents<TextMesh>()) {
			value.text = level.ToString ();
		}
	}

	private void incrementLevel() {
		setLevel (level + 1);
	}

	private void incrementTurn() {
		turn = turn + 1;
	}

	private void resetTurn() {
//		Debug.Log("resetTurn");
		turn = 0;
	}

	private void resetPlayerSequence() {
//		Debug.Log ("resetPlayerSequence");
		plaSequence = new List<int> ();
	}

	private void resetGeneratedSequence() {
//		Debug.Log ("resetGeneratedSequence");
		genSequence = new List<int> ();
	}

	private void setGameState(GameState newState) {
		stateOfGame = newState;
	}

	private void setActive(bool isActive, GameObject button) {
		if (button != null) {
			VRTK_InteractableObject obj = button.GetComponent<VRTK_InteractableObject> ();
			if (obj != null) {
				obj.highlightOnTouch = isActive;
			}
		}
	}

	private void setActive(bool isActive) {
//		Debug.Log ("setActive = " + isActive.ToString ());
		setActive (isActive, redButton);
		setActive (isActive, greenButton);
		setActive (isActive, blueButton);
		setActive (isActive, yellowButton);
	}

	public void BeginGame() {
		if (stateOfGame == GameState.ReadyToStart || stateOfGame == GameState.GameLost) {
//			Debug.Log ("Start BeginGame");
			startButton.SetActive (false);
			setActive (true);
			setScore (0);
			setLevel (1);
			resetTurn ();
			resetGeneratedSequence ();
			resetPlayerSequence ();
			setGameState (GameState.ReadyToStart);
			Invoke ("newLevel", 1f);
//		} else {
//			Debug.Log ("Unable to Start BeginGame");
		}
	}

 	private void newLevel() {
		if (stateOfGame == GameState.ReadyToStart || stateOfGame == GameState.LevelWon) {
//			Debug.Log ("newLevel");
			setGameState (GameState.ComputerActive);
			resetPlayerSequence ();
			genSequence.Add (newSequenceValue ());
			resetTurn ();
			setActive (true);
			StartCoroutine (displaySequence());
		}
	}

	private IEnumerator displaySequence() {
//		Debug.Log ("displaySequence");
		foreach (int value in genSequence) {
			GameObject currentObject = intToGameObject (value);
			toggleHighlight (currentObject, true);
			yield return new WaitForSeconds (0.3f);
			toggleHighlight (currentObject, false);
			yield return new WaitForSeconds (0.5f * difficulty);
			setActive (true, currentObject);
		}
		setGameState (GameState.PlayerActive);
	}

	private IEnumerator incorrectSequence() {
		GameObject correctPad = intToGameObject (genSequence [turn]);
		yield return new WaitForSeconds (0.3f);
		for (int counter = 0; counter < 4; counter++) {
			toggleHighlight (correctPad, true);
			yield return new WaitForSeconds (0.3f);
			toggleHighlight (correctPad, false);
			yield return new WaitForSeconds (0.3f);
			setActive (true, correctPad);
		}
		yield return new WaitForSeconds (0.5f);
		startButton.SetActive (true);
	}

	public void LogPlayerSequence(GameObject pad) {
		if (stateOfGame == GameState.PlayerActive) {
			setGameState (GameState.ValidatingPlayerChoice);
			Debug.Log ("LogPlayerSequence");
			int value = gameObjectToInt (pad);
			plaSequence.Add (value);
			checkSelectedPad (value);
		}
	}

	private void checkSelectedPad(int pad) {
//		Debug.Log ("genSequence.Count = " + genSequence.Count.ToString ());
//		Debug.Log ("plaSequence.Count = " + plaSequence.Count.ToString ());
//		Debug.Log ("Turn = " + turn.ToString ());
		if (pad != genSequence [turn]) {
//			Debug.Log ("pad = " + pad.ToString ());
//			Debug.Log ("genSquence[turn] = " + genSequence [turn].ToString ());
			setGameState (GameState.GameLost);
			StartCoroutine (incorrectSequence());
		} else {
			addToScore ();
			incrementTurn ();
			if (turn == genSequence.Count) {
				setGameState (GameState.LevelWon);
				incrementLevel ();
				Invoke ("newLevel", 1f);
			} else {
				setGameState (GameState.PlayerActive);
			}
		}
	}

	void Start () {
		setActive (false);
		Invoke ("delayedStart", 2f);
	}

	private void delayedStart() {
		startButton.SetActive (true);
		setGameState (GameState.ReadyToStart);
	}
	
	private void toggleHighlight(GameObject element, bool value) {
		foreach (VRTK_InteractableObject child in element.GetComponents<VRTK_InteractableObject>()) {
			child.ToggleHighlight (value);
		}
	}

	private int newSequenceValue() {
		return (int)((Random.value * 4));
	}

	private GameObject intToGameObject(int value) {
		switch (value) {
		case 0:
			return redButton;
		case 1:
			return greenButton;
		case 2:
			return blueButton;
		}
		return yellowButton; // 3
	}

	private int gameObjectToInt(GameObject value) {
		if (value == null) {
			return -1;
		}
		if (value.Equals(redButton)) {
			return 0;
		}
		if (value.Equals(greenButton)) {
			return 1;
		}
		if (value.Equals(blueButton)) {
			return 2;
		}
		return 3; // yellowButton
	}

	private void addToScore() {
		if (difficulty == 2) {
			setScore (score + 1);
		} else if (difficulty == 1) {
			setScore (score + 2);
		} else if (difficulty == 0.5) {
			setScore (score + 3);
		} else {
			setScore (score + 4);
		}
	}
}