using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRoomManager : UIManager {
	
	public Text txtRoom;
	public GameObject playerUIPrefab;
	public GameObject playersUISpawn;
	public GameObject playerPseudoPrefab;

	private GameObject otherPlayerPseudo;

	void Update(){
		DisplayOtherPlayerPseudo ();
	}

	public void UpdateRoom(string roomName){
		txtRoom.text = roomName;
	}

	public void UpdatePlayerLife(string player, int life){
		foreach (Transform child in playersUISpawn.transform) {
			foreach (Transform grandChild in child) {
				if (grandChild.name == "txtPlayer") {
					if(grandChild.gameObject.GetComponent<Text>().text == player){
						GameObject imgLifeBackground = grandChild.transform.parent.Find ("ImgLifeBackground").gameObject;
						UpdateBarLife (
							imgLifeBackground.GetComponent<Image>() as Image,
							imgLifeBackground.transform.Find("ImgLife").GetComponent<Image>() as Image,
							life);
						break;
					}
				}
			}
		}
	}

	public void UpdateListOfPlayers (List<string> players, string playerNickName){
		foreach(Transform child in playersUISpawn.transform){
			GameObject.Destroy (child.gameObject);
		}

		GameObject otherPlayerNode = GameObject.Find ("OtherPlayer");
		foreach(Transform child in otherPlayerNode.transform){
			GameObject.Destroy (child.gameObject);
		}

		Vector3 pos = playersUISpawn.transform.position;
		foreach (string name in players) {
			GameObject playerUI = Instantiate (playerUIPrefab, pos, Quaternion.identity, playersUISpawn.transform) as GameObject;

			GameObject txtPlayer = Utilities.FindGameObject ("txtPlayer", playerUI);
			if (txtPlayer != null) {
				Text pseudo = txtPlayer.GetComponent<Text> () as Text;
				pseudo.text = name;
				if (name == playerNickName) {
					pseudo.color = Color.white;
				}
			}

			GameObject imgLife = Utilities.FindGameObject ("ImgLifeBackground", playerUI);
			if (imgLife != null) {
				if (name == playerNickName) {
					imgLife.SetActive (false);
				} else {
					Vector3 outsideOfTheBox = new Vector3 (1000, 1000, 1000);
					otherPlayerPseudo = Instantiate (playerPseudoPrefab, outsideOfTheBox, Quaternion.identity) as GameObject;
					otherPlayerPseudo.transform.SetParent (otherPlayerNode.transform, false);
					otherPlayerPseudo.GetComponentInChildren<Text> ().text = name;

					UpdateBarLife (
						imgLife.GetComponent<Image>() as Image,
						imgLife.transform.Find("ImgLife").GetComponent<Image>() as Image,
						100);
				}
			}

			pos.y -= 15;
		}
	}

	private void DisplayOtherPlayerPseudo(){
		if (otherPlayerPseudo != null) {
			Camera cam = GameObject.Find (PhotonNetwork.player.NickName).GetComponentInChildren<Camera> () as Camera;
			string name = otherPlayerPseudo.GetComponentInChildren<Text> ().text;
			GameObject otherPlayer = GameObject.Find (name);
			if(otherPlayer != null){
				Vector3 heading = otherPlayer.transform.position - cam.transform.position;
				if (Vector3.Dot (cam.transform.forward, heading) > 0) {
					Vector3 pos = otherPlayer.transform.position;
					pos.y += 2;
					otherPlayerPseudo.transform.position = cam.WorldToScreenPoint (pos);
				}
			}
		}
	}
}
