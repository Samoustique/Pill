using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BtnRoom : MonoBehaviour {
	private Animator anim;
	private MenuManager managerScript;
	private bool isEnabled = true;

	void Awake () {
		anim = GetComponent<Animator> ();
		managerScript = GameObject.Find("CanvasMenu").GetComponent<MenuManager> () as MenuManager;
	}

	void OnMouseDown(){
		if (isEnabled && managerScript.selectedRoom != gameObject) {
			anim.SetTrigger ("press");
			managerScript.SelectRoom (gameObject);
		}
	}

	void OnMouseEnter(){
		if (isEnabled && managerScript.selectedRoom != gameObject) {
			anim.SetTrigger ("highlight");
		}
	}

	void OnMouseExit(){
		BackToNormal ();
	}

	public void Disable(){
		isEnabled = false;
		anim.SetTrigger ("disable");
	}

	public void Enable(){
		isEnabled = true;
		anim.SetTrigger ("basic");
	}

	public void BackToNormal(){
		if (isEnabled && managerScript.selectedRoom != gameObject) {
			anim.SetTrigger ("basic");
		}
	}

	public void NotFull(){
		isEnabled = true;

		if (managerScript.selectedRoom != gameObject) {
			anim.SetTrigger ("basic");
		} else {
			anim.SetTrigger ("press");
		}
	}
}
