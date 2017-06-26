using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PM40 : Shoot {

	public Animator animFlame;
	public GameObject prefabBullet;
	public float range;

	private Animator animWeapon;

	protected override void StartChild (){
		animWeapon = GetComponent<Animator> ();
	}

	protected override void ReloadChild (){
		animWeapon.SetTrigger ("reload");
	}

	protected override void FireChild (){
		animFlame.SetTrigger("flame");
		animWeapon.SetTrigger ("shoot");
	}

	protected override void FireButEmptyChild (){
		animWeapon.SetTrigger ("shoot");
	}
		
	protected override void UpdateChild () {			
		if (Input.GetKey (KeyCode.LeftShift)) {
			if (Input.GetAxis ("Vertical") == 0) {
				animWeapon.SetBool ("run", false);
			} else {
				animWeapon.SetBool ("run", true);
			}
		} else {
			animWeapon.SetBool ("run", false);

			if (Input.GetAxis ("Vertical") == 0) {
				animWeapon.SetBool ("walk", false);
			} else {
				animWeapon.SetBool ("walk", true);
			}
		}
	}

	protected override GameObject GetPrefabBullet (){
		return prefabBullet;
	}

	protected override float GetRange (){
		return range;
	}
}
