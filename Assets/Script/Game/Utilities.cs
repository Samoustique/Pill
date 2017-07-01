using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Utilities : MonoBehaviour {
	public static GameObject FindGameObject(string objectToFind, GameObject source){
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
}