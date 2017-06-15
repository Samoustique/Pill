using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanSpawner : MonoBehaviour {
	public float spawnRate = 2f;
	public GameObject objectToSpawn;

	private float nextSpawn;
	private bool isContinue = true;

	void Update () {
		if (isContinue && Time.time > nextSpawn) {
			if (PhotonNetwork.isMasterClient) {
				nextSpawn = Time.time + spawnRate;
				Spawn ();
			}
			isContinue = false;
		}
	}

	protected void Spawn(){
		PhotonNetwork.InstantiateSceneObject(objectToSpawn.name, transform.position, Quaternion.identity, 0, null);
	}
}
