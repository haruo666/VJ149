using UnityEngine;
using System.Collections;

[RequireComponent (typeof (AudioSource))]
public class AudioData : MonoBehaviour {

	private AudioSource audio;
	AudioReceiver audioreceiver;
	GameObject cube1, cube2;
	Color[] colors;
	public float threshold = 1.0f;

	public LineRenderer lr;
	// Use this for initialization
	void Start () {
		audio = GetComponent<AudioSource>();
		audioreceiver = GetComponent<AudioReceiver>(); // Reference the audioreceiver script.
		Debug.Log (audioreceiver.teststr);

		lr = GetComponent<LineRenderer> ();

		cube1 = GameObject.Find ("Cube1");
		cube2 = GameObject.Find ("Cube2");

		colors = new Color[5];
		colors [0] = new Color (122/255, 188/255, 204/255, 1);
		colors [1] = new Color (125/255, 147/255, 153/255, 1);
		colors [2] = new Color (106/255, 255/255, 203/255, 1);
		colors [3] = new Color (255/255, 179/255, 176/255, 1);
		colors [4] = new Color (204/255, 85/255, 127/255, 1);

	}
	
	// Update is called once per frame
	void Update () {
		/*audio.GetOutputData(waveData_, 1);
		Debug.Log (waveData_);

		var volume = waveData_.Select(x => x*x).Sum() / waveData_.Length;
		transform.localScale = Vector3.one * volume;

		Debug.Log (volume);*/
		
		//float volume = GetAveragedVolume ();
		//Debug.Log (audioreceiver.loudness);
		//cube.transform.position = new Vector3 (0, audioreceiver.loudness * 5, 0);
		//cube.transform.Rotate(Vector3.up * Time.deltaTime*20, Space.World);
		//cube.transform.Rotate(new Vector3(1, 1, 1) * Time.deltaTime*20, Space.World);
		Debug.Log(audioreceiver.loudness);

		if (audioreceiver.loudness > threshold) {
			//cube1.GetComponent<Renderer> ().material.color = colors[Mathf.RoundToInt (Random.value*6f)];
			//cube2.GetComponent<Renderer> ().material.color = colors[Mathf.RoundToInt (Random.value*6f)];
			cube1.GetComponent<Renderer> ().material.color = new Color(Random.value, Random.value, Random.value, Mathf.Max(1, audioreceiver.loudness));
			cube2.GetComponent<Renderer> ().material.color = new Color(Random.value, Random.value, Random.value, Mathf.Max(1, audioreceiver.loudness));

		}
		//Debug.DrawLine (Vector3.zero, new Vector3 (1, 0, 0), Color.red);
		float[] data = new float[256];
		float a = 0;
		audio.GetOutputData(data, 0);
		int count = 256;
		lr.SetVertexCount(count);

		for(int k=0; k<count; k++) {
			lr.SetPosition(k, new Vector3(-256 + 2*k, 300 * data[k], 200));
		}

		var spectrum = audio.GetSpectrumData(1024, 0, FFTWindow.BlackmanHarris);
		var i = 1;
		while ( i < 1023 ) {
			Debug.DrawLine(new Vector3(i - 1, spectrum[i] + 10, 0), new Vector3(i, spectrum[i + 1] + 10, 0), Color.red, 2, false);
			Debug.DrawLine(new Vector3(i - 1, Mathf.Log(spectrum[i - 1]) + 10, 2), new Vector3(i, Mathf.Log(spectrum[i]) + 10, 2), Color.cyan);
			Debug.DrawLine(new Vector3(Mathf.Log(i - 1), spectrum[i - 1] - 10, 1), new Vector3(Mathf.Log(i), spectrum[i] - 10, 1), Color.green);
			Debug.DrawLine(new Vector3(Mathf.Log(i - 1), Mathf.Log(spectrum[i - 1]), 3), new Vector3(Mathf.Log(i), Mathf.Log(spectrum[i]), 3), Color.yellow);
			i++;
		}

	}
	
	float GetAveragedVolume()
	{ 
		float[] data = new float[256];
		float a = 0;
		audio.GetOutputData(data, 0);
		foreach(float s in data)
		{
			a += Mathf.Abs(s);
		}
		return a/256;
	}
}
