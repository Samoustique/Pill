﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombiSpawner : MonoBehaviour {
	public float spawnRate = 2f;
	public GameObject objectToSpawn;

	private float nextSpawn;
	private bool isContinue = true;

	void Update () {
		if(Time.time > nextSpawn && isContinue){
			nextSpawn = Time.time + spawnRate;
			PhotonNetwork.Instantiate(objectToSpawn.name, transform.position, Quaternion.identity, 0);
			isContinue = false;
		}
	}
}
