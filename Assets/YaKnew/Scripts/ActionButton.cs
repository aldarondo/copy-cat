using UnityEngine;
using System.Collections;
using VRTK;

public class ActionButton : VRTK_InteractableObject {
	public AudioSource soundOnTouch;

	public override void ToggleHighlight (bool toggle, Color globalHighlightColor)
	{
		base.ToggleHighlight (toggle, globalHighlightColor);
		if (highlightOnTouch && toggle && soundOnTouch != null && !soundOnTouch.isPlaying) {
			soundOnTouch.mute = false;
			soundOnTouch.Play ();
			CopyCatScript script = GetComponentInParent<CopyCatScript> ();
			if (script != null) {	
				script.LogPlayerSequence (gameObject);
			}
		}
	}
}
