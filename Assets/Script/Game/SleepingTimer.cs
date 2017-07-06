using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SleepingTimer : MonoBehaviour {
	private float sleepingTimer = 0f;
	private float maxSleepingTime = -1f;
	private Image image;
	private float startingDistance;
	private Vector3 startingScale;
	private Vector3 mobPosition;

	void Start () {
		image = GetComponent<Image> () as Image;
		startingDistance = Vector3.Distance (Camera.main.transform.position, transform.position);
		startingScale = transform.localScale;
	}

	void Update () {
		if (maxSleepingTime > -1) {
			sleepingTimer -= Time.deltaTime;
			image.fillAmount = sleepingTimer / maxSleepingTime;
			float currentDistance = Vector3.Distance (Camera.main.transform.position, mobPosition);
			if (currentDistance > 20f) {
				Vector3 newScale = new Vector3 (
					1 -  ((startingScale.x * currentDistance) / 100),
					1 -  ((startingScale.y * currentDistance) / 100),
					1 -  ((startingScale.z * currentDistance) / 100));
				transform.localScale = newScale;
			}
		}

		if (sleepingTimer <= 0f) {
			Destroy (gameObject);
		}
	}

	public void UpdateTimer(float sleepingTime){
		float timer = sleepingTimer + sleepingTime;
		if (timer > maxSleepingTime) {
			sleepingTimer = maxSleepingTime;
		} else {
			sleepingTimer = timer;
		}
	}

	public void SetMaxTimer(float time){
		maxSleepingTime = time;
	}

	public void SetMobPosition(Vector3 position){
		mobPosition = position;
	}
}
