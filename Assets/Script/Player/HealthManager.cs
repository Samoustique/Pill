using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour {

	public int life = 100;
	
	private UIManager uiManagerScript;

	void Start () {
		/*uiManagerScript = GameObject.Find("PanelUI").GetComponent<UIManager>();
		uiManagerScript.UpdateLife(life);*/
	}
	
	void Update () {
		if(Input.GetKeyDown(KeyCode.F)){
			life--;
			//uiManagerScript.UpdateLife(life);
		}
	}
}
