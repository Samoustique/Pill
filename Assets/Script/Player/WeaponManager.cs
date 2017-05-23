using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour {

	public GameObject[] weapons;
	public int current = 0;
	public float delay = 0.4f;
	
	void Start () {
		for(int i = 0 ; i < weapons.Length ; ++i){
			weapons[i].SetActive(i == current);
		}
	}
	
	void Update () {
		if(Input.GetKeyDown(KeyCode.Alpha1)){
			if(current != 0){
				StartCoroutine(Transition(weapons[current], weapons[0]));
				current = 0;
			}
		}
		
		if(Input.GetKeyDown(KeyCode.Alpha2)){
			if(current != 1){
				StartCoroutine(Transition(weapons[current], weapons[1]));
				current = 1;
			}
		}
	}
	
	IEnumerator Transition(GameObject oldWeapon, GameObject newWeapon){
		oldWeapon.GetComponent<Animator>().SetTrigger("out");
		yield return new WaitForSeconds(delay);
		oldWeapon.SetActive(false);
		newWeapon.SetActive(true);
		newWeapon.GetComponent<Animator>().SetTrigger("in");
	}
	
	
}
