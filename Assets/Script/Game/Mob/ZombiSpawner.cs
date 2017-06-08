using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombiSpawner : MonoBehaviour {
	public float spawnRate = 2f;
	public GameObject objectToSpawn;

	private float nextSpawn;

	void Update () {
		if(Time.time > nextSpawn){
			nextSpawn = Time.time + spawnRate;
			PhotonNetwork.Instantiate(objectToSpawn.name, transform.position, Quaternion.identity, 0);
		}
	}
}
