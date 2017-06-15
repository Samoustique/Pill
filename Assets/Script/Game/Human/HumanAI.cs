using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HumanAI : MonoBehaviour {
	public GameObject constantAudio;
	public AudioClip soundAttack;
	public float distanceAttack = 2f;
	public float distanceHeal = 2f;
	public int damage = 10;
	public float minSpeed = 1f;
	public float maxSpeed = 2f;

	private GameObject target;
	private NavMeshAgent agent;
	private Animator anim;
	private AudioSource audioPunctualSource;
	private AudioSource audioConstantSource;
	private GameObject[] targetsZombis;
	private bool isHealing;
	private HealthManager healthManagerScript;
	private List<Rigidbody> rigidBodies;
	private HumanHurts humanHurts;

	void Start () {
		audioPunctualSource = GetComponent<AudioSource> ();
		audioConstantSource = constantAudio.GetComponent<AudioSource> ();
		anim = GetComponent<Animator> ();
		agent = GetComponent<NavMeshAgent> ();
		humanHurts = GetComponentInChildren<HumanHurts> ();

		rigidBodies = new List<Rigidbody>(GetComponentsInChildren<Rigidbody> ());
		rigidBodies.Remove (GetComponent<Rigidbody> ());
		agent.speed = Random.Range (minSpeed, maxSpeed);

		foreach(Rigidbody rb in rigidBodies){
			rb.isKinematic = true;
		}
	}

	void Update () {
		targetsZombis = GameObject.FindGameObjectsWithTag ("Zombi");
		KeyValuePair<GameObject, float> closestZombi = GetClosest (targetsZombis);

		if (closestZombi.Key == null) {
			anim.SetBool ("walk", false);
			anim.SetBool ("attack", false);
			// stay still
			agent.SetDestination (transform.position);
		} else {
			GoForZombi (closestZombi);
		}

		Debug.Log(agent.destination);
	}

	private void GoForZombi (KeyValuePair<GameObject, float> zombi){
		if (zombi.Value < distanceAttack) {
			anim.SetBool ("attack", true);
			// stay still
			agent.SetDestination (transform.position);
		} else {
			anim.SetBool ("walk", true);
			anim.SetBool ("attack", false);
			agent.SetDestination (zombi.Key.transform.position);
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

	public void DamageToZombi(){
		//audioPunctualSource.PlayOneShot (soundAttack);
		humanHurts.NotifyIsHitting(damage);
	}

	private IEnumerator FallDown(){
		anim.enabled = false;
		audioPunctualSource.enabled = false;
		audioConstantSource.enabled = false;

		foreach(Rigidbody rb in rigidBodies){
			rb.isKinematic = false;
		}
		yield return new WaitForSeconds (3f);
		//Destroy (gameObject);
	}

}
