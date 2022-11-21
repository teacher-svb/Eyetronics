/* ******************************

obj runtime importer by SavaB
----------------------------------
This script is free to use, as long as credits for the script are reserved to Sam Van Battel (SavaB).

How To Use?
--------------
 - Add the script to the main camera. Adjust _textFieldString and _textureLink to direct to the
   right model and texture.
   
Notes
-------
 - This script is still under development. Check the website or the communityforum topic regularly topic
   get the latest version

Contact:
----------
SavaB
programmer - technical artist
www.savab-multimedia.com

****************************** */

using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class objReaderCSharpV4 : MonoBehaviour {

	public string _textFieldString = "http://howest.stage.eyebcom.com/DanceDance/Users/cube.obj";
	public string _textureLink = "";
	
	public string _meshName = "unknown";
	
	Mesh _myMesh;
	Material _myMaterial = new Material(Shader.Find("Diffuse"));
	
	Vector3[] _vertexArray;
	ArrayList _vertexArrayList = new ArrayList();
	Vector3[] _normalArray;
	ArrayList _normalArrayList = new ArrayList();
	Vector2[] _uvArray;
	ArrayList _uvArrayList = new ArrayList();
	
	int[] _triangleArray;
	
	ArrayList _facesVertNormUV = new ArrayList();
	
    internal class IndexPair {
		public IndexPair(int tOld, int tNew) {
			_old = tOld;
			_new = tNew;
		}
		public int _old;
		public int _new;
    }

	// Use this for initialization
	public IEnumerator Init () {
		yield return StartCoroutine(SomeFunction());
	}
	
	public void OnGUI() {
		_textFieldString = GUI.TextField(new Rect(10,10,300,20), _textFieldString);
		GUI.Label(new Rect(320,10,300,20), "mesh");
		_textureLink = GUI.TextField(new Rect(10,40,300,20), _textureLink);
		GUI.Label(new Rect(320,40,300,20), "texture");
		
		if (GUI.Button(new Rect(10,70,100,20), "click")) {
			StartCoroutine(Init());
		}
		
		
		GUI.Label(new Rect(10,Screen.height - 60,300,20), "scroll to zoom; click and drag to rotate");
		GUI.Label(new Rect(10,Screen.height - 40,300,20), "//  note: no texture without UVs");
		GUI.Label(new Rect(10,Screen.height - 20,300,20), "//  note: no smoothing without normals");
	}
	
	void initArrayLists() {
		_uvArrayList = new ArrayList();
		_normalArrayList = new ArrayList();
		_vertexArrayList = new ArrayList();
		_facesVertNormUV = new ArrayList();
	}
	
	public IEnumerator SomeFunction() {
		if (GameObject.Find("obj_gameobject") != null)
			GameObject.Destroy(GameObject.Find("obj_gameobject"));
		GameObject obj_gameobject = new GameObject();
		obj_gameobject.name = "obj_gameobject";
		//Debug.Log("started parsing the obj...");
		initArrayLists();
		// re-initialize the mesh and name it
		if (_myMesh != null)
			_myMesh.Clear();
		_myMesh = new Mesh();
		_myMesh.name = _meshName;
		// retrieve data from OBJ file
		WWW www3d = new WWW(_textFieldString);
		yield return www3d;
		string s = www3d.text;
		//replace double spaces and dot-notations
		s = s.Replace("  ", " ");
		s = s.Replace("  ", " ");
		s = s.Replace(".", ",");
		// call loadFile() and pass through the data from the OBJ file --> here we load the OBJ
		LoadFile(s);
		//set the vertices and triangles to the unity mesh
		//~ Debug.Log(_vertexArrayList.Count);
		//~ Debug.Log(_triangleArray.Length);
		_myMesh.vertices = _vertexArray;
		_myMesh.triangles = _triangleArray;
		if (_uvArrayList.Count > 0)
			_myMesh.uv = _uvArray;
		if (_normalArrayList.Count > 0)
			_myMesh.normals = _normalArray;
		else
			_myMesh.RecalculateNormals();
		//calculate the bounds
		_myMesh.RecalculateBounds();
		// check if there is allready a MeshFilter present, if not add one
		if ((MeshFilter)obj_gameobject.GetComponent("MeshFilter") == null)
			obj_gameobject.AddComponent("MeshFilter");
		//assign the mesh to the meshfilter
		MeshFilter temp;
		temp = (MeshFilter)obj_gameobject.GetComponent("MeshFilter");
		temp.mesh = _myMesh;
		// check if there is allready a MeshRenderer present, if not add one
		if ((MeshRenderer)obj_gameobject.GetComponent("MeshRenderer") == null)
			obj_gameobject.AddComponent("MeshRenderer");
		// retrieve the texture
		if (_uvArrayList.Count > 0 && _textureLink != "") {
			WWW wwwtx = new WWW(_textureLink);
			yield return wwwtx;
			_myMaterial.mainTexture = wwwtx.texture;
		}
		// assign the texture to the meshrenderer
		MeshRenderer temp2;
		temp2 = (MeshRenderer)obj_gameobject.GetComponent("MeshRenderer");
		if (_uvArrayList.Count > 0 && _textureLink != "") {
			temp2.material = _myMaterial;
			_myMaterial.shader = Shader.Find("Diffuse");
		}
		
		yield return new WaitForFixedUpdate();
		//Debug.Log("Done in " + Time.deltaTime + "seconds! \nvertex count: " + _myMesh.vertexCount);
		//Debug.Log(_facesVertNormUV.Count);
	}
	
	public void LoadFile(string s) {
		// split the file into lines by detecting the breaklines
		string[] lines = s.Split("\n"[0]);
		
		foreach (string item in lines) {
			ReadLine(item);
		}
		//re-initialize the arrays
		_vertexArray = new Vector3[_facesVertNormUV.Count];
		_uvArray = new Vector2[_facesVertNormUV.Count];
		_normalArray = new Vector3[_facesVertNormUV.Count];
		_triangleArray = new int[_facesVertNormUV.Count];
		
		ArrayList vertexArrayList = new ArrayList();
		ArrayList uvArrayList = new ArrayList();
		ArrayList normalArrayList = new ArrayList();
		ArrayList triangleArrayList = new ArrayList();
		// fill the arrays by crossreferencing the data in _facesVertNormUV and 
		// the arraylists of each type
		int i = 0;
		
		foreach (Vector3 item in _facesVertNormUV) {
			vertexArrayList.Add((Vector3)_vertexArrayList[(int)item.x - 1]);
			if (_uvArrayList.Count > 0) {
			Vector3 tVec = (Vector3)_uvArrayList[(int)item.y - 1];
			uvArrayList.Add(new Vector2(tVec.x, tVec.y));
			}
			if (_normalArrayList.Count > 0) {
				normalArrayList.Add((Vector3)_normalArrayList[(int)item.z - 1]);
			}
			triangleArrayList.Add(i);
			i++;
		}
		
		for (int k = 0; k < vertexArrayList.Count; ++k) {
			Vector3 kVertTemp = (Vector3)vertexArrayList[k];
			Vector2 kUvTemp = (Vector2)uvArrayList[k];
			Vector3 kNormTemp = (Vector3)normalArrayList[k];
			
			for (int j = k+1; j < vertexArrayList.Count; ++j) {
				Vector3 jVertTemp = (Vector3)vertexArrayList[j];
				Vector2 jUvTemp = (Vector2)uvArrayList[j];
				Vector3 jNormTemp = (Vector3)normalArrayList[j];
				
				if (kVertTemp.x == jVertTemp.x && kVertTemp.y == jVertTemp.y && kVertTemp.z == jVertTemp.z &&
					kUvTemp.x == jUvTemp.x && kUvTemp.y == jUvTemp.y &&
					kNormTemp.x == jNormTemp.x && kNormTemp.y == jNormTemp.y && kNormTemp.z == jNormTemp.z) {
					triangleArrayList[j] = triangleArrayList[k];
					
					for (int l = j+1; l < triangleArrayList.Count; ++l) {
						int t = (int)triangleArrayList[l];
						triangleArrayList[l] = t--;
					}
				}
			}
		}
		
		int tester = 0;
		ArrayList testAL = new ArrayList();
		for (int test = 0; test < vertexArrayList.Count; ++test) {
			foreach (int item in triangleArrayList){
				if (test == item) {
					tester = -1;
					break;
				}
				else
					tester = test;
			}
			if (tester != -1)
				testAL.Add(tester);
		}
		
		foreach (int item in testAL) {
			vertexArrayList[item] = null;
			uvArrayList[item] = null;
			normalArrayList[item] = null;
		}
		
		ArrayList newVertexArrayList = new ArrayList();
		ArrayList newUvArrayList = new ArrayList();
		ArrayList newNormalArrayList = new ArrayList();
		ArrayList oldNewIndexArrayList = new ArrayList();
		
		for (int j = 0; j < vertexArrayList.Count; ++j) {
			if (vertexArrayList[j] != null) {
				newVertexArrayList.Add(vertexArrayList[j]);
				newUvArrayList.Add(uvArrayList[j]);
				newNormalArrayList.Add(normalArrayList[j]);
				oldNewIndexArrayList.Add(new IndexPair(j, newVertexArrayList.Count - 1));
			}
		}
		
		for (int j = 0; j < triangleArrayList.Count; ++j) {
			foreach (IndexPair pair in oldNewIndexArrayList) {
				if ((int)triangleArrayList[j] == pair._old)
					triangleArrayList[j] = pair._new;
			}
		}
		
		_vertexArray = (Vector3[])newVertexArrayList.ToArray(typeof(Vector3));
		_uvArray = (Vector2[])newUvArrayList.ToArray(typeof(Vector2));
		_normalArray = (Vector3[])newNormalArrayList.ToArray(typeof(Vector3));
		_triangleArray = (int[])triangleArrayList.ToArray(typeof(int));
	}
	
	public void ReadLine(string s) {
		//remove any trailing white-space chararcters to ensure that there will be no empty splits
		char[] charsToTrim = {' ', '\n', '\t', '\r'};
		s= s.TrimEnd(charsToTrim);
		//split the incoming string in words
		string[] words = s.Split(" "[0]);
		//trim each word to avoid white-space chararcters
		foreach (string item in words)
			item.Trim();
		//assemble all vertices, normals and uv-coordinates
		if (words[0] == "v")  {
			_vertexArrayList.Add(new Vector3(System.Convert.ToSingle(words[1]), System.Convert.ToSingle(words[2]), System.Convert.ToSingle(words[3])));
		}
		if (words[0] == "vn")
			_normalArrayList.Add(new Vector3(System.Convert.ToSingle(words[1]), System.Convert.ToSingle(words[2]), System.Convert.ToSingle(words[3])));
		if (words[0] == "vt") 
			_uvArrayList.Add(new Vector3(System.Convert.ToSingle(words[1]), System.Convert.ToSingle(words[2])));
		//assemble the faces by index, and disassemble them back to each point
		if (words[0] == "f") {
			ArrayList temp = new ArrayList();
			ArrayList triangleList = new ArrayList();
			for (int j = 1; j < words.Length; ++j)
			{
				Vector3 indexVector = new Vector3(0,0);
				string[] indices = words[j].Split("/"[0]);
				indexVector.x = System.Convert.ToInt32(indices[0]);
				if (indices.Length > 1) {
					if (indices[1] != "")
						indexVector.y = System.Convert.ToInt32(indices[1]);
				}
				if (indices.Length > 2) {
					if (indices[2] != "")
						indexVector.z = System.Convert.ToInt32(indices[2]);
				}
				temp.Add(indexVector);
			}
			for (int i = 1; i < temp.Count - 1; ++i) {
				triangleList.Add(temp[0]);
				triangleList.Add(temp[i]);
				triangleList.Add(temp[i+1]);
			}
			
			foreach (Vector3 item in triangleList) {
				_facesVertNormUV.Add(item);
			}
		}
	}
}
