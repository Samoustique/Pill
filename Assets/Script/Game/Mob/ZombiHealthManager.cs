using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombiHealthManager : MonoBehaviour {
	public int life = 100;

	private PhotonView view;

	// Use this for initialization
	void Start () {
		view = GetComponent<PhotonView> ();
	}

	public void TakeDamage (int damage){
		view.RPC ("UpdateZombiLife", PhotonTargets.AllBuffered, damage);
	}

	[PunRPC]
	protected void UpdateZombiLife(int damage){
		life -= damage;
		if (life <= 0) {
			Destroy (gameObject);
		}
	}
}
