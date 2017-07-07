using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerManager : UIManager {
	
	public GameObject bloodScreen;
	public Image imgLifeBackground;
	public Image imgLife;
	//public Text txtLife;
	public Text txtTotalBullets;
	public GameObject goBullets;

	private CanvasGroup bloodCanvas;

	void Start(){
		bloodCanvas = bloodScreen.GetComponent<CanvasGroup> ();
	}

	public void UpdateTxtTotalBullet(int nbTotalBullet, int nbLoadedBullet, GameObject prefabBullet){
		txtTotalBullets.text = nbTotalBullet + "";
		// Destroy
		foreach (Transform child in goBullets.transform) {
			GameObject.Destroy (child.gameObject);
		}
		// Rebuild
		for (int i = 0; i < nbLoadedBullet; ++i) {
			/*Vector3 place = goBullets.transform.position;
			place.x += 10 * i;
			GameObject bullet = Instantiate(prefabBullet, place, Quaternion.identity) as GameObject;
			bullet.transform.SetParent(goBullets.transform, false);*/

			GameObject bullet = Instantiate(prefabBullet, Vector3.zero, Quaternion.identity) as GameObject;
			bullet.transform.SetParent(goBullets.transform, false);
			Vector3 place = goBullets.transform.position;

			RectTransform rectTransform = bullet.GetComponent<RectTransform> () as RectTransform;
			Canvas currentCanevas = gameObject.GetComponent<Canvas> () as Canvas;
			place.x += rectTransform.rect.width * currentCanevas.scaleFactor * i;
			bullet.transform.position = place;
		}
	}
	
	public void UpdateTxtCartridge (int nbCartridge, int nbMaxCartridge) {
		//txtCartridge.text = "AMMO " + nbCartridge + "/" + nbMaxCartridge;
	}
	
	public void UpdateLife (int life) {
		UpdateBarLife (imgLifeBackground, imgLife, life);
		//txtLife.text = life + "%";

		// blood screen
		if (life <= 0) {
			bloodCanvas.alpha = 1f;
		} else if (life >= 80) {
			bloodCanvas.alpha = 0f;
		} else if (life >= 60) {
			bloodCanvas.alpha = 0.2f;
		} else if (life >= 40) {
			bloodCanvas.alpha = 0.3f;
		} else if (life >= 20) {
			bloodCanvas.alpha = 0.5f;
		}
	}
}
