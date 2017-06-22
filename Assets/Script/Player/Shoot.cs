using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Shoot : MonoBehaviour {

	public AudioSource audioSource;
	public AudioClip soundShoot;
	public AudioClip soundReload;
	public AudioClip soundEmpty;
	public GameObject prefabBulletHole;
	public GameObject prefabSparks;
	public float shootRate = 1f;
	public int nbMagazineCapacity;
	public int nbLoadedBullet = -1;
	public int nbTotalBullet;
	public int damage = 100;
	public bool isAutomatic = true;
	
	private Ray ray;
	private RaycastHit hit;
	private float nextFire;
	private UIPlayerManager uiPlayerManagerScript;
	private PhotonView view;
	
	void Start(){
		view = GetComponent<PhotonView> ();

		if (view.isMine) {
			nbLoadedBullet = nbMagazineCapacity;
			nbTotalBullet -= nbMagazineCapacity;
			uiPlayerManagerScript = GameObject.Find ("CanvasPlayer").GetComponent<UIPlayerManager> ();
			uiPlayerManagerScript.UpdateTxtTotalBullet (nbTotalBullet, nbLoadedBullet, GetPrefabBullet ());
			StartChild ();
		}
	}

	protected abstract void StartChild ();
	protected abstract void ReloadChild ();
	protected abstract void FireChild ();
	protected abstract void FireButEmptyChild ();
	protected abstract void UpdateChild ();
	protected abstract GameObject GetPrefabBullet ();
	
	void OnEnable(){
		if(uiPlayerManagerScript != null){
			uiPlayerManagerScript.UpdateTxtTotalBullet(nbTotalBullet, nbLoadedBullet, GetPrefabBullet());
		}
	}
	
	void Update () {
		if (view.isMine) {
			if (Time.time > nextFire) {
				if ((isAutomatic && Input.GetButton ("Fire1") ||
				   !isAutomatic && Input.GetButtonDown ("Fire1")) &&
				   nbLoadedBullet > 0) {
					Fire ();
				} else if (Input.GetButton ("Fire1") && nbLoadedBullet == 0) {
					FireButEmpty ();
				}
			}
		
			if (Input.GetKeyDown (KeyCode.R) &&
			  nbTotalBullet > 0 &&
			  nbLoadedBullet < nbMagazineCapacity) {
				StartCoroutine (Reload ());
			}

			UpdateChild ();
		}
	}

	[PunRPC]
	protected void ShootOnProps(Vector3 point, Vector3 normal){
		// bullet hole
		GameObject bulletHole = Instantiate(prefabBulletHole, point, Quaternion.FromToRotation(Vector3.forward, normal)) as GameObject;
		Destroy(bulletHole, 20f);
		
		// sparks
		GameObject sparks = Instantiate(prefabSparks, point, Quaternion.FromToRotation(Vector3.forward, normal)) as GameObject;
		Destroy(sparks, 3f);
	}

	[PunRPC]
	protected void SoundShoot(){
		audioSource.PlayOneShot(soundShoot);
	}

	[PunRPC]
	protected void SoundEmpty(){
		audioSource.PlayOneShot(soundEmpty);
	}

	[PunRPC]
	protected void SoundReload(){
		audioSource.PlayOneShot(soundReload);
	}
	
	private void Fire(){
		nextFire = Time.time + shootRate;
		view.RPC ("SoundShoot", PhotonTargets.All);

		nbLoadedBullet--;
		uiPlayerManagerScript.UpdateTxtTotalBullet(nbTotalBullet, nbLoadedBullet, GetPrefabBullet());
		FireChild();

		Vector2 screenCenterPoint = new Vector2(Screen.width / 2, Screen.height / 2);
		ray = Camera.main.ScreenPointToRay(screenCenterPoint);

		if(Physics.Raycast(ray, out hit, Camera.main.farClipPlane)){
			// Something has been hit
			string tag = hit.transform.gameObject.tag;
			switch (tag) {
				case "Zombi":
				case "Human":
					MobAI aiScript = hit.transform.gameObject.GetComponent<MobAI>() as MobAI;
					aiScript.IsHurt (hit.transform.gameObject, ray.direction, damage);
					break;
				case "ZombiPart":
				case "HumanPart":
					GameObject goParent = hit.transform.parent.gameObject;
					while (goParent != null && goParent.tag != "Human" && goParent.tag != "Zombi") {
						goParent = goParent.transform.parent.gameObject;
					}
					aiScript = goParent.GetComponent<MobAI>() as MobAI;
					if (aiScript != null) {
						aiScript.IsHurt (hit.transform.gameObject, ray.direction, damage);
					}
					break;
				case "Props":
					view.RPC ("ShootOnProps", PhotonTargets.All, hit.point, hit.normal);
					break;
			}
		}
	}
	
	private void FireButEmpty(){
		nextFire = Time.time + shootRate;
		view.RPC ("SoundEmpty", PhotonTargets.All);
		FireButEmptyChild ();
	}
	
	private IEnumerator Reload(){
		view.RPC ("SoundReload", PhotonTargets.All);
		ReloadChild ();
		yield return new WaitForSeconds(0.2f);

		int missingBullet = nbMagazineCapacity - nbLoadedBullet;

		if (nbTotalBullet >= missingBullet) {
			nbLoadedBullet = nbMagazineCapacity;
			nbTotalBullet -= missingBullet;
		} else {
			nbLoadedBullet += nbTotalBullet;
			nbTotalBullet = 0;
		}
		uiPlayerManagerScript.UpdateTxtTotalBullet(nbTotalBullet, nbLoadedBullet, GetPrefabBullet());
	}

	public void AddMagazine(){
		nbTotalBullet += nbMagazineCapacity;
		uiPlayerManagerScript.UpdateTxtTotalBullet(nbTotalBullet, nbLoadedBullet, GetPrefabBullet());
	}
}
