using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour {

	public int life = 100;
	
	private UIPlayerManager uiPlayerManagerScript;
	private UIRoomManager uiRoomManagerScript;
	private bool isDead = false;
	private PhotonView view;

	void Start () {
		view = GetComponent<PhotonView> ();

		uiRoomManagerScript = GameObject.Find ("CanvasRoom").GetComponentInChildren<UIRoomManager> ();

		if (view.isMine) {
			uiPlayerManagerScript = GameObject.Find ("CanvasPlayer").GetComponentInChildren<UIPlayerManager> ();
			uiPlayerManagerScript.UpdateLife (life);
		}
	}

	public void TakeDamage (int damage){
		life -= damage;

		view.RPC ("UpdateRoomLife", PhotonTargets.OthersBuffered, PhotonNetwork.player.NickName, life);

		if (view.isMine) {
			uiPlayerManagerScript.UpdateLife (life);

			if (life <= 0 && !isDead) {
				isDead = true;
				//GetComponent<AudioSource> ().PlayOneShot (soundPlayerDead);
				//GetComponent<PlayerDead> ().Die ();
			}
		}
	}

	[PunRPC]
	protected void UpdateRoomLife(string player, int life){
		Debug.Log (player + " " + life);
		uiRoomManagerScript.UpdatePlayerLife (player, life);
	}
}
