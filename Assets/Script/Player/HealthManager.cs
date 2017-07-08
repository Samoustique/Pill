using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour {
	public int life = 100;

	private UIPlayerManager uiPlayerManagerScript;
	private UIRoomManager uiRoomManagerScript;
	private bool isDead = false;
	private PhotonView view;

	void Start () {
		view = GetComponent<PhotonView> ();
		GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;

		uiRoomManagerScript = GameObject.Find ("CanvasRoom").GetComponentInChildren<UIRoomManager> ();

		if (view.isMine) {
			uiPlayerManagerScript = GameObject.Find ("CanvasPlayer").GetComponentInChildren<UIPlayerManager> ();
			uiPlayerManagerScript.UpdateLife (life);
		}
	}

	public void TakeDamage (int damage){
		if (view.isMine) {
			life -= damage;

			view.RPC ("UpdateRoomLife", PhotonTargets.OthersBuffered, PhotonNetwork.player.NickName, life);

			uiPlayerManagerScript.UpdateLife (life);

			if (life <= 0 && !isDead) {
				isDead = true;
				view.RPC ("DisablePlayer", PhotonTargets.AllBuffered, PhotonNetwork.player.NickName);
			}
		}
	}

	[PunRPC]
	protected void UpdateRoomLife(string player, int life){
		uiRoomManagerScript = GameObject.Find ("CanvasRoom").GetComponentInChildren<UIRoomManager> ();
		uiRoomManagerScript.UpdatePlayerLife (player, life);
	}

	[PunRPC]
	protected void DisablePlayer(string player){
		GameObject goPlayer = GameObject.Find (player);
		goPlayer.tag = "Dead";

		GameObject weapons = Utilities.FindGameObject("Weapons", goPlayer);
		if (weapons != null) {
			weapons.SetActive (false);
		}

		GameObject body = Utilities.FindGameObject("Body", goPlayer);
		if (body != null) {
			body.SetActive (false);
		}

		if (view.isMine) {
			GameObject.Find("CrossHair").SetActive(false);
			GameObject.Find("pnlAmmoLife").SetActive(false);
			GameObject.Find("BloodScreen").SetActive(false);
			Color newColor;
			ColorUtility.TryParseHtmlString ("#ACAAB2FF", out newColor);
			GameObject.Find("pnlRoom").GetComponent<Image>().color = newColor;

			goPlayer.GetComponentInChildren<UnityStandardAssets.ImageEffects.Bloom> ().enabled = true;
			goPlayer.GetComponentInChildren<UnityStandardAssets.ImageEffects.Fisheye> ().enabled = true;
			goPlayer.GetComponentInChildren<UnityStandardAssets.ImageEffects.GlobalFog> ().enabled = true;
			goPlayer.GetComponentInChildren<UnityStandardAssets.ImageEffects.Grayscale> ().enabled = true;
		}
	}

}
