using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mission : MonoBehaviour {
	public float missionTiming; // in seconds
	public GameObject goTxtTimer;

	private float gameTimeLeft = -1;
	private Text txtTimer;
	private PhotonView view;

	void Start () {
		view = GetComponent<PhotonView> () as PhotonView;
		txtTimer = goTxtTimer.GetComponent<Text> () as Text;

		if (PhotonNetwork.isMasterClient) {
			// start the game timer
			gameTimeLeft = missionTiming;
		}
	}

	void Update () {
		if (gameTimeLeft > 0) {
			txtTimer.text = string.Format ("{0:00}:{1:00}", 
				Mathf.Floor ((gameTimeLeft) / 60),//minutes
				Mathf.Floor ((gameTimeLeft) % 60));//seconds
			gameTimeLeft -= Time.deltaTime;
		}
	}

	void OnPhotonPlayerConnected(){ // a player has just joined the room
		if (PhotonNetwork.isMasterClient) {
			view.RPC ("SetGameTimeLeft", PhotonTargets.Others, gameTimeLeft);
		}
	}

	[PunRPC]
	protected void SetGameTimeLeft(float timeLeft){
		gameTimeLeft = timeLeft;
	}
}


/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mission : MonoBehaviour {
	public GameObject goTxtTimer;
	public int[] nbZombiToSpawn;
	public float timeBetweenWaves;
	public GameObject zombiPrefab;

	private float timerStarter = 10; // in seconds
	private float gameTimeLeft = -1;
	private Text txtTimer;
	private PhotonView view;
	private int iZombiToSpawn = 0;
	private bool hasTimerEnded = false;
	private Transform[] zombiSpawnPoints;

	void Start () {
		view = GetComponent<PhotonView> () as PhotonView;
		txtTimer = goTxtTimer.GetComponent<Text> () as Text;
		zombiSpawnPoints = GameObject.Find ("SpawnZombiPoints").GetComponentsInChildren<Transform>();

		if (PhotonNetwork.isMasterClient) {
			// start the game timer
			gameTimeLeft = timeBetweenWaves;
		}
	}

	void Update () {
		GameObject[] currentZombis = GameObject.FindGameObjectsWithTag ("Zombi");
		GatherZombis (currentZombis);
		Debug.Log ("gameTimeLeft : " + gameTimeLeft + " timerStarter " + timerStarter);
		if (gameTimeLeft <= timerStarter && gameTimeLeft > 0) {
			goTxtTimer.SetActive (true);
			txtTimer.text = string.Format ("{0:00}:{1:00}", 
				Mathf.Floor ((gameTimeLeft) / 60),//minutes
				Mathf.Floor ((gameTimeLeft) % 60));//seconds
		} else if(gameTimeLeft <= 0){
			hasTimerEnded = true;
		}

		if (hasTimerEnded) {
			if (PhotonNetwork.isMasterClient) {
				if (iZombiToSpawn < nbZombiToSpawn.Length){
					for(int i = nbZombiToSpawn[iZombiToSpawn] ; i > 0 ; --i){
						SpawnZombi (Random.Range (0, zombiSpawnPoints.Length));
					}
				}
				view.RPC ("SetGameTimeLeft", PhotonTargets.All, timeBetweenWaves);
			}

			goTxtTimer.SetActive (false);
			hasTimerEnded = false;
		}
		gameTimeLeft -= Time.deltaTime;
	}

	void OnPhotonPlayerConnected(){ // a player has just joined the room
		if (PhotonNetwork.isMasterClient) {
			view.RPC ("SetGameTimeLeft", PhotonTargets.Others, gameTimeLeft);
		}
	}

	private void GatherZombis(GameObject[] zombis){
		Transform zombisTransform = GameObject.Find ("Zombis").transform;

		foreach (GameObject zombi in zombis) {
			zombi.transform.SetParent (zombisTransform);
		}
	}

	protected void SpawnZombi(int iZombiSpawnPoint){
		if (iZombiSpawnPoint < zombiSpawnPoints.Length) {
			PhotonNetwork.InstantiateSceneObject (zombiPrefab.name, zombiSpawnPoints[iZombiSpawnPoint].transform.position, Quaternion.identity, 0, null);
		}
	}

	[PunRPC]
	protected void SetGameTimeLeft(float timeLeft){
		gameTimeLeft = timeLeft;
	}
}
*/