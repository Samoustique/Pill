using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HumanAI : MobAI {
	public float distanceChase = 20f;
	public float minRunningSpeed = 3f;
	public float maxRunningSpeed = 5f;

	private GameObject[] targetsZombis;
	private HumanHurts humanHurtsScript;
	private bool isFighting;
	private List<Transform> humanDestinations;
	private Transform nextDestination;
	private float runningSpeed;
	private float walkingSpeed;

	protected override void StartChild (){
		humanHurtsScript = GetComponentInChildren<HumanHurts> ();

		GameObject destinationsParent = GameObject.Find ("HumanDestinations");
		humanDestinations = new List<Transform> ();
		foreach (Transform child in destinationsParent.transform) {
			humanDestinations.Add (child);
		}

		runningSpeed = Random.Range (minRunningSpeed, maxRunningSpeed);
		walkingSpeed = Random.Range (minWalkingSpeed, maxWalkingSpeed);
	}

	protected override void UpdateChild (){
		targetsZombis = GameObject.FindGameObjectsWithTag ("Zombi");
		targetsZombis = RemoveDeadAndHealingZombis (targetsZombis);
		KeyValuePair<GameObject, float> closestZombi = GetClosest (targetsZombis);

		if (closestZombi.Key == null || closestZombi.Value > distanceChase) {
			GoForRandomDestination ();
		} else {
			GoForZombi (closestZombi);
		}
	}

	private GameObject[] RemoveDeadAndHealingZombis (GameObject[] objects){
		List<GameObject> aliveZombis = new List<GameObject>();

		foreach(GameObject obj in objects){
			MobHealthManager healthScript = obj.GetComponent<MobHealthManager> () as MobHealthManager;
			if (!healthScript.isHealing && healthScript.life > 0) {
				aliveZombis.Add (obj);
			}
		}
		return aliveZombis.ToArray();
	}

	private void GoForRandomDestination (){
		if (nextDestination == null || (anim.isActiveAndEnabled && agent.remainingDistance <= float.Epsilon)) {
			nextDestination = humanDestinations [Random.Range (0, humanDestinations.Count)];
		}

		anim.SetBool ("walk", true);
		anim.SetBool ("run", false);
		agent.SetDestination (nextDestination.position);
		agent.speed = walkingSpeed;
	}

	private void GoForZombi (KeyValuePair<GameObject, float> zombi){
		if (zombi.Value < distanceAttack) {
			// stay still
			agent.SetDestination (transform.position);
			MobAI mobAIScript = zombi.Key.GetComponent<MobAI> () as MobAI;
			if (mobAIScript.IsMobAwakeAndUp ()) {
				anim.SetTrigger ("attack");
			} else {
				anim.SetTrigger ("attackDown");
			}
		} else if (!isFighting) {
			anim.SetBool ("run", true);
			anim.SetBool ("walk", false);
			agent.SetDestination (zombi.Key.transform.position);
			agent.speed = runningSpeed;
		}
	}

	protected override void MakeDamage(){
		humanHurtsScript.NotifyIsHitting(damage);
	}

	protected override void DisableBoolAnimChild (){
		anim.SetBool ("walk", false);
		anim.SetBool ("run", false);
	}

	private void StopMoving(){
		isFighting = true;
		// stay still
		agent.enabled = true;
		agent.SetDestination (transform.position);
	}

	private void CanMove(){
		isFighting = false;
	}
}
