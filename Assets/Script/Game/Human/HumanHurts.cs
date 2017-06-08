using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanHurts : MonoBehaviour {

	private bool isTouchingZombi = false;
	private GameObject player;

	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "Zombi") {
			player = other.gameObject;
			isTouchingZombi = true;
		}
	}

	void OnTriggerExit(Collider other){
		if (other.gameObject.tag == "Zombi") {
			player = null;
			isTouchingZombi = false;
		}
	}

	public void NotifyIsHitting(int damage){
		if (isTouchingZombi) {
			ZombiHealthManager healthManagerScript = player.GetComponentInChildren<ZombiHealthManager> ();
			healthManagerScript.TakeDamage (damage);
		}
	}
}
