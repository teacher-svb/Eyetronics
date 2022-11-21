using UnityEngine;
using System.Collections;

public class scrollui : MonoBehaviour {
	public Vector2 scrollViewVector = Vector2.zero;
	string[] stringarray;
	
	public int lineoffset = 14;
	public int aantallijnen = 0;
	public int selectedline = 0;
	
	// Use this for initialization
	void Start () {
		lerpcolor = 0.01f;
		stringarray = new string[10];
		stringarray[0] ="----------------------";
		stringarray[1] ="Relight my fire : blabla1";
		stringarray[2] ="Relight my fire : blabla2";
		stringarray[3] ="Relight my fire : blabla3";
		stringarray[4] ="Relight my fire : blabla4";
		stringarray[5] ="Relight my fire : blabla5";
		stringarray[6] ="Relight my fire : blabla6";
		stringarray[7] ="Relight my fire : blabla7";
		stringarray[8] ="Relight my fire : blabla8";
		stringarray[9] ="----------------------";
		aantallijnen = stringarray.Length - 2;
	}
	
	//~ // Update is called once per frame
	void Update () {
		if(scrollViewVector.y < selectedline *lineoffset)
		{
			++scrollViewVector.y;
		}
		else if(scrollViewVector.y > selectedline *lineoffset)
		{
			--scrollViewVector.y;
		}
		
		
	}
	
	void OnGUI(){
		GUI.color = Color.black;
		GUI.skin = myskin;
		//----------------------------------------------------------------------------
		scrollViewVector = GUI.BeginScrollView(new Rect (25, 25, 300, 45), scrollViewVector, new Rect (0, 0, 300, (aantallijnen+2) * lineoffset));
		GUILayout.BeginArea( new Rect( 0, 0, 300, (aantallijnen+2) * lineoffset ) );
		for(int i =0; i<stringarray.Length;++i){
			if(i == selectedline + 1){
				GUI.color = Color.white;
			}
			GUI.Label(new Rect(0,i*lineoffset,200,20),stringarray[i]);
			GUI.color = Color.black;
		}
		GUILayout.EndArea();    
		GUI.EndScrollView();
		//----------------------------------------------------------------------------
		
		GUI.color = Color.white;
		if(GUI.Button(new Rect(0,15,300,10),"")){
			if(selectedline > 0)
				selectedline--;
			}
		if(GUI.Button(new Rect(0,65,300,10),"")){
			if(selectedline < aantallijnen - 1)
				selectedline++;
			}
	}
}
