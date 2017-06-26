using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombiHurts : MonoBehaviour {
	public AudioClip soundAttack;

	private bool isTouchingPlayer = false;
	private GameObject player;
	private AudioSource audioPunctualSource;

	void Start(){
		audioPunctualSource = GetComponent<AudioSource> ();
	}

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

	public void NotifyIsHitting(int damage){
		if (isTouchingPlayer) {
			audioPunctualSource.PlayOneShot (soundAttack);

			HealthManager healthManagerScript = player.GetComponentInChildren<HealthManager> ();
			healthManagerScript.TakeDamage (damage);
		}
	}
}
