using UnityEngine;
using System.Collections;
using VRTK;

public class StartButton : VRTK_InteractableObject {
	public GameObject gameCase;

	public override void ToggleHighlight (bool toggle, Color globalHighlightColor)
	{
		base.ToggleHighlight (toggle, globalHighlightColor);
		if (gameCase != null) {
			CopyCatScript script = gameCase.GetComponent<CopyCatScript> ();
			if (script != null) {
				script.BeginGame ();
			}
		}
	}
}
