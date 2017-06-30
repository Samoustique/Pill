using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PM40 : Shoot {
	public Animator animFlame;
	public GameObject prefabDartIcon;
	public GameObject prefabDart;
	public float range;
	public float sleepingTime = 10f;

	private Animator animWeapon;

	protected override void StartChild (){
		animWeapon = GetComponent<Animator> ();
	}

	protected override void ReloadChild (){
		animWeapon.SetTrigger ("reload");
	}

	protected override void FireChild (){
		//animFlame.SetTrigger("flame");
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
		return prefabDartIcon;
	}

	protected override float GetRange (){
		return range;
	}

	protected override void CreateBulletImpact (string injuredPart, int viewId, Vector3 point, Vector3 direction){
		GameObject dart = Instantiate (prefabDart, point, Quaternion.FromToRotation (Vector3.forward, direction)) as GameObject;
		PhotonView view = PhotonView.Find (viewId) as PhotonView;

		GameObject target = FindGameObject (injuredPart, view.gameObject);
		if (target != null) {
			dart.transform.parent = target.transform;
		} else {
			dart.transform.parent = view.gameObject.transform;
		}
	}

	private GameObject FindGameObject(string objectToFind, GameObject source){
		if (source.transform.name == objectToFind) {
			return source;
		}
		foreach (Transform child in source.transform) {
			GameObject target = FindGameObject (objectToFind, child.gameObject);
			if (target != null) {
				return target;
			}
		}
		return null;
	}

	protected override void LaunchBulletOnPropsChild (Vector3 point, Vector3 direction){
		GameObject dart = Instantiate(prefabDart, point, Quaternion.FromToRotation(Vector3.forward, -direction)) as GameObject;
		Destroy (dart, 10f);
	}

	protected override void ApplyEffectOnMob (MobAI aiScript, GameObject gameObjectHit, Vector3 direction, int damage){
		aiScript.GetSleepy (gameObjectHit, direction, sleepingTime);
	}
}
