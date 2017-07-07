﻿using System.Collections;
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
		if (view.isMine) {
			life -= damage;

			view.RPC ("UpdateRoomLife", PhotonTargets.OthersBuffered, PhotonNetwork.player.NickName, life);

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
		uiRoomManagerScript = GameObject.Find ("CanvasRoom").GetComponentInChildren<UIRoomManager> ();
		uiRoomManagerScript.UpdatePlayerLife (player, life);
	}
}
