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
	protected List<Rigidbody> rigidBodies;

	protected float impactEndTime = 0;
	protected Vector3 impact;
	protected Rigidbody impactTarget = null;

	void Start () {
		audioPunctualSource = GetComponent<AudioSource> ();
		audioConstantSource = constantAudio.GetComponent<AudioSource> ();
		col = GetComponent<Collider> ();
		view = GetComponent<PhotonView> ();
		anim = GetComponent<Animator> ();
		agent = GetComponent<NavMeshAgent> ();
		healthManagerScript = GetComponentInChildren<MobHealthManager> ();

		rigidBodies = new List<Rigidbody>(GetComponentsInChildren<Rigidbody> ());
		rigidBodies.Remove (GetComponent<Rigidbody> ());
		foreach(Rigidbody rb in rigidBodies){
			rb.isKinematic = true;
		}

		StartChild ();
	}

	void Update () {
		if (!healthManagerScript.isHealing) {
			if (!healthManagerScript.IsSleeping ()) {
				if (healthManagerScript.life > 0) {
					UpdateChild ();
				}
			}

			if (Time.time < impactEndTime && impactTarget != null) {
				impactTarget.AddForce (impact, ForceMode.VelocityChange);
			}
		}
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
		impact = direction * 2.0f;
		impactEndTime = Time.time + 0.25f;
		Debug.Log ("objectHitName " + objectHitName);

		GameObject target = Utilities.FindGameObject (objectHitName, view.gameObject);
		Debug.Log ("target " + target);
		Debug.Log ("target name " + target.name);
		Debug.Log ("impactTarget " + impactTarget);

		if (target != null) {
			impactTarget = target.GetComponent<Rigidbody> ();
			Debug.Log ("impactTarget 1" + impactTarget);

		} else {
			impactTarget = view.gameObject.GetComponent<Rigidbody> ();
			Debug.Log ("impactTarget 2" + impactTarget);

		}
	}

	[PunRPC]
	protected void DisableMob(){
		anim.enabled = false;
		audioPunctualSource.enabled = false;
		audioConstantSource.enabled = false;

		foreach(Rigidbody rb in rigidBodies){
			rb.isKinematic = false;
		}

		// stay still
		//agent.SetDestination (transform.position);
		agent.enabled = false;
		//col.enabled = false;
	}

	[PunRPC]
	protected void EnableMob(){
		anim.enabled = true;
		audioPunctualSource.enabled = true;
		audioConstantSource.enabled = true;

		foreach(Rigidbody rb in rigidBodies){
			rb.isKinematic = true;
		}

		// stay still
		//agent.SetDestination (transform.position);
		agent.enabled = true;
		//col.enabled = true;

		Debug.Log(anim.isActiveAndEnabled);
		Debug.Log(anim.GetBoneTransform(HumanBodyBones.Hips));
		Debug.Log(anim.GetBoneTransform(HumanBodyBones.Head));
		Debug.Log(anim.GetBoneTransform(HumanBodyBones.Spine));
		Debug.Log(anim.GetBoneTransform(HumanBodyBones.UpperChest));
		Debug.Log(anim.GetBoneTransform(HumanBodyBones.Neck));
		Debug.Log(anim.GetBoneTransform(HumanBodyBones.Jaw));
		Debug.Log(anim.GetBoneTransform(HumanBodyBones.Chest));
		Debug.Log("----");

		if (anim.GetBoneTransform(HumanBodyBones.Hips).forward.y>0) //hip hips forward vector pointing upwards, initiate the get up from back animation
		{
			anim.SetTrigger("getUpBack");
		}
		else{
			anim.SetTrigger("getUpFront");
		}
	}
}
