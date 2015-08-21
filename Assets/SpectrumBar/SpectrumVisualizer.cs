using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpectrumVisualizer : MonoBehaviour
{
	public GameObject barPrefab;
	public GUIStyle labelStyle;
	
	SpectrumBar.BarType barType;
	int barCount;

	// Camera
	private Camera maincam;
	private Vector3 camPosition;

	void Awake () {
		Application.runInBackground = true;
		maincam = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera>();
	}

	void Start ()
	{

	}

	void Update ()
	{
		var spectrum = GetComponent<AudioSpectrum>();

		if (Input.GetKeyDown (KeyCode.Space)) {
			//Debug.Log("Updating camera position.");
			changeCamPosition();
		}
		
		if (barCount == spectrum.Levels.Length && !Input.GetMouseButtonDown(0)) {
			return;
		}
		
		// Change the bar type on mouse click.
		if (Input.GetMouseButtonDown(0)) {
			if (barType == SpectrumBar.BarType.MeanLevel) {
				barType = SpectrumBar.BarType.Realtime;
			} else {
				barType++;
			}
		}
		
		// Destroy the old bars.
		foreach (var child in transform) {
			Destroy ((child as Transform).gameObject);
		}
		
		// Change the number of bars.
		barCount = spectrum.Levels.Length;
		var barWidth = 6.0f / barCount;
		var barScale = new Vector3 (barWidth * 0.9f, 1, 0.75f);

		GameObject[] bars = GameObject.FindGameObjectsWithTag ("SpectrumBar");

		// Create new bars.
		//for (var i = 0; i < barCount; i++) {
		foreach (var bar in bars) {
			//var x = 6.0f * i / barCount - 3.0f + barWidth / 2;
			
			//var bar = Instantiate (barPrefab, Vector3.right * x, transform.rotation) as GameObject;
			
			var index = bar.GetComponent<SpectrumBar> ().index;

			bar.GetComponent<SpectrumBar> ().barType = barType;
			
			bar.transform.parent = transform;
			bar.transform.localScale = barScale;
		}
	}
	
	void OnGUI ()
	{
		var text = "Current mode: " + barType + "\n";
		text += "Click the screen to change the mode.";
		GUI.Label (new Rect(0, 0, Screen.width, Screen.height), text, labelStyle);
	}

	private void changeCamPosition() {
		Vector3 R = Random.onUnitSphere*100;
		R.y = Mathf.Abs (R.y);
		Vector3 center = new Vector3(0, 0, 100);
		Debug.Log (R);
		camPosition = center + R;
		//maincam.transform.position = camPosition;
		//maincam.transform.LookAt(center);
		//iTween.MoveTo(maincam.gameObject, camPosition, 1);
		//iTween.LookTo(maincam.gameObject, center, 1);
		iTween.MoveTo(maincam.gameObject, iTween.Hash("x", camPosition.x, "y", camPosition.y, "z", camPosition.z, "time", 0.6f, "looktarget", center, "easetype", "easeOutCubic"));
		//maincam.transform.LookAt(center);
		//iTween.LookTo(maincam.gameObject, center, 0.3f);
	}


}