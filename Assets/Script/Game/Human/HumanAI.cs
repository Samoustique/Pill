using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HumanAI : MonoBehaviour {
	public GameObject constantAudio;
	public AudioClip soundAttack;
	public float distanceAttack = 2f;
	public float distanceChase = 20f;
	public float distanceHeal = 2f;
	public int damage = 10;
	public float minRunningSpeed = 3f;
	public float maxRunningSpeed = 5f;
	public float minWalkingSpeed = 1f;
	public float maxWalkingSpeed = 2f;

	private GameObject target;
	private NavMeshAgent agent;
	private Animator anim;
	private AudioSource audioPunctualSource;
	private AudioSource audioConstantSource;
	private GameObject[] targetsZombis;
	private bool isHealing;
	private MobHealthManager healthManagerScript;
	private ZombiHurts zombiHurtsScript;
	private List<Rigidbody> rigidBodies;
	private HumanHurts humanHurts;
	private bool isFighting;
	private List<Transform> humanDestinations;
	private Transform nextDestination;
	private float runningSpeed;
	private float walkingSpeed;
	private float impactEndTime = 0;
	private Vector3 impact;
	private Rigidbody impactTarget = null;
	private PhotonView view;
	private Collider col;

	void Start () {
		audioPunctualSource = GetComponent<AudioSource> ();
		audioConstantSource = constantAudio.GetComponent<AudioSource> ();
		col = GetComponent<Collider> ();
		view = GetComponent<PhotonView> ();
		anim = GetComponent<Animator> ();
		agent = GetComponent<NavMeshAgent> ();
		humanHurts = GetComponentInChildren<HumanHurts> ();
		zombiHurtsScript = GetComponentInChildren<ZombiHurts> ();
		healthManagerScript = GetComponentInChildren<MobHealthManager> ();

		GameObject destinationsParent = GameObject.Find ("HumanDestinations");
		humanDestinations = new List<Transform> ();
		foreach (Transform child in destinationsParent.transform) {
			humanDestinations.Add (child);
		}

		rigidBodies = new List<Rigidbody>(GetComponentsInChildren<Rigidbody> ());
		rigidBodies.Remove (GetComponent<Rigidbody> ());
		runningSpeed = Random.Range (minRunningSpeed, maxRunningSpeed);
		walkingSpeed = Random.Range (minWalkingSpeed, maxWalkingSpeed);

		foreach(Rigidbody rb in rigidBodies){
			rb.isKinematic = true;
		}
	}

	void Update () {
		if (!healthManagerScript.isHealing) {
			if (healthManagerScript.life > 0) {
				targetsZombis = GameObject.FindGameObjectsWithTag ("Zombi");
				targetsZombis = RemoveDeadZombis (targetsZombis);
				KeyValuePair<GameObject, float> closestZombi = GetClosest (targetsZombis);

				if (closestZombi.Key == null || closestZombi.Value > distanceChase) {
					GoForRandomDestination ();
				} else {
					GoForZombi (closestZombi);
				}
			}

			if (Time.time < impactEndTime && impactTarget != null) {
				impactTarget.AddForce (impact, ForceMode.VelocityChange);
			}
		}
	}

	private GameObject[] RemoveDeadZombis (GameObject[] objects){
		List<GameObject> aliveZombis = new List<GameObject>();

		foreach(GameObject obj in objects){
			MobHealthManager healthScript = obj.GetComponent<MobHealthManager> () as MobHealthManager;
			if (healthScript.life > 0) {
				aliveZombis.Add (obj);
			}
		}

		return aliveZombis.ToArray();
	}

	private void GoForRandomDestination (){
		if (nextDestination == null || agent.remainingDistance <= float.Epsilon) {
			nextDestination = humanDestinations [Random.Range (0, humanDestinations.Count)];
			Debug.Log ("dest : " + nextDestination.position);
		}

		anim.SetBool ("walk", true);
		agent.SetDestination (nextDestination.position);
		agent.speed = walkingSpeed;
	}

	private void GoForZombi (KeyValuePair<GameObject, float> zombi){
		if (zombi.Value < distanceAttack) {
			anim.SetTrigger ("attack");
			// stay still
			agent.SetDestination (transform.position);
		} else if (!isFighting) {
			anim.SetBool ("run", true);
			agent.SetDestination (zombi.Key.transform.position);
			agent.speed = runningSpeed;
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

	private void DamageToZombi(){
		//audioPunctualSource.PlayOneShot (soundAttack);
		humanHurts.NotifyIsHitting(damage);
	}

	private void StopMoving(){
		isFighting = true;
		// stay still
		agent.SetDestination (transform.position);
	}

	private void CanMove(){
		isFighting = false;
	}

	private void FallDown(){
		view.RPC ("DisableHuman", PhotonTargets.AllBuffered);
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
	protected void DisableHuman(){
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
