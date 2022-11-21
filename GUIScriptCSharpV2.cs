/*
// ******* GUI SCRIPT ********** //
the GUI script handles most of the GUI. This class is made just not to clutch the gamemanagerscript.
All the menus are made and shown here.
// ******* GUI SCRIPT ********** //
*/

using UnityEngine;
using System.Collections;
using System.Xml;

public class GUIScriptCSharpV2 : MonoBehaviour {
	
	public enum menuStates { MS_start, MS_levelSelection, MS_pause };
	public menuStates _menuState = menuStates.MS_start;
	
	public Vector2 scrollViewVector = Vector2.zero;
	
	string _pathTogameXML = "http://howest.stage.eyebcom.com/DanceDance/dancedance.xml";
	
	public string[] _songs;
	public string[] _songPaths;
	
	public int lineoffset = 14;
	public int aantallijnen = 0;
	public int selectedline = 1;
	public int cameraPosition = 0; 
	public int selectionGridLevel = 0;
	public int selectionGridSong = 0;
	
	public GUISkin myskin;
	
	public Texture2D GUIBackground;
	public Texture2D leftarrow;
	public Texture2D rightarrow;
	
	public Texture _loadTxr_base;
	public Texture _loadTxr_pixel;
	
	
	public IEnumerator loadData () {
		WWW www = new WWW(_pathTogameXML);
		yield return www;
		XmlDocument gameXML = new XmlDocument();
		gameXML.LoadXml(www.data);
		XmlNodeList songNodeList = gameXML.GetElementsByTagName("song");
		
		_songs = new string[songNodeList.Count];
		_songPaths = new string[songNodeList.Count];
		
		for (int i = 0; i < songNodeList.Count; ++i) {
			XmlAttributeCollection attrCol = songNodeList[i].Attributes;
			_songs[i] = attrCol["name"].InnerXml.Replace("\\n", System.Environment.NewLine);
			_songPaths[i] = attrCol["link"].InnerXml;
		}
	}
	
	void Update () {
		if (_menuState == menuStates.MS_levelSelection) {
			if (GameObject.Find("GameCamera").GetComponent<GUIScriptCSharpV2>()._menuState == GUIScriptCSharpV2.menuStates.MS_levelSelection) {
				if(scrollViewVector.y < (selectedline-1) *lineoffset)
				{
					++scrollViewVector.y;
				}
				else if(scrollViewVector.y > (selectedline-1) *lineoffset)
				{
					--scrollViewVector.y;
				}
			}
			aantallijnen = _songs.Length - 2;
		}
		float smooth = 5.0f;
			
		float tiltAngle = 90.0f * cameraPosition;
		Quaternion target = Quaternion.Euler (GameObject.Find("GameCamera").transform.eulerAngles.x, tiltAngle,0);
		GameObject.Find("GameCamera").transform.localRotation = Quaternion.Slerp(GameObject.Find("GameCamera").transform.localRotation, target, Time.deltaTime * smooth);
	}
	
	void OnGUI () {
		GUI.skin = myskin;
		switch (this.gameObject.GetComponent<GameManagerCSharpV1>().GameState) {
			case GameManagerCSharpV1.gameStates.GS_playing: 
				OnPlay();
				break;
			case GameManagerCSharpV1.gameStates.GS_paused:
				OnPause();
				break;
			case GameManagerCSharpV1.gameStates.GS_loading: 
				break;
			default: break;
		}
	}
	
	public void OnPlay () {
		
		GUIStyle myStyle = new GUIStyle();
		myStyle.font = (Font)Resources.Load("Fonts/EIGH3");
		myStyle.normal.textColor = Color.white;
		myStyle.alignment = TextAnchor.MiddleCenter;
		GameObject.Find("GameCamera").GetComponent<ControlManagerCSharpV2>().DrawGUI();
	}
	
