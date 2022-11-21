using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class objReaderCSharpV2 : MonoBehaviour {
	string _textFieldString = "http://howest.stage.eyebcom.com/sam_1object.obj";
//	string _textureLink = "http://www.savab-multimedia.com/downloads/pietje.jpg";
	
	Mesh _myMesh;
	Material _myMaterial = new Material(Shader.Find("Diffuse"));
	
	Mesh _bodyMesh;
	
	Vector3[] _vertexArray;
	int[] _triangleArray;
	ArrayList _vertexArrayList = new ArrayList();
	ArrayList _triangleArrayList = new ArrayList();

	// Use this for initialization
	void Start () {
			StartCoroutine(SomeFunction());
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	
	void OnGUI () {
		_textFieldString = GUI.TextField(new Rect(10,10,500,20), _textFieldString);

		if (GUI.Button(new Rect(10,40,100,20), "Click"))
		{
			StartCoroutine(SomeFunction());
		}
	}
	
	IEnumerator SomeFunction() {
		Debug.Log("started parsing the obj...");
		// re-initialize the mesh and name it
		_myMesh = new Mesh();
		_myMesh.name = "SavaBmesh";
		// retrieve data from OBJ file
		WWW www3d = new WWW(_textFieldString);
		yield return www3d;
		string s = www3d.data;
		// retrieve the texture
//		WWW wwwtx = new WWW(_textureLink);
//		yield return wwwtx;
//		_myMaterial.mainTexture = wwwtx.texture;
		//replace double spaces and dot-notations
		s = s.Replace("  ", " ");
		s = s.Replace(".", ",");
		// call loadFile() and pass through the data from the OBJ file
		LoadFile(s);
		//set the vertices and triangles to the unity mesh
		_myMesh.vertices = _vertexArray;
		_myMesh.triangles = _triangleArray;
		//calculate the normals
		_myMesh.RecalculateNormals();
//		_myMesh.RecalculateBounds();
			
		// check if there is allready a MeshFilter present, if not add one
		if ((MeshFilter)gameObject.GetComponent("MeshFilter") == null) {
			gameObject.AddComponent("MeshFilter");
			MeshFilter temp;
			temp = (MeshFilter)gameObject.GetComponent("MeshFilter");
			temp.mesh = _myMesh;
		}
		// check if there is allready a MeshRenderer present, if not add one
		if ((MeshRenderer)gameObject.GetComponent("MeshRenderer") == null) {
			_myMaterial.shader = Shader.Find("Diffuse");
			_myMaterial.color = Color.green;
			gameObject.AddComponent("MeshRenderer");
			MeshRenderer temp;
			temp = (MeshRenderer)gameObject.GetComponent("MeshRenderer");
			temp.material = _myMaterial;
		}
		
		
		Debug.Log("Done!");
	}
	
	public void LoadFile(string s) {
		// split the file into lines by detecting the breaklines
		string[] lines = s.Split("\n"[0]);
		// build the vertexarray by the number of vertices there are in the OBJ
		_triangleArrayList = new ArrayList();
		_vertexArrayList = new ArrayList();
		
		foreach (string item in lines) {
			ReadLine(item);
		}
		
		_triangleArray = new int[_triangleArrayList.Count];
		for (int i = 0; i < _triangleArrayList.Count; ++i)
		{
			_triangleArray[i] = (int) _triangleArrayList[i]-1;
		}
		
		_vertexArray = new Vector3[_vertexArrayList.Count];
		for (int i = 0; i < _vertexArrayList.Count; ++i)
		{
			_vertexArray[i] = (Vector3) _vertexArrayList[i];
		}
	}
	
	
	public void ReadLine(string s) {
		char[] charsToTrim = {' ', '\n', '\t', '\r'};
		s= s.TrimEnd(charsToTrim);
		
		string[] words = s.Split(" "[0]);
		
		foreach (string item in words) {
			item.Trim();
		}
		Vector3 coords;
		
		if (words[0] == "v") {
			coords = new Vector3(System.Convert.ToSingle(words[1]),
											System.Convert.ToSingle(words[2]), 
											System.Convert.ToSingle(words[3]));
		
			_vertexArrayList.Add(coords);
		}
		if (words[0] == "f") {
			if (words.Length == 4) {
				for (int i = 1; i < words.Length; ++i) {
					string[] indices = words[i].Split("/"[0]);
					_triangleArrayList.Add(System.Convert.ToInt32(indices[0]));
				}
			}
			else if (words.Length == 5) {
				int[] temp = new int[4];
				for (int i = 1; i < words.Length; ++i) {
					string[] indices = words[i].Split("/"[0]);
					temp[i-1] = System.Convert.ToInt32(indices[0]);
				}
				_triangleArrayList.Add(temp[0]);
				_triangleArrayList.Add(temp[1]);
				_triangleArrayList.Add(temp[2]);
				
				_triangleArrayList.Add(temp[0]);
				_triangleArrayList.Add(temp[2]);
				_triangleArrayList.Add(temp[3]);
			}
		}
	}
	
	public static int CountStringOccurrences(string s, string pattern) {
        int count = 0;
        int i = 0;
        while ((i = s.IndexOf(pattern, i)) != -1)
        {
            i += pattern.Length;
            count++;
        }
        return count;
    }

}
