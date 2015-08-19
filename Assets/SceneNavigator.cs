using UnityEngine;
using System.Collections;

public class SceneNavigator : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.C)) {
			Application.LoadLevel ("LiveCamera");
		} else if (Input.GetKeyDown (KeyCode.W)) {
			Application.LoadLevel ("AudioWave3DLines");
		} else if (Input.GetKeyDown (KeyCode.S)) {
			Application.LoadLevel("SunBurst");
		}

		if (Input.GetKeyDown (KeyCode.F)) {
			Screen.fullScreen = !Screen.fullScreen;
		}
	}
}