	public void OnPause () {
		float relativePosition = (float)Screen.height / 768.0f;
		switch (_menuState) {
			case menuStates.MS_start: 
				break;
			case menuStates.MS_levelSelection:
				LevelSelection();
				if (GUI.Button(new Rect(0, Screen.height - (int)(relativePosition*360.0f), (int)(relativePosition * 104.0f),  (int)(relativePosition * 77.0f)), ""))
					cameraPosition--;
				if (GUI.Button(new Rect((int)(Screen.width - (int)(relativePosition * 104.0f)), Screen.height - (int)(relativePosition*360.0f), (int)(relativePosition * 104.0f),  (int)(relativePosition * 77.0f)), ""))
					cameraPosition++;
				
				if (GUI.Button(new Rect((int)(Screen.width/2) - (relativePosition * 70.0f), Screen.height - (int)(relativePosition*160.0f), (int)(relativePosition * 140.0f),  (int)(relativePosition * 70.0f)), "next")) {
					AlignTargetWithObjectX(GameObject.Find("bodies_animated"), GameObject.Find("BodyAttacher"));
					Vector3 v = GameObject.Find("GameCamera").transform.position - GameObject.Find("bodies_animated").transform.position;
					v.x = v.z = 0.0f;
					GameObject.Find("bodies_animated").transform.LookAt(GameObject.Find("GameCamera").transform.position - v); 
					StartCoroutine(onSongSelection());
				}
				break;
			case menuStates.MS_pause: 
				if (GUI.Button(new Rect((int)(Screen.width/2) - (relativePosition * 70.0f), Screen.height - (int)(relativePosition*160.0f), (int)(relativePosition * 140.0f),  (int)(relativePosition * 70.0f)), "Play"))
					this.gameObject.GetComponent<GameManagerCSharpV1>().GoToPlay();
				break;
			default: break;
		}
	}
	
	IEnumerator onSongSelection () {
		string temp = _songPaths[selectedline];
		string temp2 = _songs[selectedline];
		this.gameObject.GetComponent<LoadManagerCSharpV1>().AddAudioClipToLoad(temp, 0, temp2);
		yield return StartCoroutine(this.gameObject.GetComponent<LoadManagerCSharpV1>().Init());
		_menuState++;
	}
	
	void AlignTargetWithObjectX (GameObject goTarget,GameObject objectx) {
		goTarget.transform.position = objectx.transform.position;
	}
	
	public void LevelSelection(){
			//~ GUI.skin = myskin;
		float relativePosition = (float)Screen.height / 768.0f;
		
		int leftpos = Screen.width/2 - 150;
		int toppos = (int)(relativePosition *50.0f);
		
		
		GUI.DrawTexture(new Rect(leftpos,toppos,300,60), GUIBackground);
		GUI.DrawTexture(new Rect((int)(Screen.width - (int)(relativePosition * 104.0f)), Screen.height - (int)(relativePosition*360.0f), (int)(relativePosition * 104.0f),  (int)(relativePosition * 77.0f)), leftarrow);
		GUI.DrawTexture(new Rect(0, Screen.height - (int)(relativePosition*360.0f), (int)(relativePosition * 104.0f),  (int)(relativePosition * 77.0f)), rightarrow);
		
		
		if (_menuState == GUIScriptCSharpV2.menuStates.MS_levelSelection) {
			GUI.color = Color.black;
			//----------------------------------------------------------------------------
			scrollViewVector = GUI.BeginScrollView(new Rect (leftpos, toppos+10, 300, 40), scrollViewVector, new Rect (0, 0, 300, (aantallijnen+2) * lineoffset));
			GUILayout.BeginArea( new Rect( 0, 0, 300, (aantallijnen+2) * lineoffset ) );
			for(int i =0; i<_songs.Length;++i){
				if(i == selectedline){
					GUI.color = Color.white;
				}
				GUI.Label(new Rect(0,i*lineoffset,300,20),_songs[i]);
				GUI.color = Color.black;
			}
			GUILayout.EndArea();    
			GUI.EndScrollView();
			//----------------------------------------------------------------------------
			
			GUI.color = Color.white;
			
			if(GUI.Button(new Rect(leftpos,toppos,300,10),"")){
				if(selectedline > 1)
					selectedline--;
			}
			if(GUI.Button(new Rect(leftpos,toppos + 50,300,10),"")){
				if(selectedline < aantallijnen)
					selectedline++;
			}
		}
	}
}
