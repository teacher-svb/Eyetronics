/*
the game manager is made to build the game, using all the other classes. 
*/


using UnityEngine;
using System.Collections;
using System.Xml;
using System.Globalization;

public class GameManagerCSharpV1 : MonoBehaviour {
	//********   ENUMS   ********//
	public enum gameStates { GS_playing, GS_paused, GS_loading };
	
	//******** MEMBERS ********//
	gameStates _gameState = gameStates.GS_paused;
	gameStates _previousGameState;
	
	public gameStates GameState { get { return _gameState; } private set { _previousGameState = GameState; _gameState = value; } }
	public void UndoGameState () { GameState = _previousGameState; }
	
	public bool _isBeatActive = false;
	
	public int _tempo;
	public int _points = 0;
	public int _currentBeatNum = 0;
	
	public float timer = 0.0f;
	float[] _beatArray;
	
	Color lightColor;
	
	public string _currentLevelCameraObject;
	public string _beatXML = "http://howest.stage.eyebcom.com/DanceDance/relight_my_fire_beats.xml";
	public string _characterMesh = "";
	
	//********FUNCTIONS********//
	void Start () {
			StartCoroutine(loadData());
	}
	
	// function to load all necessary data
	IEnumerator loadData () {
		yield return StartCoroutine(GameObject.Find("GameCamera").GetComponent<GUIScriptCSharpV2>().loadData());
		string[] morphTargets;
		WWW www = new WWW("http://howest.stage.eyebcom.com/DanceDance/dancedance.xml");
		yield return www;
		XmlDocument gameXML = new XmlDocument();
		gameXML.LoadXml(www.data);
		XmlNodeList morphNodeList = gameXML.GetElementsByTagName("morph");
		
		morphTargets = new string[morphNodeList.Count];
		
		for (int i = 0; i < morphNodeList.Count; ++i) {
			XmlAttributeCollection attrCol = morphNodeList[i].Attributes;
			morphTargets[i] = attrCol["name"].InnerXml;
		}
		// loading the basemesh for the head
		this.gameObject.GetComponent<LoadManagerCSharpV1>().Add3DObjectToLoad("http://howest.stage.eyebcom.com/DanceDance/MorphTargets/neutral.obj", "", "neutral");
		// loading the morphtargets
		foreach (string item in morphTargets) {
			this.gameObject.GetComponent<LoadManagerCSharpV1>().AddMorphTargetToLoad("http://howest.stage.eyebcom.com/DanceDance/MorphTargets/" + item + ".obj", "neutral", item);
		}
		//init loading
		yield return StartCoroutine(this.gameObject.GetComponent<LoadManagerCSharpV1>().Init());
		
		//get the beats from the song
		www = new WWW(_beatXML);
		yield return www;
		XmlDocument myDoc = new XmlDocument();
		myDoc.LoadXml(www.data);
		XmlNodeList myList = myDoc.GetElementsByTagName("beat");
		_beatArray = new float[myList.Count];
		for (int i = 0; i < myList.Count; ++i) {
			_beatArray[i] = (float)System.Convert.ToDouble(myList[i].InnerXml, CultureInfo.InvariantCulture);
		}
		
		GameObject.Find("neutral").GetComponent<MeshFilter>().mesh.RecalculateBounds();
		Bounds headBounds = GameObject.Find("neutral").GetComponent<MeshFilter>().mesh.bounds;
		Bounds bodyBounds = GameObject.Find("Male_Body_Disco").GetComponent<SkinnedMeshRenderer>().bounds;
		float scale = (headBounds.size.y / (headBounds.size.y * 5)) * bodyBounds.size.y;
		scale = 1/ (headBounds.size.y / scale);
		
		GameObject.Find("neutral").transform.localScale = new Vector3(scale, scale, scale);
		GameObject.Find("neutral").transform.position = GameObject.Find("GameCameraMenuHeadHolder").transform.position;
		GameObject.Find("neutral").transform.rotation = GameObject.Find("GameCameraMenuHeadHolder").transform.rotation;
		GameObject.Find("neutral").transform.parent = GameObject.Find("GameCameraMenuHeadHolder").transform;
		
		Instantiate(GameObject.Find("neutral"), GameObject.Find("Bip01 Head").transform.position, GameObject.Find("Bip01 Head").transform.rotation);
		GameObject.Find("neutral(Clone)").transform.parent = GameObject.Find("Bip01 Head").transform;
		GameObject.Find("neutral(Clone)").transform.localEulerAngles = new Vector3(270,270,180);
		GameObject.Find("neutral").layer = 8;
		
		GameObject.Find("Director").GetComponent<PHPHandler>().Init();
		yield return new WaitForSeconds( 1 );
		gameObject.GetComponent<GUIScriptCSharpV2>()._menuState++;
	}
	
