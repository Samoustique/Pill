using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

	public Image imgLifeBackground;
	public Image imgLife;
	public Sprite sprLightGreen;
	public Sprite sprGreen;
	public Sprite sprLightRed;
	public Sprite sprRed;
	//public Text txtLife;
	public Text txtTotalBullets;
	public GameObject goBullets;

	public void UpdateTxtTotalBullet(int nbTotalBullet, int nbLoadedBullet, GameObject prefabBullet){
		txtTotalBullets.text = nbTotalBullet + "";
		// Destroy
		foreach (Transform child in goBullets.transform) {
			GameObject.Destroy (child.gameObject);
			//Destroy (child);
		}
		// Rebuild
		for (int i = 0; i < nbLoadedBullet; ++i) {
			Vector3 place = goBullets.transform.position;
			place.x += 10 * i;
			GameObject bullet = Instantiate(prefabBullet, place, Quaternion.identity) as GameObject;
			bullet.transform.SetParent(goBullets.transform);
		}
	}
	
	public void UpdateTxtCartridge (int nbCartridge, int nbMaxCartridge) {
		//txtCartridge.text = "AMMO " + nbCartridge + "/" + nbMaxCartridge;
	}
	
	public void UpdateLife (int life) {
		life = Mathf.Clamp(life, 0, 100);
		if (life > 30) {
			imgLifeBackground.sprite = sprLightGreen;
			imgLife.sprite = sprGreen;
		} else {
			imgLifeBackground.sprite = sprLightRed;
			imgLife.sprite = sprRed;
		}
		imgLife.fillAmount = (float) life / 100;
		//txtLife.text = life + "%";
	}
}
