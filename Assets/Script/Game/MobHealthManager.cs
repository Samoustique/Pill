using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobHealthManager : MonoBehaviour {
	public int life = 100;
	public bool isHealing = false;
	public float maxSleepingTime = 10f;
	public GameObject timerCirclePrefab;

	private PhotonView view;
	private MobAI mobAIScript;
	private float sleepingTimer = 0f;
	private bool isSleeping = false;
	private GameObject canvas;
	private GameObject timerCircle;

	void Start () {
		view = GetComponent<PhotonView> () as PhotonView;
		mobAIScript = GetComponent<MobAI> () as MobAI;
		canvas = GameObject.Find ("CanvasPlayer");
	}

	void Update () {
		if (timerCircle != null) {
			Vector3 heading = transform.position - Camera.main.transform.position;
			if (Vector3.Dot (Camera.main.transform.forward, heading) > 0) {
				timerCircle.transform.position = Camera.main.WorldToScreenPoint (transform.position);
			}
		} else if (isSleeping) {
			// the mob has just woken up
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
		if (timerCircle == null) {
			timerCircle = Instantiate (timerCirclePrefab, Vector3.zero, Quaternion.identity) as GameObject;
			timerCircle.transform.SetParent (canvas.transform, false);

			Vector3 heading = transform.position - Camera.main.transform.position;
			if (Vector3.Dot (Camera.main.transform.forward, heading) > 0) {
				timerCircle.transform.position = Camera.main.WorldToScreenPoint (transform.position);
			}

			SleepingTimer sleepingTimerScript = timerCircle.GetComponent<SleepingTimer> () as SleepingTimer;
			sleepingTimerScript.SetMaxTimer (maxSleepingTime);
			sleepingTimerScript.SetMobPosition (transform.position);
		}
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

		SleepingTimer sleepingTimerScript = timerCircle.GetComponent<SleepingTimer> () as SleepingTimer;
		sleepingTimerScript.UpdateTimer (sleepingTime);
	}
}
