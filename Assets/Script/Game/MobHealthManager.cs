using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobHealthManager : MonoBehaviour {
	public int life = 100;
	public bool isHealing = false;
	public float maxSleepingTime = 10f;

	private PhotonView view;
	private MobAI mobAIScript;
	private float sleepingTimer = 0f;
	private bool isSleeping = false;

	void Start () {
		view = GetComponent<PhotonView> ();
		mobAIScript = GetComponent<MobAI> () as MobAI;
	}

	void Update () {
		if (isSleeping && sleepingTimer > 0) {
			sleepingTimer -= Time.deltaTime;
		} else if (isSleeping && sleepingTimer <= 0) {
			mobAIScript.WakeUp ();
			isSleeping = false;
		}
	}

	public bool IsSleeping (){
		return isSleeping;
	}

	public void TakeDamage (int damage){
		view.RPC ("UpdateLife", PhotonTargets.AllBuffered, damage);
	}

	public void GetSleepy (float sleepingTime){
		view.RPC ("UpdateSleepingTimer", PhotonTargets.AllBuffered, sleepingTime);
	}

	[PunRPC]
	protected void UpdateLife(int damage){
		StartCoroutine (UpdateLifeMayDestroy(damage));
	}

	protected IEnumerator UpdateLifeMayDestroy(int damage){
		life -= damage;
		if (life <= 0) {
			mobAIScript.FallDown ();
			yield return new WaitForSeconds (5f);
			Destroy (gameObject);
		}
	}

	[PunRPC]
	protected void UpdateSleepingTimer(float sleepingTime){
		mobAIScript.FallDown ();

		isSleeping = true;

		float timer = sleepingTimer + sleepingTime;
		if (timer > maxSleepingTime) {
			sleepingTimer = maxSleepingTime;
		} else {
			sleepingTimer = timer;
		}
	}
}