	void Update () {
		switch (_gameState) {
			case gameStates.GS_playing: 
				timer += Time.deltaTime;
				OnPlay();
				break;
			case gameStates.GS_paused:
				OnPause();
				break;
			case gameStates.GS_loading: 
				break;
			default: 
				_gameState = gameStates.GS_paused;
				break;
		}
	}
	
	public void GoToPlay () {
		GameState = gameStates.GS_playing;
		this.gameObject.GetComponent<AudioSource>().Play();
	}
	
	void OnPlay () {
		GameObject.Find("neutral").GetComponent<MorphTargetsCSharp>().SetMorph();
		
		if (timer < _beatArray[_currentBeatNum] + 0.1f && timer > _beatArray[_currentBeatNum] - 0.1f) {
			_isBeatActive = true;
		}
		else {
			if (_isBeatActive) {
				_currentBeatNum++;
					GameObject.Find("Underwater Podium").GetComponent<PodiumScript>().ChangeTexture();
					GameObject.Find("City Podium").GetComponent<PodiumScript>().ChangeTexture();
					GameObject.Find("Sunset Podium").GetComponent<PodiumScript>().ChangeTexture();
					GameObject.Find("Jungle Podium").GetComponent<PodiumScript>().ChangeTexture();
			}
			_isBeatActive = false;
		}
		
		for (int i = 0; i < GameObject.Find("neutral").GetComponent<MorphTargetsCSharp>().attributeProgress.Length; ++i) {
			string s = "expression" + GameObject.Find("scorekeeper").GetComponent<ScoreBoxScriptCSharpV1>()._feverScore;
			Debug.Log(s);
			if (GameObject.Find("neutral").GetComponent<MorphTargetsCSharp>().attributes[i] == s || GameObject.Find("neutral").GetComponent<MorphTargetsCSharp>().attributes[i] == "character") 
				GameObject.Find("neutral").GetComponent<MorphTargetsCSharp>().attributeProgress[i] = -0.05f;
			else
				GameObject.Find("neutral").GetComponent<MorphTargetsCSharp>().attributeProgress[i] = +0.05f;
		}
		if (_currentBeatNum+1 >= _beatArray.Length) {
			_gameState = gameStates.GS_paused;
			gameObject.GetComponent<GUIScriptCSharpV2>()._menuState = GUIScriptCSharpV2.menuStates.MS_end;
		}
	}
	
	public void GoToPause () {
		GameState = gameStates.GS_paused;
	}
	
	void OnPause () {
		if (GameObject.Find("character") != null) {
			for (int i = 0; i < GameObject.Find("neutral").GetComponent<MorphTargetsCSharp>().attributes.Length; ++i) {
				if (GameObject.Find("neutral").GetComponent<MorphTargetsCSharp>().attributes[i] == "character")
					GameObject.Find("neutral").GetComponent<MorphTargetsCSharp>().attributeProgress[i] = -1.01f;
			}
			GameObject.Find("neutral").GetComponent<MorphTargetsCSharp>().SetMorph();
		}
		this.gameObject.GetComponent<AudioSource>().Pause();
	}
	
	public void GoToLoad () {
		GameState = gameStates.GS_loading;
	}
}
