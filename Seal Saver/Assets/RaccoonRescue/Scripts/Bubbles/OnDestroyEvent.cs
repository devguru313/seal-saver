using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnDestroyEvent : MonoBehaviour {
	public GameObject[] instantiatePrefabs;

	void OnDisable () {
		if (!isQuitting) {
			foreach (GameObject item in instantiatePrefabs) {
				Instantiate (item, transform.position, transform.rotation);
			}
		}
	}

	bool isQuitting;

	void OnApplicationQuit () {
		isQuitting = true;
	}
}

