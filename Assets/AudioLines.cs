using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshRenderer))]
[RequireComponent (typeof (AudioSource))] // Requires AudioReceiver.cs instead.
[RequireComponent (typeof (MeshCollider))]
public class AudioLines : MonoBehaviour, AudioProcessor.AudioCallbacks {
	
	private AudioSource audio;
	//AudioReceiver audioreceiver;
	LineRenderer lr;
	//public LineRenderer[] lrs;
	
	// Mesh
	MeshFilter meshfilter;
	Mesh mesh;
	MeshCollider meshcollider;
	Rigidbody rigidb;
	Vector3[] vertices;
	
	public int N = 1;
	public int M = 256;
	private Vector3[,] wavePositions;
	public int dz = -4;
	public int sensitivity = 300;
	
	// Camera
	private Camera maincam;
	private Vector3 camPosition;
	
	// Mesh type
	//[SerializeField]
	public enum WaveType {
		triangles,
		lines,
		points
	};
	public WaveType wavetype = WaveType.lines;
	
	//new Color(Random.value, Random.value, Random.value,1)
	
	void Awake ()
	{
		// Initialize the mesh instance.
		mesh = new Mesh ();
		mesh.MarkDynamic (); //Call this when you continually update mesh vertices.
		meshfilter = GetComponent<MeshFilter> ();
		meshfilter.sharedMesh = mesh;
		meshcollider = GetComponent<MeshCollider> ();
		//rigidb = GetComponent<Rigidbody> ();
		//Application.targetFrameRate = 30;
		Application.runInBackground = true;
		maincam = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera>();
	}
	
	// Use this for initialization
	void Start () {
		
		//audioreceiver = GetComponent<AudioReceiver>(); // Reference the audioreceiver script.
		//Debug.Log (audioreceiver.teststr);
		
		audio = GetComponent<AudioSource>();
		
		lr = GetComponent<LineRenderer> ();
		// Initialize LineRenderers
		//lrs = new LineRenderer[N];
		wavePositions = new Vector3[N, M];
		
		vertices = new Vector3[N*M];
		var normals = new Vector3[N*M];
		
		int vertexIndex = 0;
		for (int j=0; j<N; j++) {
			//lrs[j] = GetComponent<LineRenderer>();
			//lrs[j].SetVertexCount(count);
			for(int k=0; k<M; k++) {
				Vector3 wavepos = new Vector3(256*Mathf.Cos (2f * Mathf.PI/M*k), 0, 256*Mathf.Sin (2f * Mathf.PI/M*j));  //(-256 + 2*256/M*k, 0, 500 + dz*j);
				//wavePositions[j, k] = wavepos;
				//lrs[j].SetPosition(k, new Vector3(-256 + 2*k, 0, 200 - 10*j));
				vertices[vertexIndex] = wavepos;
				normals[vertexIndex] = new Vector3(0, 0, -1);
				//normals[vertexIndex] = Random.onUnitSphere*10;
				vertexIndex++;
			}
		}
		
		// Initialize the triangle set.
		//var indices = new int[N*M];
		//for (var i = 0; i < indices.Length; i++) {
		//	indices [i] = i;
		//}
		/*
		var indices = new int[(N - 1) * (M - 1) * 6];
		int index = 0;
		for (var j=0; j<N-1; j++) {
			for (var k=M*j; k<M*j+M-1; k++) {
				//Debug.Log (index);
				indices[index] = k;
				indices[index+1] = k+1;
				indices[index+2] = k+M;
				indices[index+3] = k+1;
				indices[index+4] = k+M;
				indices[index+5] = k+M+1;
				index += 6;
			}
		}*/
		
		/*
		var indices = new int[2*M];
		int index = 0;
		for (var k=0; k<M; k++) {
			indices[index] = k;
			if (k<M-1) {
				indices[index+1] = k+1;
			} else {
				indices[index+1] = 0;
			}
			index += 2;
		}*/
		
		var indices = new int[8*M];
		int index = 0;
		for (var k=0; k<M; k++) {
			indices[index] = k;
			if (k%4==3) {
				indices[index+1] = k-3;
			} else {
				indices[index+1] = k+1;
			}
			index += 4;
		}
		
		
		// Initialize the mesh.
		mesh.Clear ();
		mesh.vertices = vertices;
		mesh.normals = normals;
		switch(wavetype) {
		case WaveType.lines:
			mesh.SetIndices(indices, MeshTopology.Lines, 0);
			break;
		case WaveType.triangles:
			mesh.triangles = indices;
			meshcollider.sharedMesh = mesh;
			break;
		case WaveType.points:
			mesh.SetIndices(indices, MeshTopology.Points, 0);
			break;
		}
		//Select the instance of AudioProcessor and pass a reference
		//to this object
		//AudioProcessor processor = FindObjectOfType<AudioProcessor>();
		//processor.addAudioCallback(this);
		
	}
	
