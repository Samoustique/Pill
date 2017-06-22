using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombiSpawner : MonoBehaviour {
	public float spawnRate = 2f;
	public GameObject objectToSpawn;
	public int nbMaxZombi = 10;
	public bool isActivated = true;
	public bool isSpawningOnlyOne = false;

	private float nextSpawn;
	private bool isContinuing = true;

	void Update () {
		GameObject[] zombis = GameObject.FindGameObjectsWithTag ("Zombi");
		GatherZombis (zombis);

		if (isActivated && isContinuing && zombis.Length < nbMaxZombi && Time.time > nextSpawn) {
			if (PhotonNetwork.isMasterClient) {
				nextSpawn = Time.time + spawnRate;
				Spawn ();
			}
			isContinuing = !isSpawningOnlyOne;
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
