using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobHealthManager : MonoBehaviour {
	public int life = 100;
	public bool isHealing = false;

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
		Debug.Log ("life : " + life + " damage : " + damage);
		life -= damage;
		if (life <= 0) {
			// to remove
			Debug.Log ("boum");
			MobAI mobAIScript = gameObject.GetComponent<MobAI> () as MobAI;
			mobAIScript.FallDown ();
			//////////

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
