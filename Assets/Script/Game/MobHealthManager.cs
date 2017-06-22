using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobHealthManager : MonoBehaviour {
	public int life = 100;
	public bool isHealing;

	private PhotonView view;

	// Use this for initialization
	void Start () {
		view = GetComponent<PhotonView> ();
	}

	public void TakeDamage (int damage){
		view.RPC ("UpdateLife", PhotonTargets.AllBuffered, damage);
	}

	[PunRPC]
	protected void UpdateLife(int damage){
		StartCoroutine (UpdateLifeMayDestroy(damage));
	}
		
	protected IEnumerator UpdateLifeMayDestroy(int damage){
		life -= damage;
		if (life <= 0) {
			yield return new WaitForSeconds (5f);
			Destroy (gameObject);

			//view.RPC ("DestroyMob", PhotonTargets.AllBuffered);
		}
	}

	[PunRPC]
	protected void DestroyMob(){
		PhotonNetwork.Destroy (view);
	}

}
