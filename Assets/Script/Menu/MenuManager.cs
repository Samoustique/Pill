using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {
	public GameObject roomA;
	public GameObject roomB;
	public GameObject roomC;
	public Button btnPlay;
	public InputField inPseudo;
	public GameObject lightSelectedHero;
	public GameObject selectedRoom;
	public string sceneToLoad;

	private string selectedHero = "";
	private byte nbMaxPlayers = 2;

	void Start(){
		btnPlay.interactable = false;

		if (!PhotonNetwork.connected) {
			PhotonNetwork.ConnectUsingSettings ("version1");
		}
		OnReceivedRoomListUpdate ();
	}

	public void SelectRoom(GameObject room){
		selectedRoom = room;

		if (room.transform.name != roomA.name) {
			roomA.GetComponent<BtnRoom> ().BackToNormal ();
		}
		if (room.transform.name != roomB.name) {
			roomB.GetComponent<BtnRoom> ().BackToNormal ();
		}
		if (room.transform.name != roomC.name) {
			roomC.GetComponent<BtnRoom> ().BackToNormal ();
		}

		CheckIsPlayable ();
	}

	public void OnPseudoChanged(){
		CheckIsPlayable ();
	}

	public void SelectHero (Transform hero){
		lightSelectedHero.GetComponent<Light>().enabled = true;
		lightSelectedHero.transform.LookAt (hero);
		selectedHero = hero.name;
		CheckIsPlayable ();
	}

	private void CheckIsPlayable(){
		if (selectedRoom != null && selectedHero != "" && inPseudo.text.Length > 0) {
			btnPlay.interactable = true;
		} else {
			btnPlay.interactable = false;
		}
	}

	public void OnPlayPressed(){
		PhotonNetwork.playerName = inPseudo.text;
		RoomOptions roomOptions = new RoomOptions ();
		roomOptions.MaxPlayers = nbMaxPlayers;
		roomOptions.IsVisible = true;
		PlayerPrefs.SetString ("Hero", selectedHero);
		PhotonNetwork.JoinOrCreateRoom (selectedRoom.name, roomOptions, TypedLobby.Default);
	}

	void OnJoinedRoom(){
		PhotonNetwork.LoadLevel (sceneToLoad);
	}
		
	void OnJoinedLobby(){
	}

	void OnJoinedRoomFailed(){
		Debug.Log ("ERROR");
	}

	void OnPhotonJoinRoomFailed(){
		Debug.Log ("ERROR 2");
	}

	void OnReceivedRoomListUpdate(){
		bool needToFreeRoomA = true;
		bool needToFreeRoomB = true;
		bool needToFreeRoomC = true;

		foreach (RoomInfo room in PhotonNetwork.GetRoomList()) {
			if (room.Name == roomA.name) {
				UpdateRoomDisplay (roomA, room.PlayerCount, room.MaxPlayers);
				needToFreeRoomA = false;
			} else if (room.Name == roomB.name) {
				UpdateRoomDisplay (roomB, room.PlayerCount, room.MaxPlayers);
				needToFreeRoomB = false;
			} else if (room.Name == roomB.name) {
				UpdateRoomDisplay (roomB, room.PlayerCount, room.MaxPlayers);
				needToFreeRoomC = false;
			}
		}

		if (needToFreeRoomA) {
			UpdateRoomDisplay (roomA, 0, nbMaxPlayers);
		}
		if (needToFreeRoomB) {
			UpdateRoomDisplay (roomB, 0, nbMaxPlayers);
		}
		if (needToFreeRoomC) {
			UpdateRoomDisplay (roomC, 0, nbMaxPlayers);
		}
	}

	private void UpdateRoomDisplay(GameObject room, int playerCount, int maxPlayers){
		foreach (Transform child in room.transform){
			if (child.name == "txtRoomNbPlayers"){
				child.gameObject.GetComponent<Text>().text = playerCount + "/" + maxPlayers;
			}
		}

		bool isRoomFull = playerCount == maxPlayers;
		room.GetComponent<Button> ().interactable = !isRoomFull;
		BtnRoom btnRoom = room.GetComponent<BtnRoom> ();
		if (isRoomFull) {
			btnRoom.Disable ();
			if (room == selectedRoom) {
				selectedRoom = null;
				CheckIsPlayable ();
			}
		} else {
			btnRoom.NotFull ();
		}
	}
}
