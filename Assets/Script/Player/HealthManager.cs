using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour {

	public int life = 100;
	
	private UIManager uiManagerScript;
	private bool isDead = false;
	private PhotonView view;

	void Start () {
		view = GetComponent<PhotonView> ();

		if (view.isMine) {
			uiManagerScript = GameObject.Find ("CanvasPlayer").GetComponentInChildren<UIManager> ();
			uiManagerScript.UpdateLife (life);
		}
	}

	public void TakeDamage (int damage){
		life -= damage;

		if (view.isMine) {
			uiManagerScript.UpdateLife (life);

			if (life <= 0 && !isDead) {
				isDead = true;
				//GetComponent<AudioSource> ().PlayOneShot (soundPlayerDead);
				//GetComponent<PlayerDead> ().Die ();
			}
		}
	}
}
