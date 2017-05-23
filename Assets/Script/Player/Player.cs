using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
	[PunRPC]
	private void SetPlayerName(string playerName){
		gameObject.name = playerName;
	}

	[PunRPC]
	private void ActivateSkin(string hero){
		foreach (Transform child in transform) {
			if (child.name == "Body") {
				foreach (Transform grandChild in child) {
					if (grandChild.name == hero) {
						grandChild.gameObject.SetActive (true);
						return;
					}
				}
			}
		}
	}
}
