using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombiSpawner : MonoBehaviour {
	public float spawnRate = 2f;
	public GameObject objectToSpawn;

	private float nextSpawn;
	private bool isContinue = true;

	void Update () {
		if (isContinue && Time.time > nextSpawn) {
			if (PhotonNetwork.isMasterClient) {
				nextSpawn = Time.time + spawnRate;
				//PhotonNetwork.Instantiate(objectToSpawn.name, transform.position, Quaternion.identity, 0);
				//view.RPC ("Spawn", PhotonTargets.MasterClient);
				Spawn ();
			}
			isContinue = false;
		}
	}

	//[PunRPC]
	protected void Spawn(){
		//Instantiate(objectToSpawn, transform.position, Quaternion.identity);
		PhotonNetwork.InstantiateSceneObject(objectToSpawn.name, transform.position, Quaternion.identity, 0, null);
	}
}
