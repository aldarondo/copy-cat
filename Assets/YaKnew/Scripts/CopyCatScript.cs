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
	private bool active = false;
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

	private void setScore(int pointsToAdd) {
		score = score + pointsToAdd;
		foreach (Text value in scoreValue.GetComponents<Text>()) {
			value.text = score.ToString ();
		}
	}

	private void incrementLevel() {
		level = level + 1;
		foreach (Text value in scoreValue.GetComponents<Text>()) {
			value.text = level.ToString ();
		}
	}

	private void incrementTurn() {
		turn = turn + 1;
	}

	private void resetTurn() {
		turn = 0;
	}

	private void resetPlayerSequence() {
		plaSequence = new List<int> ();
	}

	private void resetGeneratedSequence() {
		genSequence = new List<int> ();
	}

	private void setActive(bool isActive) {
		active = isActive;
		foreach (VRTK_InteractableObject obj in redButton.GetComponents<VRTK_InteractableObject>()) {
			obj.highlightOnTouch = active;
		}
		foreach (VRTK_InteractableObject obj in greenButton.GetComponents<VRTK_InteractableObject>()) {
			obj.highlightOnTouch = active;
		}
		foreach (VRTK_InteractableObject obj in blueButton.GetComponents<VRTK_InteractableObject>()) {
			obj.highlightOnTouch = active;
		}
		foreach (VRTK_InteractableObject obj in yellowButton.GetComponents<VRTK_InteractableObject>()) {
			obj.highlightOnTouch = active;
		}
	}

	public void BeginGame() {
		Debug.Log ("Start BeginGame");
		resetGeneratedSequence ();
		resetPlayerSequence ();
		startButton.SetActive (false);
		setActive (true);
		Debug.Log ("End BeginGame");
	}

	public void LogPlayerSequence(GameObject pad) {
		int value = gameObjectToInt (pad);
		plaSequence.Add (value);
		checkSelectedPad (value);
	}

	private IEnumerator checkSelectedPad(int pad) {
		if (pad != genSequence [turn]) {
			incorrectSequence ();
		} else {
			keepScore ();
			incrementTurn ();
		}
		if (turn == genSequence.Count) {
			incrementLevel ();
			setActive (false);
			yield return new WaitForSeconds(1);
			newLevel();
		}
	}

	private void newLevel() {
		resetPlayerSequence ();
		genSequence.Add (newSequenceValue ());
		resetTurn ();
		setActive (true);
		displaySequence ();
	}

	// Use this for initialization
	void Start () {
		setActive (false);
	}
	
	// Update is called once per frame
	void Update () {
	}

	private IEnumerator flash(GameObject element, int times, float speed) {
		foreach (VRTK_InteractableObject child in element.GetComponents<VRTK_InteractableObject>()) {
			for (int i = 0; i < times; i++) {
				child.ToggleHighlight (true);
				yield return new WaitForSeconds (speed);
				child.StopTouching (element);
			}
		}
	}

	private int newSequenceValue() {
		return (int)((Random.value * 4) + 1);
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
		if (value == redButton) {
			return 0;
		}
		if (value == greenButton) {
			return 1;
		}
		if (value == blueButton) {
			return 2;
		}
		return 3; // yellowButton
	}

	private IEnumerable displaySequence() {
		foreach (int value in genSequence) {
			flash (intToGameObject (value), 1, 0.3f);
			yield return new WaitForSeconds (0.5f * difficulty);
		}
	}

	private void keepScore() {
		if (difficulty == 2) {
			setScore (1);
		} else if (difficulty == 1) {
			setScore (2);
		} else if (difficulty == 0.5) {
			setScore (3);
		} else {
			setScore (4);
		}
	}

	private IEnumerable incorrectSequence() {
		GameObject correctPad = intToGameObject (genSequence [turn]);
		setActive (false);
		flash (correctPad, 4, 0.3f);
		yield return new WaitForSeconds (0.5f);
		startButton.SetActive (true);
	}
}