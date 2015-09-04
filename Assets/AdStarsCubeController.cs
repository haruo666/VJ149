using UnityEngine;
using System.Collections;

public class AdStarsCubeController : MonoBehaviour {

	public GameObject cubePrefab;
	private Vector3 gravity;
	public int N = 300;

	// Camera
	private Camera maincam;
	private Vector3 camPosition;

	void Awake () {
		Application.runInBackground = true;
		maincam = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera>();
	}

	// Use this for initialization
	void Start () {
		//GameObject[] bars = GameObject.FindGameObjectsWithTag ("SpectrumBar")

		gravity = new Vector3 (0, 0f, 0);
		Physics.gravity = gravity;

		for (int i=0; i<N; i++) {
			var cube = Instantiate (cubePrefab, new Vector3 (Random.Range (-20,20), Random.Range (15, 40), -16), Quaternion.Euler(Random.Range(-180, 180), Random.Range(-180, 180), Random.Range(-180, 180))) as GameObject;
			var t = Random.Range (0.3f, 2.0f);
			cube.transform.localScale = new Vector3(t, t, t);
		}
		InvokeRepeating ("changeCamPosition", 5f, 5f);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space)) {
			//Debug.Log("Updating camera position.");
			changeCamPosition();
		}
	}

	void changeCamPosition() {
		//gravity = new Vector3 (0, -2.0f, 0);
		//Physics.gravity = gravity;
		Debug.Log ("Changed gravitation!");
		Vector3 R = Random.onUnitSphere*25;
		R.y = Mathf.Abs (R.y);
		Vector3 center = new Vector3(0, 25, 0);
		Debug.Log (R);
		camPosition = center + R;
		//iTween.MoveTo(maincam.gameObject, iTween.Hash("x", -25, "y", 25, "z", -30, "time", 0.6f, "looktarget", center, "easetype", "easeOutCubic"));
		iTween.MoveTo(maincam.gameObject, iTween.Hash("x", camPosition.x, "y", camPosition.y, "z", camPosition.z, "time", 3.0f, "looktarget", center, "easetype", "easeInOutCubic"));

	}
}
