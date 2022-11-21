using UnityEngine;
using System.Collections;
using System.Xml;

public class GUIScriptCSharpV2 : MonoBehaviour {
	
	public enum menuStates { MS_start, MS_levelSelection, MS_songSelection, MS_pause };
	public menuStates _menuState = menuStates.MS_start;
	
	string _pathTogameXML = "http://howest.stage.eyebcom.com/DanceDance/dancedance.xml";
	string _pathToLevellist = "http://howest.stage.eyebcom.com/DanceDance/Levels/levels.txt";
	
	string[] _levels;
	string[] _levelImagePaths;
	string[] _songs;
	string[] _songPaths;
	string[] _songTempos;
	
	public int selectionGridLevel = 0;
	public int selectionGridSong = 0;
	
	void Start () {
		
		StartCoroutine(loadData());
	}
		
	IEnumerator loadData () {
		WWW www = new WWW(_pathToLevellist);
		yield return www;
		
		string textContent = www.data;
		char[] charsToTrim = {' ', '\n', '\t', '\r'};
		textContent.TrimEnd(charsToTrim);
		
		_levels = textContent.Split("\n"[0]);
		
		www = new WWW(_pathTogameXML);
		yield return www;
		XmlDocument gameXML = new XmlDocument();
		gameXML.LoadXml(www.data);
		XmlNodeList songNodeList = gameXML.GetElementsByTagName("song");
		
		_songs = new string[songNodeList.Count];
		_songPaths = new string[songNodeList.Count];
		_songTempos = new string[songNodeList.Count];
		
		for (int i = 0; i < songNodeList.Count; ++i) {
			XmlAttributeCollection attrCol = songNodeList[i].Attributes;
			_songs[i] = attrCol["name"].InnerXml;
			_songPaths[i] = attrCol["link"].InnerXml;
			_songTempos[i] = attrCol["tempo"].InnerXml;
		}
		
		XmlNodeList levelNodeList = gameXML.GetElementsByTagName("level");
		
		_levels = new string[levelNodeList.Count];
		_levelImagePaths = new string[levelNodeList.Count];
		
		for (int i = 0; i < levelNodeList.Count; ++i) {
			XmlAttributeCollection attrCol = levelNodeList[i].Attributes;
			_levels[i] = attrCol["name"].InnerXml;
			_levelImagePaths[i] = attrCol["image"].InnerXml;
		}
	}		
	
	void OnGUI () {
		switch (this.gameObject.GetComponent<GameManagerCSharpV1>().GameState) {
			case GameManagerCSharpV1.gameStates.GS_playing: 
				OnPlay();
				break;
			case GameManagerCSharpV1.gameStates.GS_paused:
				OnPause();
				break;
			case GameManagerCSharpV1.gameStates.GS_loading: 
				OnLoad();
				break;
			default: break;
		}
	}
	
	public void OnPlay () {
		GUI.Label(new Rect(10,10,100,20), "play");
		
		GUIStyle myStyle = new GUIStyle();
		myStyle.font = (Font)Resources.Load("Fonts/EIGH3");
		myStyle.normal.textColor = Color.white;
		myStyle.alignment = TextAnchor.MiddleCenter;
	}
	
	public void OnLoad () {
		GUI.Label(new Rect(10,10,100,20), "loading");
	}
	
	public void OnPause () {
		float relativePosition = (float)Screen.height / 768.0f;
		GUI.Label(new Rect(10,10,100,20), "paused");
		switch (_menuState) {
			case menuStates.MS_start: 
				if (GUI.Button(new Rect((int)(Screen.width/2) - (relativePosition * 70.0f), Screen.height - (int)(relativePosition*160.0f), (int)(relativePosition * 140.0f),  (int)(relativePosition * 70.0f)), "Play"))
					_menuState++;
				break;
			case menuStates.MS_levelSelection:
				selectionGridLevel = GUI.SelectionGrid(new Rect(25,25,Screen.width - 50, Screen.height - 150), selectionGridLevel, _levels, (int)((float)_levels.Length / 5.1f) + 1);
				if (GUI.Button(new Rect((int)(Screen.width/2) - (relativePosition * 70.0f), Screen.height - (int)(relativePosition*160.0f), (int)(relativePosition * 140.0f),  (int)(relativePosition * 70.0f)), "Go to Song Selection")) {
					this.gameObject.GetComponent<GameManagerCSharpV1>()._currentLevelCameraObject = "level" + _levels[selectionGridLevel] + "Position";
					_menuState++;
				}
				break;
			case menuStates.MS_songSelection:
				selectionGridSong = GUI.SelectionGrid(new Rect(25,25,Screen.width - 50, Screen.height - 150), selectionGridSong, _songs, (int)((float)_songs.Length / 5.1f) + 1);
				
				if (GUI.Button(new Rect((int)(Screen.width/2) - (relativePosition * 70.0f), Screen.height - (int)(relativePosition*160.0f), (int)(relativePosition * 140.0f),  (int)(relativePosition * 70.0f)), "Dance!")) {
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
		this.gameObject.GetComponent<LoadManagerCSharpV1>().AddAudioClipToLoad(_songPaths[selectionGridSong], System.Convert.ToInt32(_songTempos[selectionGridSong]), _songs[selectionGridSong]);
		yield return StartCoroutine(this.gameObject.GetComponent<LoadManagerCSharpV1>().Init());
		_menuState++;
	}
}
