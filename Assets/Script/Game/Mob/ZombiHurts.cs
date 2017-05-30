using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombiHurts : MonoBehaviour {

	private bool isTouchingPlayer = false;
	private GameObject player;

	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "Player") {
			player = other.gameObject;
			isTouchingPlayer = true;
		}
	}

	void OnTriggerExit(Collider other){
		if (other.gameObject.tag == "Player") {
			player = null;
			isTouchingPlayer = false;
		}
	}

	public void NotifyIsHitting(){
		if (isTouchingPlayer) {
			HealthManager healthManagerScript = player.GetComponentInChildren<HealthManager> ();
			healthManagerScript.TakeDamage (10);
		}
	}
}
