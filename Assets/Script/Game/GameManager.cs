using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	public Text txtRoom;
	public Text txtPlayerList;
	public GameObject prefabPlayer;
	public GameObject spawnPoint;

	void Start () {
		// This part is only played for MY character
		txtRoom.text = PhotonNetwork.room.Name;

		//Spawnpoint
		Vector3 spawnPointPosition = new Vector3(
			spawnPoint.transform.position.x + Random.Range(-4f, 4f),
			spawnPoint.transform.position.y +
			spawnPoint.transform.position.z);
		spawnPointPosition += spawnPoint.transform.position;

		GameObject player = PhotonNetwork.Instantiate
			(prefabPlayer.name, spawnPointPosition, Quaternion.identity, 0);

		// Make some common changes (to all players, including me)
		PhotonView playerView = player.GetComponent<PhotonView> ();
		playerView.RPC ("SetPlayerName", PhotonTargets.AllBuffered, PhotonNetwork.player.NickName);
		playerView.RPC ("ActivateSkin", PhotonTargets.AllBuffered, PlayerPrefs.GetString ("Hero"));

		((MonoBehaviour)player.GetComponent("FirstPersonController")).enabled = true;

		// audioListener
		player.GetComponentInChildren<AudioListener>().enabled = true;

		// cameras : activate them both
		Camera[] cams = player.GetComponentsInChildren<Camera> ();
		foreach (Camera cam in cams) {
			cam.enabled = true;
		}

		// Layers
		foreach (Transform child in player.transform) {
			if (child.name == "Body") {
				SetLayerRecursively(child.gameObject, LayerMask.NameToLayer ("Skin"));
			}
			if (child.name == "Weapons") {
				SetLayerRecursively(child.gameObject, LayerMask.NameToLayer ("FPS"));
			}
		}

		UpdateListOfPlayers ();
	}

	private void SetLayerRecursively(GameObject obj, int layerNumber){
		obj.layer = layerNumber;
		foreach (Transform child in obj.transform) {
			SetLayerRecursively(child.gameObject, layerNumber);
		}
	}

	public void UpdateListOfPlayers () {
		txtPlayerList.text = null;
		foreach (PhotonPlayer player in PhotonNetwork.playerList) {
			txtPlayerList.text += player.NickName + "\n";	
		}
	}

	public void BackToLogin(){
		PhotonNetwork.LeaveRoom ();
	}

	void OnJoinedRoom(){
		Debug.Log ("OnJoinedRoom");
	}

	void OnLeftRoom(){
		PhotonNetwork.LoadLevel ("login");
	}

	void OnPhotonPlayerConnected(){ // a player has just joined the room
		Debug.Log("OnPhotonPlayerConnected"); 
	}

	void OnPhotonPlayerDisconnected(){
		UpdateListOfPlayers ();
	}

	void OnMasterClientSwitched(PhotonPlayer newMasterClient){
		Debug.Log("new master client : " + newMasterClient.NickName);
	}
}
