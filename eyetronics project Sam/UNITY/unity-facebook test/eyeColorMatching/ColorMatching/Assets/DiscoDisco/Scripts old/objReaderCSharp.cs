/*
using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class objReaderCSharp : MonoBehaviour {
	string textFieldString = "http://howest.stage.eyebcom.com/teapot01.obj";
	Mesh myMesh = new Mesh();
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
		
	
	void OnGUI () {
		textFieldString = GUI.TextField(new Rect(10,10,500,20), textFieldString);
		
		if (GUI.Button(new Rect(10,40,100,20), "Click"))
		{
			StartCoroutine(SomeFunction());
		}
	}
	
	
	IEnumerator SomeFunction() {
		Debug.Log("hello");
		WWW www = new WWW(textFieldString);
		yield return www;
	
		string s = www.data;
		LoadFile(s);
	}
	
	
	public void LoadFile(string data) {
		//create lines;
		string[] lines = data.Split("\n"[0]);
		//arraylists to store vertices, normals and UV coordinates
		ArrayList vertsTemp = new ArrayList();
		ArrayList normsTemp = new ArrayList();
		ArrayList txruvsTemp = new ArrayList();
		//arraylists to store final vertices, normals and UV coordinates in the right order
		ArrayList verts = new ArrayList();
		ArrayList norms = new ArrayList();
		ArrayList txruvs = new ArrayList();
		
		// check each line to find the floats and put them in the rigth arraylist
		for(int i = 0; i < lines.Length; ++i)
		{
			if (lines[i].Length != 0)
			{
				ArrayList coords = new ArrayList();
				Vector3 myVertex = new Vector3();
				Vector3 myNormal = new Vector2();
				Vector2 myTxrCoord = new Vector3();
				// split line into pieces - 1 header (v, vn or vt) and 2 or 3 coordinates
				string[] pieces = lines[i].Split(" "[0]);
				// remove the spaces from the pieces, parse the coordinates to floats
				// and add them to the arraylist
				for (int j = 1; j < pieces.Length; ++j)
					pieces[j].Trim();
				// check the header (v, vn or vt) and add them to the matching arraylist
				if (pieces[0] == "vn")
				{
					for (int j = 1; j < pieces.Length; ++j)
						myNormal[j] = float.Parse(pieces[j]);
					normsTemp.Add(myNormal);
				}
				else if (pieces[0] == "vt")
				{
					for (int j = 1; j < pieces.Length; ++j)
						myTxrCoord[j] = float.Parse(pieces[j]);
					txruvsTemp.Add(myTxrCoord);
				}
				else if (pieces[0] == "v")
				{
					for (int j = 1; j < pieces.Length; ++j)
						myVertex[j] = float.Parse(pieces[j]);
					vertsTemp.Add(coords);
				}
				else if (pieces[0] == "f")
				{
					
					for (int k = 1; k < pieces.Length; ++k)
					{
						string[] FaceIndicesInStrings = pieces[k].Split("/"[0]);
						float[] FaceIndices = new float[FaceIndicesInStrings.Length];
						
						for (int l= 1; l < FaceIndicesInStrings.Length; ++l)
							FaceIndices[l-1] = float.Parse(FaceIndicesInStrings[l]);
						
						float temp = FaceIndices[0];
						
						verts.Add((float)vertsTemp[(int)temp]);
						if (FaceIndices.Length > 1)
						{
							temp = FaceIndices[1];
							norms.Add((float)normsTemp[(int)temp]);
						}
						if (FaceIndices.Length > 2)
						{
							temp = FaceIndices[2];
							txruvs.Add((float)txruvsTemp[(int)temp]);
						}
					}
				}
			}
		}
	}
}
*/