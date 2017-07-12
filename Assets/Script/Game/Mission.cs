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
