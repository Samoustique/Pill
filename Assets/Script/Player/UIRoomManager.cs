using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRoomManager : UIManager {
	
	public Text txtRoom;
	public GameObject playerUIPrefab;
	public GameObject playersUISpawn;

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

		Vector3 pos = playersUISpawn.transform.position;
		foreach (string name in players) {
			GameObject playerUI = Instantiate (playerUIPrefab, pos, Quaternion.identity, playersUISpawn.transform) as GameObject;
			foreach (Transform child in playerUI.transform) {
				if (child.name == "txtPlayer") {
					Text pseudo = child.gameObject.GetComponent<Text> () as Text;
					pseudo.text = name;
					if (name == playerNickName) {
						pseudo.color = Color.white;
					}
				} else if (child.name == "ImgLifeBackground") {
					if (name == playerNickName) {
						child.gameObject.SetActive (false);
					} else {
						UpdateBarLife (
							child.gameObject.GetComponent<Image>() as Image,
							child.transform.Find("ImgLife").GetComponent<Image>() as Image,
							100);
					}
				}
			}
			pos.y -= 15;
		}
	}
}
