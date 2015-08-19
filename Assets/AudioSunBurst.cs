// Sunburst effects.
// By Keijiro Takahashi, 2013
// https://github.com/keijiro/unity-sunburst-mesh-fx
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshRenderer))]
[RequireComponent (typeof (AudioSource))]
public class AudioSunBurst : MonoBehaviour
{
	#region Public variables
	public int beamCount = 100;
	[Range(0.01f, 0.5f)]
	public float beamWidth = 0.1f;
	[Range(0.1f, 10.0f)]
	public float speed = 0.4f;
	[Range(1.0f, 10.0f)]
	public float scalePower = 1.0f;
	#endregion
	
	#region Beam vectors
	Vector3[] beamDir;
	Vector3[] beamExt;
	#endregion
	
	#region Mesh data
	Mesh mesh;
	Vector3[] vertices;
	#endregion
	
	#region Animation parameters
	const float indexToNoise = 0.77f;
	float time;
	#endregion

	private AudioSource audio;
	AudioReceiver audioreceiver;
	MeshRenderer meshr;
	public float sensitivity = 0.1f;

	// Camera
	private Camera maincam;
	private Vector3 camPosition;
	
	#region Private functions
	void ResetBeams ()
	{
		// Allocate arrays.
		beamDir = new Vector3[beamCount];
		beamExt = new Vector3[beamCount];
		vertices = new Vector3[beamCount * 3];
		var normals = new Vector3[beamCount * 3];
		
		// Initialize the beam vectors.
		var normalIndex = 0;
		for (var i = 0; i < beamCount; i++) {
			// Make a beam in a completely random way.
			var dir = Random.onUnitSphere;
			//var v = new Vector3(0.1f, 0.1f, 0.1f);
			var ext = Random.onUnitSphere;
			Debug.Log (dir);
			Debug.Log (ext);
			beamDir [i] = dir;
			beamExt [i] = ext;
			
			// Use a slightly modified vector on the first vertex to make a gradation.
			var normal = Vector3.Cross (dir, ext).normalized;
			normals [normalIndex++] = Vector3.Lerp (dir, normal, 0.5f).normalized;
			normals [normalIndex++] = normal;
			normals [normalIndex++] = normal;

		}
		
		// Initialize the triangle set.
		var indices = new int[beamCount * 3];
		for (var i = 0; i < indices.Length; i++) {
			indices [i] = i;
		}
		
		// Initialize the mesh.
		mesh.Clear ();
		mesh.vertices = vertices;
		mesh.normals = normals;
		mesh.triangles = indices;

	}
	
	void UpdateVertices ()
	{

		var vertexIndex = 0;
		for (var i = 0; i < beamCount; i++) {
			// Use 2D Perlin noise to animate the beam.
			//var scale = Mathf.Pow (Mathf.PerlinNoise (time, i * indexToNoise), scalePower)*audioreceiver.loudness;;
			//var scale = 1.0f + audioreceiver.volume*Mathf.PerlinNoise(time, i*indexToNoise)*3;
			var scale = 1.0f;
			beamWidth = audioreceiver.loudness*sensitivity;
			//audioreceiver.loudness
			
			// Never modify the first vertex.
			vertexIndex++;
			
			// Update the 2nd and 3rd vertices.
			var tip = beamDir [i] * scale;
			var ext = beamExt [i] * beamWidth * scale;
			vertices [vertexIndex++] = tip - ext; 
			//vertices [vertexIndex++] = tip + ext;
			vertices [vertexIndex++] = tip + ext;

		}
	}
	#endregion
	
	#region Monobehaviour functions
	void Awake ()
	{
		// Initialize the mesh instance.
		mesh = new Mesh ();
		mesh.MarkDynamic (); //Call this when you continually update mesh vertices.
		GetComponent<MeshFilter> ().sharedMesh = mesh;

		meshr = GetComponent<MeshRenderer> ();
		//meshr.material.SetColor ("_LineColor", new Color(Random.value, Random.value, Random.value,1));

		// Initialize the beam array.
		ResetBeams ();

		maincam = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera>();
	}

	void Start() 
	{
		audio = GetComponent<AudioSource>();
		audioreceiver = GetComponent<AudioReceiver>(); // Reference the audioreceiver script.
	}
	
	void Update ()
	{
		// Reset the beam array if the number was changed.
		if (beamCount != beamDir.Length) {
			ResetBeams ();
		}
		
		// Do animation.
		UpdateVertices ();
		
		// Update the vertex array.
		mesh.vertices = vertices;

		gameObject.transform.Rotate (Vector3.up * Time.deltaTime * 40, Space.World);
		
		// Advance the time count.
		time += Time.deltaTime * speed;

		if (Input.GetKeyDown (KeyCode.Space)) {
			iTween.ColorTo (gameObject, iTween.Hash("time", 3.0f, "NamedColorValue", "_Color", "easeType", "easeInOutQuad", "color", new Color(Random.value, Random.value, Random.value,1)));
			//meshr.material.SetColor ("_Color", new Color(Random.value, Random.value, Random.value,1));
			Debug.Log ("color change");
		} else if (Input.GetKeyDown (KeyCode.RightArrow)) {
			changeCamPosition();
		}
		if (Input.GetKeyDown (KeyCode.UpArrow)) {
			sensitivity += 0.01f;
			Debug.Log (sensitivity);
		}
		if (Input.GetKeyDown (KeyCode.DownArrow)) {
			if (sensitivity >= 0.01f) {
				sensitivity -= 0.01f;
				Debug.Log (sensitivity);
			}
		}

	}

	private void changeCamPosition() {
		Vector3 R = Random.onUnitSphere*10;
		R.y = Mathf.Abs (R.y);
		Vector3 center = new Vector3(0, 1, 0);
		Debug.Log (R);
		camPosition = center + R;
		//maincam.transform.position = camPosition;
		//maincam.transform.LookAt(center);
		//iTween.MoveTo(maincam.gameObject, camPosition, 1);
		//iTween.LookTo(maincam.gameObject, center, 1);
		iTween.MoveTo(maincam.gameObject, iTween.Hash("x", camPosition.x, "y", camPosition.y, "z", camPosition.z, "time", 1.0f, "looktarget", center));
		//maincam.transform.LookAt(center);
		//iTween.LookTo(maincam.gameObject, center, 0.3f);
	}

	#endregion
}