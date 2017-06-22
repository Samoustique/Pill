using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombiSpawner : MonoBehaviour {
	public float spawnRate = 2f;
	public GameObject objectToSpawn;
	public bool isActivated = true;
	public bool isSpawningOnlyOne = false;

	private float nextSpawn;
	private bool isContinuing = true;

	void Update () {
		if (isActivated && isContinuing && Time.time > nextSpawn) {
			if (PhotonNetwork.isMasterClient) {
				nextSpawn = Time.time + spawnRate;
				Spawn ();
			}
			isContinuing = !isSpawningOnlyOne;
		}
	}

	protected void Spawn(){
		PhotonNetwork.InstantiateSceneObject(objectToSpawn.name, transform.position, Quaternion.identity, 0, null);
	}
}
