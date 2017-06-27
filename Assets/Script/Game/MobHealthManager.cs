using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobHealthManager : MonoBehaviour {
	public int life = 100;
	public bool isHealing = false;

	private PhotonView view;

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
			MobAI mobAIScript = gameObject.GetComponent<MobAI> () as MobAI;
			mobAIScript.FallDown ();

			yield return new WaitForSeconds (5f);
			Destroy (gameObject);
		}
	}
}
