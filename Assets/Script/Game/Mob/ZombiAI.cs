using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombiAI : MonoBehaviour {
	public GameObject constantAudio;
	public AudioClip soundAttack;
	public float distanceAttack = 2f;
	public float distanceHeal = 2f;
	public int damage = 10;
	public float minSpeed = 1f;
	public float maxSpeed = 2f;

	private GameObject target;
	private Collider collider;
	private NavMeshAgent agent;
	private Animator anim;
	private AudioSource audioPunctualSource;
	private AudioSource audioConstantSource;
	private GameObject[] targetsPlayer;
	private GameObject[] targetsMedic;
	private bool isHealing;
	private HealthManager healthManagerScript;
	private List<Rigidbody> rigidBodies;
	private ZombiHurts zombiHurts;

	void Start () {
		audioPunctualSource = GetComponent<AudioSource> ();
		audioConstantSource = constantAudio.GetComponent<AudioSource> ();
		collider = GetComponent<Collider> ();
		anim = GetComponent<Animator> ();
		agent = GetComponent<NavMeshAgent> ();
		zombiHurts = GetComponentInChildren<ZombiHurts> ();

		rigidBodies = new List<Rigidbody>(GetComponentsInChildren<Rigidbody> ());
		rigidBodies.Remove (GetComponent<Rigidbody> ());
		agent.speed = Random.Range (minSpeed, maxSpeed);

		foreach(Rigidbody rb in rigidBodies){
			rb.isKinematic = true;
		}
	}

	void Update () {
		if (!isHealing) {
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
			isHealing = true;
			anim.SetTrigger ("eat");
			// stay still
			agent.SetDestination (transform.position);
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
		//audioPunctualSource.PlayOneShot (soundAttack);
		zombiHurts.NotifyIsHitting(damage);
	}

	public void Healed(){
		StartCoroutine (FallDown ());
	}

	private IEnumerator FallDown(){
		anim.enabled = false;
		agent.enabled = false;
		collider.enabled = false;
		audioPunctualSource.enabled = false;
		audioConstantSource.enabled = false;

		foreach(Rigidbody rb in rigidBodies){
			rb.isKinematic = false;
		}
		yield return new WaitForSeconds (3f);
		//Destroy (gameObject);
	}

}