	// Update is called once per frame
	void Update () {
		
		/*audio.GetOutputData(waveData_, 1);
		Debug.Log (waveData_);

		var volume = waveData_.Select(x => x*x).Sum() / waveData_.Length;
		transform.localScale = Vector3.one * volume;

		Debug.Log (volume);*/
		
		//float volume = GetAveragedVolume ();
		//Debug.Log (volume);
		//Debug.DrawLine (Vector3.zero, new Vector3 (1, 0, 0), Color.red);
		float[] data = new float[M];
		lr.SetVertexCount(M);
		
		// Audio data or Audo spectrum
		audio.GetOutputData(data, 0);
		//audio.GetSpectrumData(data, 0, FFTWindow.BlackmanHarris); 
		
		for(int k=0; k<M; k++) {
			lr.SetPosition(k, new Vector3(-256 + 2*256/M*k, 300 * data[k], 500));
		}
		// 3D waves
		/*
		int vertexIndex = 0;
		for (int j=N-1; j>=0; j--) {
			//Debug.Log (j);
			for (int k=0; k<M; k++) {
				if (j==0) {
					var R = -256 + 4*sensitivity * data[k];
					wavePositions[j, k] = new Vector3(R*Mathf.Cos (2f * Mathf.PI*k/M), 4*sensitivity * data[k], R*Mathf.Sin (2f * Mathf.PI*k/M)); //new Vector3(-256 + 2*256/M*k, sensitivity * data[k], 500 + dz*j);
				
				}
				else {
					wavePositions[j, k] = wavePositions[j-1, k];
					//wavePositions[j,k].y += 5*j;
					wavePositions[j, k].z = 500 + dz*j;
				}
				vertices[vertexIndex] = wavePositions[j, k];
				vertexIndex++;
			}
		}*/
		
		int vertexIndex = 0;
		for (int j=N-1; j>=0; j--) {
			//Debug.Log (j);
			for (int k=0; k<M; k++) {
				var L = 256*(M-k)/M + 256/M*data[k]*sensitivity;
				if (j==0) {
					wavePositions[j, k] = new Vector3(-L - 1*data[k], 0, L - 1*data[k]); //new Vector3(-256 + 2*256/M*k, sensitivity * data[k], 500 + dz*j);
					
				}
				/*else {
					wavePositions[j, k] = wavePositions[j-1, k];
					//wavePositions[j,k].y += 5*j;
					wavePositions[j, k].z = 500 + dz*j;
				}*/
				//vertices[vertexIndex] = wavePositions[j, k];
				vertices[vertexIndex] = new Vector3(-L, 0, L);
				vertices[vertexIndex+1] = new Vector3(L, 0, L);
				vertices[vertexIndex+2] = new Vector3(L, 0, -L);
				vertices[vertexIndex+3] = new Vector3(-L, 0, -L);
				vertexIndex += 4;
			}
		}
		
		// Update the vertex array.
		mesh.vertices = vertices;
		
		// Recalculate normals for triangle mesh
		if (wavetype == WaveType.triangles) {
			mesh.RecalculateNormals();
		}
		
		//Debug.Log (mesh.normals [200]);
		
		//var spectrum = audio.GetSpectrumData(1024, 0, FFTWindow.BlackmanHarris);
		/*var i = 1;
		while ( i < 1023 ) {
			Debug.DrawLine(new Vector3(i - 1, spectrum[i] + 10, 0), new Vector3(i, spectrum[i + 1] + 10, 0), Color.red, 2, false);
			Debug.DrawLine(new Vector3(i - 1, Mathf.Log(spectrum[i - 1]) + 10, 2), new Vector3(i, Mathf.Log(spectrum[i]) + 10, 2), Color.cyan);
			Debug.DrawLine(new Vector3(Mathf.Log(i - 1), spectrum[i - 1] - 10, 1), new Vector3(Mathf.Log(i), spectrum[i] - 10, 1), Color.green);
			Debug.DrawLine(new Vector3(Mathf.Log(i - 1), Mathf.Log(spectrum[i - 1]), 3), new Vector3(Mathf.Log(i), Mathf.Log(spectrum[i]), 3), Color.yellow);
			i++;
		}*/
		if (Input.GetKeyDown (KeyCode.RightArrow)) {
			transformColor ();
			Debug.Log ("color change");
		} 
		if (Input.GetKeyDown (KeyCode.Space)) {
			changeCamPosition();
		}
		if (Input.GetKeyDown (KeyCode.UpArrow)) {
			sensitivity += 50;
			Debug.Log (sensitivity);
		}
		if (Input.GetKeyDown (KeyCode.DownArrow)) {
			if (sensitivity > 49) {
				sensitivity -= 50;
				Debug.Log (sensitivity);
			}
		}
	}
	
	private void CompleteHandler(string message) {
		Debug.Log (message);
		iTween.LookTo(maincam.gameObject, new Vector3(0, 0, 300), 1.0f);
	}
	
	//this event will be called every time a beat is detected.
	//Change the threshold parameter in the inspector
	//to adjust the sensitivity
	public void onBeatDetected()
	{
		Debug.Log("Beat!!!");
		//changeCamPosition ();
	}
	
	//This event will be called every frame while music is playing
	public void onSpectrum(float[] spectrum)
	{
		//The spectrum is logarithmically averaged
		//to 12 bands
		
		for (int i = 0; i < spectrum.Length; ++i)
		{
			Vector3 start = new Vector3(i, 0, 0);
			Vector3 end = new Vector3(i, spectrum[i], 0);
			Debug.DrawLine(start, end);
		}
	}
	
	private void transformColor() {
		iTween.ColorTo (gameObject, iTween.Hash("time", 3.0f, "NamedColorValue", "_Color", "easeType", "easeOutCubic", "color", new Color(Random.value, Random.value, Random.value,1)));
	}
	
	private void changeCamPosition() {
		Vector3 R = Random.onUnitSphere*300;
		R.y = Mathf.Abs (R.y);
		Vector3 center = new Vector3(0, 0, 0);
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