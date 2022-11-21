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

	public string _textFieldString = "http://people.sc.fsu.edu/~burkardt/data/obj/cessna.obj";
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

	// Use this for initialization
	public IEnumerator Init (string gameObjectName) {
		yield return StartCoroutine(SomeFunction(gameObjectName));
	}
	
	void initArrayLists() {
		_uvArrayList = new ArrayList();
		_normalArrayList = new ArrayList();
		_vertexArrayList = new ArrayList();
		_facesVertNormUV = new ArrayList();
	}
	
	public IEnumerator SomeFunction(string gameObjectName) {
		GameObject obj_gameobject = GameObject.Find(gameObjectName);
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
		string s = www3d.data;
		//replace double spaces and dot-notations
		s = s.Replace("  ", " ");
		s = s.Replace("  ", " ");
		s = s.Replace(".", ",");
		// call loadFile() and pass through the data from the OBJ file --> here we load the OBJ
		LoadFile(s);
		//set the vertices and triangles to the unity mesh
//		Debug.Log(_vertexArrayList.Count);
//		Debug.Log(_triangleArray.Length);
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
		// fill the arrays by crossreferencing the data in _facesVertNormUV and 
		// the arraylists of each type
		int i = 0;
//		Debug.Log(_facesVertNormUV.Count);
		foreach (Vector3 item in _facesVertNormUV) {
			_vertexArray[i] = (Vector3)_vertexArrayList[(int)item.x - 1];
			if (_uvArrayList.Count > 0) {
			Vector3 tVec = (Vector3)_uvArrayList[(int)item.y - 1];
			_uvArray[i] = new Vector2(tVec.x, tVec.y);
			}
			if (_normalArrayList.Count > 0) {
				_normalArray[i] = (Vector3)_normalArrayList[(int)item.z - 1];
			}
			_triangleArray[i] = i;
			i++;
		}
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
			/*
			int i = 0;
			triangleList.Add(temp[0]);
			i++;
			triangleList.Add(temp[1]);
			i++;
			triangleList.Add(temp[2]);
			i++;
			for (; i+1 <= temp.Count; i+=3) {
				if (i % 2 != 0) {
					triangleList.Add(triangleList[i-3]);
					triangleList.Add(triangleList[i-1]);
					triangleList.Add(temp[(i/3)+2]);
				}
				else{
					triangleList.Add(triangleList[i-1]);
					triangleList.Add(triangleList[i-2]);
					triangleList.Add(temp[(i/3)+2]);
				}
			}
			*/
			foreach (Vector3 item in triangleList) {
				_facesVertNormUV.Add(item);
			}
		}
	}
}
