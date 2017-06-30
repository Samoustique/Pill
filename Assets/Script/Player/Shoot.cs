using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Shoot : MonoBehaviour {

	public AudioSource audioSource;
	public AudioClip soundShoot;
	public AudioClip soundReload;
	public AudioClip soundEmpty;
	public float shootRate = 1f;
	public int nbMagazineCapacity;
	public int nbLoadedBullet = -1;
	public int nbTotalBullet;
	public int damage = 0;
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
	protected abstract void LaunchBulletOnPropsChild (Vector3 point, Vector3 direction);
	protected abstract GameObject GetPrefabBullet ();
	protected abstract float GetRange ();
	protected abstract void ApplyEffectOnMob (MobAI aiScript, GameObject gameObjectHit, Vector3 direction, int damage);
	protected abstract void CreateBulletImpact (string injuredPart, int viewId, Vector3 point, Vector3 direction);
	
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

		if(Physics.Raycast(ray, out hit, GetRange())){
			// Something has been hit
			string tag = hit.transform.gameObject.tag;
			switch (tag) {
			case "Zombi":
			case "Human":
					MobAI aiScript = hit.transform.gameObject.GetComponent<MobAI> () as MobAI;
					PhotonView targetView = hit.transform.gameObject.GetComponent<PhotonView> () as PhotonView;
					ApplyEffectOnMob (aiScript, hit.transform.gameObject, ray.direction, damage);
					view.RPC ("LaunchBulletOnTarget", PhotonTargets.AllBuffered, PhotonNetwork.player.NickName, hit.transform.name, targetView.viewID, hit.point, hit.normal);
					break;
				case "ZombiPart":
				case "HumanPart":
					GameObject goParent = hit.transform.parent.gameObject;
					while (goParent != null && goParent.tag != "Human" && goParent.tag != "Zombi") {
						goParent = goParent.transform.parent.gameObject;
					}
					aiScript = goParent.GetComponent<MobAI>() as MobAI;
					targetView = goParent.GetComponent<PhotonView> () as PhotonView;
					if (aiScript != null) {
						ApplyEffectOnMob (aiScript, hit.transform.gameObject, ray.direction, damage);
						view.RPC ("LaunchBulletOnTarget", PhotonTargets.AllBuffered, PhotonNetwork.player.NickName, hit.transform.name, targetView.viewID, hit.point, hit.normal);
					}
					break;
				case "Props":
				    view.RPC ("LaunchBulletOnProps", PhotonTargets.AllBuffered, hit.point, ray.direction);
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

	[PunRPC]
	protected void LaunchBulletOnProps(Vector3 point, Vector3 direction){
		LaunchBulletOnPropsChild (point, direction);
	}

	[PunRPC]
	public void LaunchBulletOnTarget(string playerName, string injuredPart, int viewId, Vector3 point, Vector3 direction){
		GameObject player = GameObject.Find (playerName);

		if (player != null) {
			Shoot shootScript = player.GetComponentInChildren<Shoot> () as Shoot;
			if (shootScript != null) {
				shootScript.CreateBulletImpact (injuredPart, viewId, point, direction);
			}
		}
	}
}
