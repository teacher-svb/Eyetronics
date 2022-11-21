using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoadManagerCSharpV1 : MonoBehaviour {

	ArrayList _3DArrayList = new ArrayList();
	ArrayList _2DArrayList = new ArrayList();
	ArrayList _MorphArrayList = new ArrayList();
	ArrayList _AudioArrayList = new ArrayList();
	
	public int _maxObjectsToLoad = 0;
	public int _numObjectsLoaded = 1;
	
	Texture _loadTxr_base;
	Texture _loadTxr_bar;
	public Texture _background_color;
	
	int _loadbarLeftPixel = 211;
	int _loadbarRightPixel = 811;
	public int _loadbarProgress = 0;
	
	void Start () {
		StartCoroutine(LoadData());
	}
	
	IEnumerator LoadData () {
		WWW www = new WWW("http://howest.stage.eyebcom.com/DanceDance/Interface/loading/disco_loading_base.png");
		yield return www;
		_loadTxr_base = www.texture;
		
		www = new WWW("http://howest.stage.eyebcom.com/DanceDance/Interface/loading/disco_loading_bar.png");
		yield return www;
		_loadTxr_bar = www.texture;
		
		www = new WWW("http://howest.stage.eyebcom.com/DanceDance/Interface/loading/pixel.png");
		yield return www;
		_background_color = www.texture;
	}
	
    internal class MorphTargetToLoad {
		public MorphTargetToLoad(string PathToLoadFrom, string ParentName, string ObjectName) {
			_objectName = ObjectName;
			_pathToLoadFrom = PathToLoadFrom;
			_morphParentName = ParentName;
		}
		public string _objectName;
		public string _pathToLoadFrom;
		public string _morphParentName;
    }
	
    internal class AudioClipToLoad {
		public AudioClipToLoad(string PathToLoadFrom, int tempo, string AudioName) {
			_audioName = AudioName;
			_pathToLoadFrom = PathToLoadFrom;
			GameObject.Find("GameManager").GetComponent<GameManagerCSharpV1>()._tempo = tempo;
		}
		public string _audioName;
		public string _pathToLoadFrom;
    }
	
    internal class Object3DToLoad {
		public Object3DToLoad(string PathToLoadFrom, string ParentName, string ObjectName) {
			_objectName = ObjectName;
			_pathToLoadFrom = PathToLoadFrom;
			_parentName = ParentName;
		}
		public string _objectName;
		public string _pathToLoadFrom;
		public string _parentName;
    }
	
    internal class Object2DToLoad {
		public Object2DToLoad(string PathToLoadFrom, ref Texture txr) {
			_txr = txr;
			_pathToLoadFrom = PathToLoadFrom;
		}
		public Texture _txr;
		public string _pathToLoadFrom;
    }
	
	void OnGUI () {
		if (this.gameObject.GetComponent<GameManagerCSharpV1>().GameState == GameManagerCSharpV1.gameStates.GS_loading) {
			GUI.Label(new Rect(10,10,100,20), "loading");
			if (_loadTxr_base != null && _loadTxr_bar != null && _background_color != null) {
				GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height), _background_color);
				
				float relativePosition = (float)Screen.width / _loadTxr_base.width;
				GUI.DrawTexture(new Rect(0,0,(relativePosition * _loadbarLeftPixel) + (relativePosition * _loadbarProgress),Screen.height), _loadTxr_bar);
				GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height), _loadTxr_base, ScaleMode.ScaleAndCrop);
			}
		}	
	}
	
	public IEnumerator Init () {
		this.gameObject.GetComponent<GameManagerCSharpV1>().GoToLoad();
		ArrayList morphParentsToInit = new ArrayList();
		foreach (Object2DToLoad item in _2DArrayList) {
			WWW www = new WWW(item._pathToLoadFrom);
			yield return www;
			item._txr = www.texture;
			AddLoadProgress();
		}
		foreach (AudioClipToLoad item in _AudioArrayList) {
			WWW www = new WWW(item._pathToLoadFrom);
			yield return www;
			if (this.gameObject.GetComponent<AudioSource>() == null || this.gameObject.GetComponent<AudioSource>().clip != null)
				this.gameObject.AddComponent<AudioSource>();
			this.gameObject.GetComponent<AudioSource>().clip = www.audioClip;
			this.gameObject.GetComponent<AudioSource>().clip.name = item._audioName;
			AddLoadProgress();
		}
		foreach (Object3DToLoad item in _3DArrayList) {
			yield return StartCoroutine(CreateGameObject(item._objectName, item._pathToLoadFrom, item._parentName));
			AddLoadProgress();
		}
		foreach (MorphTargetToLoad item in _MorphArrayList) {
			yield return StartCoroutine(CreateGameObject(item._objectName, item._pathToLoadFrom, "morphTargetHolder"));
			GameObject go1;
			GameObject go2;
			go1 = GameObject.Find(item._objectName);
			go1.name = item._objectName;
			//~ Debug.Log(item._objectName);
			go2 = GameObject.Find(item._morphParentName);
			go2.name = item._morphParentName;
			AddMorphTarget(ref go1 , ref go2 );
			morphParentsToInit.Add(item._morphParentName);
			AddLoadProgress();
		}
		foreach (string item in RemoveDups(morphParentsToInit)) {
			GameObject.Find(item).GetComponent<MorphTargetsCSharp>().Init();
		}
	}
	
	public ArrayList RemoveDups(ArrayList items)
	{
		ArrayList noDups = new ArrayList();
		foreach(string strItem in items)
		{
			if (!noDups.Contains(strItem.Trim()))
			{
				noDups.Add(strItem.Trim());
			}
		}
		//~ noDups.Sort();
		return noDups;
	}
	
	IEnumerator CreateGameObject(string GOname, string meshPath, string ParentName) {
		GameObject GOTemp = new GameObject();
		GOTemp.name = GOname;
		if (ParentName != "")
			GOTemp.transform.parent = GameObject.Find(ParentName).transform;
		GOTemp.AddComponent<objReaderCSharpV4>();

		GOTemp.GetComponent<objReaderCSharpV4>()._textFieldString = meshPath;
		GOTemp.GetComponent<objReaderCSharpV4>()._textureLink = "http://howest.stage.eyebcom.com/DanceDance/Users/dimitri.jpg";
		yield return StartCoroutine(GOTemp.GetComponent<objReaderCSharpV4>().Init(GOname));
//-			Debug.Log("object loaded");
//		AddMorphTarget(ref GOTemp);
		GOTemp.transform.parent = GameObject.Find("morphTargetHolder").transform;
	}
	
	void AddMorphTarget(ref GameObject goTarget, ref GameObject goMorphParent) {
		if (goTarget.name != goMorphParent.name) {
			if (goMorphParent.GetComponent<MorphTargetsCSharp>() == null)
				goMorphParent.AddComponent<MorphTargetsCSharp>();
			goMorphParent.GetComponent<MorphTargetsCSharp>().sourceMesh = goMorphParent.GetComponent<MeshFilter>().mesh;
			goMorphParent.GetComponent<MorphTargetsCSharp>().AddBlendMesh(goTarget.name, goTarget.GetComponent<MeshFilter>().mesh);
			//~ Debug.Log(goTarget.name);
			goTarget.renderer.enabled = false;
		}
	}
	
	void AddLoadProgress() {
		int loadbarWidth = _loadbarRightPixel - _loadbarLeftPixel;
		
		if (_numObjectsLoaded - 1 < _maxObjectsToLoad)
			_numObjectsLoaded++;
		
		if (_numObjectsLoaded - 1 >= _maxObjectsToLoad) {
			this.gameObject.GetComponent<GameManagerCSharpV1>().UndoGameState();
			LoadManagerCSharpV1.Destroy(this);
			this.gameObject.AddComponent<LoadManagerCSharpV1>();
		}	
		_loadbarProgress = (int)(((float)_numObjectsLoaded /(float) _maxObjectsToLoad) * (float)loadbarWidth);
	}
	
	public void AddAudioClipToLoad(string PathToLoadFrom, int tempo, string AudioName) {
		_AudioArrayList.Add(new AudioClipToLoad(PathToLoadFrom, tempo, AudioName));
		_maxObjectsToLoad++;
	}
	
	public void AddAudioClipToLoad(string PathToLoadFrom, int tempo) {
		_AudioArrayList.Add(new AudioClipToLoad(PathToLoadFrom, tempo, "new audio clip"));
		_maxObjectsToLoad++;
	}
	
	public void AddMorphTargetToLoad(string PathToLoadFrom, string MorphParentName, string ObjectName) {
		_MorphArrayList.Add(new MorphTargetToLoad(PathToLoadFrom,MorphParentName,ObjectName));
		_maxObjectsToLoad++;
	}
	
	public void AddMorphTargetToLoad(string PathToLoadFrom, string MorphParentName) {
		_MorphArrayList.Add(new MorphTargetToLoad(PathToLoadFrom,MorphParentName,"new morph target"));
		_maxObjectsToLoad++;
	}
	
	public void Add3DObjectToLoad(string PathToLoadFrom, string ParentName, string ObjectName) {
		_3DArrayList.Add(new Object3DToLoad(PathToLoadFrom,ParentName,ObjectName));
		_maxObjectsToLoad++;
	}
	
	public void Add3DObjectToLoad(string PathToLoadFrom, string ParentName) {
		_3DArrayList.Add(new Object3DToLoad(PathToLoadFrom,ParentName,"new 3D object"));
		_maxObjectsToLoad++;
	}
	
	public void Add3DObjectToLoad(string PathToLoadFrom) {
		_3DArrayList.Add(new Object3DToLoad(PathToLoadFrom,"","new 3D object"));
		_maxObjectsToLoad++;
	}
	
	public void Add2DObjectToLoad(string PathToLoadFrom, ref Texture txr) {
		_2DArrayList.Add(new Object2DToLoad(PathToLoadFrom, ref txr));
		_maxObjectsToLoad++;
	}
}
