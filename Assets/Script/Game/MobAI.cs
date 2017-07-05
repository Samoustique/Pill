using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class MobAI : MonoBehaviour {
	public GameObject constantAudio;
	public float distanceAttack = 2f;
	public int damage = 10;
	public float minWalkingSpeed = 1f;
	public float maxWalkingSpeed = 2f;
	public GameObject[] bulletPrefabs;

	protected GameObject target;
	protected Collider col;
	protected NavMeshAgent agent;
	protected Animator anim;
	protected PhotonView view;
	protected AudioSource audioPunctualSource;
	protected AudioSource audioConstantSource;
	protected MobHealthManager healthManagerScript;

	/*protected float impactEndTime = 0;
	protected Vector3 impact;
	protected Rigidbody impactTarget = null;*/

	private bool isWakingUp = false;
	private MobGetUp getUpScript;

	void Start () {
		audioPunctualSource = GetComponent<AudioSource> ();
		audioConstantSource = constantAudio.GetComponent<AudioSource> ();
		col = GetComponent<Collider> ();
		view = GetComponent<PhotonView> ();
		anim = GetComponent<Animator> ();
		agent = GetComponent<NavMeshAgent> ();
		healthManagerScript = GetComponentInChildren<MobHealthManager> ();
		getUpScript = GetComponent<MobGetUp> () as MobGetUp;

		StartChild ();
	}

	void Update () {
		if (IsMobFree()) {
			if (!healthManagerScript.IsSleeping ()) {
				if (!isWakingUp) {
					UpdateChild ();
				}
			}

			/*if (Time.time < impactEndTime && impactTarget != null) {
				view.RPC ("AddForceToRigidBody", PhotonTargets.MasterClient);
			}*/
		}
	}

	public bool IsMobFree(){
		return !healthManagerScript.isHealing && healthManagerScript.life > 0;
	}

	protected abstract void StartChild ();
	protected abstract void UpdateChild ();
	protected abstract void MakeDamage ();

	protected KeyValuePair<GameObject, float> GetClosest(GameObject[] objects){
		float min = float.MaxValue;
		float distance;
		GameObject minObj = null;

		foreach(GameObject obj in objects){
			distance = Vector3.Distance (obj.transform.position, transform.position);
			if (distance < min) {
				min = distance;
				minObj = obj;
			}
		}

		return new KeyValuePair<GameObject, float> (minObj, min);
	}

	public void FallDown(){
		view.RPC ("DisableMob", PhotonTargets.AllBuffered);
	}

	public void WakeUp(){
		isWakingUp = true;
		view.RPC ("EnableMob", PhotonTargets.AllBuffered);
	}

	public void TakeDamage (GameObject gameObjectHit, Vector3 direction, int damage){
		if (healthManagerScript.life > 0) {
			healthManagerScript.TakeDamage(damage);
		}

		view.RPC ("MoveRigidBody", PhotonTargets.AllBuffered, direction, gameObjectHit.transform.name);
	}

	public void GetSleepy (GameObject gameObjectHit, Vector3 direction, float sleepingTime){
		if (healthManagerScript.life > 0) {
			healthManagerScript.GetSleepy(sleepingTime);
		}

		view.RPC ("MoveRigidBody", PhotonTargets.AllBuffered, direction, gameObjectHit.transform.name);
	}

	[PunRPC]
	protected void MoveRigidBody(Vector3 direction, string objectHitName){
		/*impact = direction * 2.0f;
		impactEndTime = Time.time + 0.25f;

		GameObject target = Utilities.FindGameObject (objectHitName, view.gameObject);
		if (target != null) {
			impactTarget = target.GetComponent<Rigidbody> ();
		} else {
			impactTarget = view.gameObject.GetComponent<Rigidbody> ();
		}*/
	}

	[PunRPC]
	protected void DisableMob(){
		anim.enabled = false;
		audioPunctualSource.enabled = false;
		audioConstantSource.enabled = false;

		agent.enabled = false;
		getUpScript.ragdolled = true;
	}

	[PunRPC]
	protected void AddForceToRigidBody(){
		//impactTarget.AddForce (impact, ForceMode.VelocityChange);
	}

	[PunRPC]
	protected void EnableMob(){
		anim.enabled = true;
		audioPunctualSource.enabled = true;
		audioConstantSource.enabled = true;

		getUpScript.ragdolled = false;
	}

	private void UpReadyToGo(){
		agent.enabled = true;
		isWakingUp = false;
	}
}
