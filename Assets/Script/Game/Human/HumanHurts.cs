using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanHurts : MonoBehaviour {
	public AudioClip soundAttack;

	private AudioSource audioPunctualSource;
	private HashSet<GameObject> touchingZombis;

	void Start(){
		audioPunctualSource = GetComponent<AudioSource> ();
		touchingZombis = new HashSet<GameObject>();
	}

	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "Zombi" ||
			other.gameObject.tag == "ZombiPart") {
			touchingZombis.Add (other.gameObject);
		}
	}

	void OnTriggerExit(Collider other){
		if ((other.gameObject.tag == "Zombi" ||
		    other.gameObject.tag == "ZombiPart") &&
		    touchingZombis.Contains (other.gameObject)) {
			touchingZombis.Remove (other.gameObject);
		}
	}

	public void NotifyIsHitting(int damage){
		if (touchingZombis.Count > 0) {
			audioPunctualSource.PlayOneShot (soundAttack);

			foreach (GameObject go in touchingZombis) {
				GameObject goParent = go;
				while (goParent != null && goParent.tag != "Zombi") {
					goParent = goParent.transform.parent.gameObject;
				}

				ZombiAI zombiAIScript = goParent.GetComponentInChildren<ZombiAI> ();
				if (zombiAIScript != null) {
					zombiAIScript.TakeDamage (go, Vector3.forward, damage);
					touchingZombis.Clear ();
				}
				break;
			}
		}
	}
}
