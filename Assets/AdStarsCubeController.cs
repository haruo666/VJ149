using UnityEngine;
using System.Collections;

public class AdStarsCubeController : MonoBehaviour {

	public GameObject cubePrefab;

	// Use this for initialization
	void Start () {
		//GameObject[] bars = GameObject.FindGameObjectsWithTag ("SpectrumBar")
		Physics.gravity = new Vector3(0, -0.1F, 0);

		for (int i=0; i<50; i++) {
			var cube = Instantiate (cubePrefab, new Vector3 (Random.Range (-10,10), Random.Range (1,50), -10), new Quaternion (0, 0, 0, 0)) as GameObject;
		}
	}
	
	// Update is called once per frame
	void Update () {

	}
}
