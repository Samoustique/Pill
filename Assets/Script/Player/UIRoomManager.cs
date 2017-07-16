using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class UIRoomManager : UIManager {
	public Text txtRoom;
	public GameObject playerUIPrefab;
	public GameObject playersUISpawn;
	public GameObject playerPseudoPrefab;
	public GameObject pnlMenuPrefab;

	private GameObject otherPlayerPseudo;
	private bool displayMenu = false;
	private GameObject menu;

	void Update(){
		DisplayOtherPlayerPseudo ();

		if (Input.GetKeyDown (KeyCode.Escape)) {
			displayMenu = !displayMenu;
		}

		if (displayMenu && menu == null) {
			int x = Screen.width / 2;
			int y = Screen.height / 2;
			Vector3 middleScreen = new Vector3 (x, y);
			menu = Instantiate (pnlMenuPrefab, middleScreen, Quaternion.identity, transform) as GameObject;
			Button btnLeave = Utilities.FindGameObject ("btnLeave", menu).GetComponent<Button>() as Button;
			btnLeave.onClick.AddListener (OnLeavRoomPressed);
			Button btnResume = Utilities.FindGameObject ("btnResume", menu).GetComponent<Button>() as Button;
			btnResume.onClick.AddListener (OnResumePressed);

			DisplayHideMenu (displayMenu);
		} else if (!displayMenu && menu != null) {
			Destroy (menu);
			DisplayHideMenu (displayMenu);
		}
	}

	private void DisplayHideMenu(bool isMenu){
		Cursor.visible = isMenu;
		GameObject canvasPlayer = GameObject.Find ("CanvasPlayer");
		foreach (Transform child in canvasPlayer.transform) {
			child.gameObject.SetActive (!isMenu);
		}
		GameObject player = GameObject.Find (PhotonNetwork.player.NickName);
		FirstPersonController fpc = player.GetComponentInChildren<FirstPersonController> () as FirstPersonController;
		if (fpc != null) {
			fpc.enabled = !isMenu;
		}
		Shoot shoot = player.GetComponentInChildren<Shoot> () as Shoot;
		if (shoot != null) {
			shoot.enabled = !isMenu;
		}
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

	private void OnResumePressed(){
		displayMenu = false;
	}

	private void OnLeavRoomPressed(){
		PhotonNetwork.LeaveRoom ();
	}
}
