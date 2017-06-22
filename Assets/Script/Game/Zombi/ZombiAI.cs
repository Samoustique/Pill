using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombiAI : MonoBehaviour {
	public GameObject constantAudio;
	public AudioClip soundAttack;
	public float distanceAttack = 2f;
	public float distanceHeal = 2.5f;
	public int damage = 10;
	public float minSpeed = 1f;
	public float maxSpeed = 2f;

	private GameObject target;
	private Collider col;
	private NavMeshAgent agent;
	private Animator anim;
	private AudioSource audioPunctualSource;
	private AudioSource audioConstantSource;
	private GameObject[] targetsPlayer;
	private GameObject[] targetsMedic;
	private MobHealthManager healthManagerScript;
	private ZombiHurts zombiHurtsScript;
	private List<Rigidbody> rigidBodies;
	private float impactEndTime = 0;
	private Vector3 impact;
	private Rigidbody impactTarget = null;
	private PhotonView view;

	void Start () {
		audioPunctualSource = GetComponent<AudioSource> ();
		audioConstantSource = constantAudio.GetComponent<AudioSource> ();
		col = GetComponent<Collider> ();
		view = GetComponent<PhotonView> ();
		anim = GetComponent<Animator> ();
		agent = GetComponent<NavMeshAgent> ();
		zombiHurtsScript = GetComponentInChildren<ZombiHurts> ();
		healthManagerScript = GetComponentInChildren<MobHealthManager> ();

		rigidBodies = new List<Rigidbody>(GetComponentsInChildren<Rigidbody> ());
		rigidBodies.Remove (GetComponent<Rigidbody> ());
		agent.speed = Random.Range (minSpeed, maxSpeed);

		foreach(Rigidbody rb in rigidBodies){
			rb.isKinematic = true;
		}
	}

	void Update () {
		if (!healthManagerScript.isHealing) {
			if (healthManagerScript.life > 0) {
				targetsPlayer = GameObject.FindGameObjectsWithTag ("Player");
				targetsMedic = GameObject.FindGameObjectsWithTag ("Medic");
				KeyValuePair<GameObject, float> closestPlayer = GetClosest (targetsPlayer);
				KeyValuePair<GameObject, float> closestMedic = GetClosest (targetsMedic);

				if (closestPlayer.Key == null && closestMedic.Key != null) {
					GoForMedic (closestMedic);
				} else if (closestPlayer.Key != null && closestMedic.Key == null) {
					GoForPlayer (closestPlayer);
				} else if (closestPlayer.Key != null && closestMedic.Key != null) {
					if (closestPlayer.Value < closestMedic.Value) {
						GoForPlayer (closestPlayer);
					} else {
						GoForMedic (closestMedic);
					}
				}
			}

			if (Time.time < impactEndTime && impactTarget != null) {
				impactTarget.AddForce (impact, ForceMode.VelocityChange);
			}
		}
	}

	private void GoForPlayer (KeyValuePair<GameObject, float> player){
		if (player.Value < distanceAttack) {
			anim.SetBool ("attack", true);
			// stay still
			agent.SetDestination (transform.position);
		} else {
			anim.SetBool ("walk", true);
			anim.SetBool ("attack", false);
			agent.SetDestination (player.Key.transform.position);
		}
	}
		
	private void GoForMedic (KeyValuePair<GameObject, float> medic){
		anim.SetBool ("attack", false);

		if (medic.Value < distanceHeal) {
			healthManagerScript.isHealing = true;
			anim.SetTrigger ("eat");
			// stay still
			agent.SetDestination (transform.position);
			agent.enabled = false;
			col.enabled = false;
		} else {
			anim.SetBool ("walk", true);
			agent.SetDestination (medic.Key.transform.position);
		}
	}

	private KeyValuePair<GameObject, float> GetClosest(GameObject[] objects){
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

	public void DamageToPlayer(){
		audioPunctualSource.PlayOneShot (soundAttack);
		zombiHurtsScript.NotifyIsHitting(damage);
	}

	public void Healed(){
		FallDown ();
	}

	private void FallDown(){
		view.RPC ("DisableZombi", PhotonTargets.AllBuffered);
	}

	public void IsHurt (GameObject gameObjectHit, Vector3 direction, int damage){
		if (healthManagerScript.life > 0) {
			healthManagerScript.TakeDamage(damage);
			FallDown ();
		}

		impactTarget = gameObjectHit.GetComponent<Rigidbody> ();
		view.RPC ("MoveRigidBody", PhotonTargets.AllBuffered, direction);
	}
		
	[PunRPC]
	protected void MoveRigidBody(Vector3 direction){
		impact = direction * 2.0f;
		impactEndTime = Time.time + 0.25f;
	}

	[PunRPC]
	protected void DisableZombi(){
		anim.enabled = false;
		audioPunctualSource.enabled = false;
		audioConstantSource.enabled = false;

		foreach(Rigidbody rb in rigidBodies){
			rb.isKinematic = false;
		}

		// stay still
		agent.SetDestination (transform.position);
		agent.enabled = false;
		col.enabled = false;
	}
}
