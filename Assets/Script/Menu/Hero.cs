using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour {
	public GameObject spotLight;
	public GameObject body;

	private MenuManager managerScript;
	private float rotationSpeed;

	void Start () {
		managerScript = GameObject.Find("CanvasMenu").GetComponent<MenuManager> () as MenuManager;
		rotationSpeed = Random.Range (1.5f, 3.0f);
	}

	void Update () {
		body.transform.Rotate (Vector3.up * rotationSpeed);
	}

	void OnMouseDown(){
		managerScript.SelectHero (transform);
	}

	void OnMouseEnter(){
		spotLight.SetActive(true);
	}

	void OnMouseExit(){
		spotLight.SetActive(false);
	}
}
