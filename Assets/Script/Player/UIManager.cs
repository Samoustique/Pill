using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

	public Sprite sprLightGreen;
	public Sprite sprGreen;
	public Sprite sprLightRed;
	public Sprite sprRed;

	protected void UpdateBarLife (Image imgLifeBackground, Image imgLife, int life) {
		life = Mathf.Clamp(life, 0, 100);

		if (life > 30) {
			imgLifeBackground.sprite = sprLightGreen;
			imgLife.sprite = sprGreen;
		} else {
			imgLifeBackground.sprite = sprLightRed;
			imgLife.sprite = sprRed;
		}
		imgLife.fillAmount = (float) life / 100;
	}
}
