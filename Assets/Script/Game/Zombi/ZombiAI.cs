﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombiAI : MobAI {
	public float distanceHeal = 2.5f;

	private GameObject[] targetsPlayer;
	private GameObject[] targetsMedic;
	private ZombiHurts zombiHurtsScript;

	protected override void StartChild (){
		zombiHurtsScript = GetComponentInChildren<ZombiHurts> ();
		agent.speed = Random.Range (minWalkingSpeed, maxWalkingSpeed);
	}

	protected override void UpdateChild (){
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

	public void DamageToPlayer(){
		audioPunctualSource.PlayOneShot (soundAttack);
		zombiHurtsScript.NotifyIsHitting(damage);
	}

	public void Healed(){
		FallDown ();
	}
}
