using UnityEngine;
using System.Collections;
using System.Xml;

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
	public float timer = 0.0f;
	
	GameObject[] _stageLigthList;
	float[] _beatArray;
	
	public int _currentBeatNum = 0;
	
		Color lightColor;
	
	public string _currentLevelCameraObject;
	public string _beatXML = "http://howest.stage.eyebcom.com/DanceDance/relight_my_fire_beats.xml";
	
	//********FUNCTIONS********//
	void Start () {
		if (GameObject.Find("GameManager") == null) {
			GameObject GameManager;
			GameManager = new GameObject();
			GameManager.name = "GameManager";
			GameManager.transform.position = GameObject.Find("GameCamera").transform.position;
			GameManager.transform.parent = GameObject.Find("GameCamera").transform;
		}
		if (this.gameObject.name != "GameManager")
			GameManagerCSharpV1.Destroy(this);
		if (GameObject.Find("GameManager").GetComponent<GameManagerCSharpV1>() == null)
			GameObject.Find("GameManager").AddComponent<GameManagerCSharpV1>();
		if (GameObject.Find("GameManager").GetComponent<GUIScriptCSharpV2>() == null)
			GameObject.Find("GameManager").AddComponent<GUIScriptCSharpV2>();
		if (GameObject.Find("GameManager").GetComponent<ControlManagerCSharpV2>() == null)
			GameObject.Find("GameManager").AddComponent<ControlManagerCSharpV2>();
		if (GameObject.Find("GameManager").GetComponent<LoadManagerCSharpV1>() == null)
			GameObject.Find("GameManager").AddComponent<LoadManagerCSharpV1>();
		if (GameObject.Find("GameManager").GetComponent<AudioSource>() == null)
			GameObject.Find("GameManager").AddComponent<AudioSource>();
		if (this.gameObject.name == "GameManager") {
			GameObject morphTargetHolder = new GameObject();
			morphTargetHolder.name = "morphTargetHolder";
		}
		
		StartCoroutine(loadData());
		_stageLigthList = GameObject.FindGameObjectsWithTag("StageLight");
	}
	
	IEnumerator loadData () {
		WWW www = new WWW("http://howest.stage.eyebcom.com/DanceDance/MorphTargets/morphTargets.txt");
		yield return www;
		
		string textContent = www.data;
		char[] charsToTrim = {' ', '\n', '\t', '\r'};
		textContent.TrimEnd(charsToTrim);
		string[] morphTargets;
		morphTargets = textContent.Split("\n"[0]);
		
		www = new WWW("http://howest.stage.eyebcom.com/DanceDance/dancedance.xml");
		yield return www;
		XmlDocument gameXML = new XmlDocument();
		gameXML.LoadXml(www.data);
		XmlNodeList morphNodeList = gameXML.GetElementsByTagName("morph");
		
		morphTargets = new string[morphNodeList.Count];
		
		for (int i = 0; i < morphNodeList.Count; ++i) {
			XmlAttributeCollection attrCol = morphNodeList[i].Attributes;
			morphTargets[i] = attrCol["name"].InnerXml;
		}
		
		this.gameObject.GetComponent<LoadManagerCSharpV1>().Add3DObjectToLoad("http://howest.stage.eyebcom.com/DanceDance/MorphTargets/head_base.obj", "", "neutral");
		foreach (string item in morphTargets) {
			this.gameObject.GetComponent<LoadManagerCSharpV1>().AddMorphTargetToLoad("http://howest.stage.eyebcom.com/DanceDance/MorphTargets/" + item + ".obj", "neutral", item);
		}
		yield return StartCoroutine(this.gameObject.GetComponent<LoadManagerCSharpV1>().Init());
		
		GameObject.Find("neutral").GetComponent<MeshFilter>().mesh.RecalculateBounds();
		Bounds headBounds = GameObject.Find("neutral").GetComponent<MeshFilter>().mesh.bounds;
		Bounds bodyBounds = GameObject.Find("Male_Body_Disco").GetComponent<SkinnedMeshRenderer>().bounds;
		float scale = (headBounds.size.y / (headBounds.size.y * 5)) * bodyBounds.size.y;
		scale = 1/ (headBounds.size.y / scale);
		
		GameObject.Find("neutral").transform.localScale = new Vector3(scale, scale, scale);
		GameObject.Find("neutral").transform.position = GameObject.Find("GameCameraMenuHeadHolder").transform.position;
		GameObject.Find("neutral").transform.rotation = GameObject.Find("GameCameraMenuHeadHolder").transform.rotation;
		GameObject.Find("neutral").transform.parent = GameObject.Find("GameCameraMenuHeadHolder").transform;
		
		//~ Instantiate(GameObject.Find("neutral"), GameObject.Find("GameCameraMenuHeadHolder").transform.position, GameObject.Find("GameCameraMenuHeadHolder").transform.rotation);
		Instantiate(GameObject.Find("neutral"), GameObject.Find("Bip01 Head").transform.position, GameObject.Find("Bip01 Head").transform.rotation);
		GameObject.Find("neutral(Clone)").transform.parent = GameObject.Find("Bip01 Head").transform;
		GameObject.Find("neutral(Clone)").transform.localEulerAngles = new Vector3(270,270,180);
		GameObject.Find("neutral").layer = 8;
		
		www = new WWW(_beatXML);
		yield return www;
		XmlDocument myDoc = new XmlDocument();
		myDoc.LoadXml(www.data);
		XmlNodeList myList = myDoc.GetElementsByTagName("beat");
		_beatArray = new float[myList.Count];
		for (int i = 0; i < myList.Count; ++i) {
			_beatArray[i] = (float)System.Convert.ToDouble(myList[i].InnerXml.Replace('.',','));
		}
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
				OnLoad();
				break;
			default: 
				_gameState = gameStates.GS_paused;
				break;
		}
	}
	
	public void GoToPlay () {
		AlignTargetWithObjectX(GameObject.Find("CamLookat"), GameObject.Find(_currentLevelCameraObject));
		GameObject.Find("bodies_animated").transform.position = new Vector3(GameObject.Find(_currentLevelCameraObject).transform.position.x,
																								  GameObject.Find("bodies_animated").transform.position.y,
																								  GameObject.Find(_currentLevelCameraObject).transform.position.z);
		
		Vector3 v = GameObject.Find("GameCamera").transform.position - GameObject.Find("bodies_animated").transform.position;
		v.x = v.z = 0.0f;
		GameObject.Find("bodies_animated").transform.LookAt(GameObject.Find("GameCamera").transform.position - v); 
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
				foreach (GameObject item in _stageLigthList) {
					float fr = item.light.color.r + 0.16f;
					if (fr > 1.0f)
						fr = 0.0f;
					float fg = item.light.color.g + 0.32f;
					if (fg > 1.0f)
						fg = 0.0f;
					float fb = item.light.color.b + 0.8f;
					if (fb > 1.0f)
						fb = 0.0f;
					item.light.color = new Color(fr, fg, fb);
					item.light.enabled = false;
				}
			}
			_isBeatActive = false;
		}
		
		if (_isBeatActive) {
			foreach (GameObject item in _stageLigthList) {
				item.light.enabled = true;
			}
		}
		
		for (int i = 0; i < 5; ++i) {
			string s = "expression" + GameObject.Find("scorekeeper").GetComponent<ScoreBoxScriptCSharpV1>()._feverScore;
			Debug.Log(s);
			if (GameObject.Find("neutral").GetComponent<MorphTargetsCSharp>().attributes[i] == s) 
				GameObject.Find("neutral").GetComponent<MorphTargetsCSharp>().attributeProgress[i] = -0.05f;
			else
				GameObject.Find("neutral").GetComponent<MorphTargetsCSharp>().attributeProgress[i] = +0.05f;
		}
	}
	
	public void GoToPause () {
		GameState = gameStates.GS_paused;
	}
	
	void OnPause () {
		AlignTargetWithObjectX(GameObject.Find("CamLookat"), GameObject.Find("MenuCamPosition"));
		this.gameObject.GetComponent<AudioSource>().Pause();
	}
	
	public void GoToLoad () {
		GameState = gameStates.GS_loading;
	}
	
	void OnLoad () {
		AlignTargetWithObjectX(GameObject.Find("CamLookat"), GameObject.Find("MenuCamPosition"));
	}
	
	void AlignTargetWithObjectX (GameObject goTarget,GameObject objectx) {
		goTarget.transform.position = objectx.transform.position;
	}
}
