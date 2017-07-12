using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanSpawner : MonoBehaviour {
	public float spawnRate = 2f;
	public float radius = 10f;
	public GameObject objectToSpawn;
	public int nbMaxHuman = 10;
	public bool isActivated = true;
	public bool hasToStopAtMax = true;

	private float nextSpawn;
	private int nbTotalHuman = 0;

	void Update () {
		GameObject[] currentHumans = GameObject.FindGameObjectsWithTag ("Human");
		GatherHumans (currentHumans);

		bool isContinuing = true;
		if (nbTotalHuman >= nbMaxHuman && hasToStopAtMax) {
			isContinuing = false;
		}

		if (isActivated &&
			isContinuing &&
			currentHumans.Length < nbMaxHuman &&
			Time.time > nextSpawn) {
			if (PhotonNetwork.isMasterClient) {
				nextSpawn = Time.time + spawnRate;
				Spawn ();
			}
			nbTotalHuman++;
		}
	}

	private void GatherHumans(GameObject[] humans){
		Transform humansTransform = GameObject.Find ("Humans").transform;
		foreach (GameObject human in humans) {
			human.transform.SetParent (humansTransform);
		}
	}

	protected void Spawn(){
		Vector3 circlePos = Random.insideUnitCircle * radius;
		Vector3 pos = new Vector3 (circlePos.x, 0, circlePos.y);
		pos *= radius;
		pos += transform.position;
		pos.y = 0;
		PhotonNetwork.InstantiateSceneObject(objectToSpawn.name, pos, Quaternion.identity, 0, null);
	}
}
