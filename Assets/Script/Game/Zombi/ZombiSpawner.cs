using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombiSpawner : MonoBehaviour {
	public float spawnRate = 2f;
	public GameObject objectToSpawn;
	public int nbMaxZombi = 10;
	public bool isActivated = true;
	public bool hasToStopAtMax = true;

	private float nextSpawn;
	private int nbTotalZombi = 0;

	void Update () {
		GameObject[] currentZombis = GameObject.FindGameObjectsWithTag ("Zombi");
		GatherZombis (currentZombis);

		bool isContinuing = true;
		if (nbTotalZombi >= nbMaxZombi && hasToStopAtMax) {
			isContinuing = false;
		}

		if (isActivated &&
			isContinuing &&
			currentZombis.Length < nbMaxZombi &&
			Time.time > nextSpawn) {
			if (PhotonNetwork.isMasterClient) {
				nextSpawn = Time.time + spawnRate;
				Spawn ();
			}
			nbTotalZombi++;
		}
	}

	private void GatherZombis(GameObject[] zombis){
		Transform zombisTransform = GameObject.Find ("Zombis").transform;

		foreach (GameObject zombi in zombis) {
			zombi.transform.SetParent (zombisTransform);
		}
	}

	protected void Spawn(){
		PhotonNetwork.InstantiateSceneObject(objectToSpawn.name, transform.position, Quaternion.identity, 0, null);
	}
}
