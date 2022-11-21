//****** HOW THIS STUFF WORKS ******//
/*
the idea is that you add any kind of object using it's appropriate function, and then call Init()
this will load all objects to the scene, destroy the loadmanager and assign a new loadmanager to the gamemanager
(just making sure nothing gets stuck)

The manager should be worked with ONLY as a plugin to your object! This means you don't need to touch this script,
just add the script to your object, call it through scripting, add the objects en initialize!

If there are objects you want to load that the loadmanager can't handle yet (it's, as is everything, a W.I.P.) just
add a new AddYourObject() function, overload it as you like, and place it at the appropriate place in the loading-
sequence of Init(). (the order in which the Init() function loads all the stuff is very important, e.g. you need to 
first make a mesh, and only after that add morphtargets.

Using this proved to be very handy once you figure out how to work with it.

W.I.P. 
As an extra, this script provides a loading-bar on a new thread. Just adjust the textures and _loadbarleftpixel and
_loadbarrightpixel (which are the coordinates of the loadingbar) to make your own loading screen.
*/
//**********************************//


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
	public int _loadbarProgress = 0;
	int _loadbarLeftPixel = 250;
	int _loadbarRightPixel = 780;
	
	public Texture _loadTxr_base;
	public Texture _loadTxr_bar;
	public Texture _background_color;
	
	// *********************************************** //
	// internal classes to represent each kind of object
	// *********************************************** //
	
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
	//**********************//
	// add object functions //
	//**********************//
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
	//**********************//
	// end object functions //
	//**********************//
	
	
	// use this to init once all object have been set ready to load
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
	
	void OnGUI () {
		if (this.gameObject.GetComponent<GameManagerCSharpV1>().GameState == GameManagerCSharpV1.gameStates.GS_loading ||
			this.gameObject.GetComponent<GUIScriptCSharpV2>()._menuState == GUIScriptCSharpV2.menuStates.MS_start) {
			if (_loadTxr_base != null && _loadTxr_bar != null && _background_color != null) {
				GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height), _background_color);
				
				float relativePosition = (float)Screen.width / _loadTxr_base.width;
				GUI.DrawTexture(new Rect(0,0,(relativePosition * _loadbarLeftPixel) + (relativePosition * _loadbarProgress),Screen.height), _loadTxr_bar);
				GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height), _loadTxr_base, ScaleMode.ScaleAndCrop);
			}
		}	
	}
	
	void AddLoadProgress() {
		int loadbarWidth = _loadbarRightPixel - _loadbarLeftPixel;
		
		if (_numObjectsLoaded - 1 < _maxObjectsToLoad)
			_numObjectsLoaded++;
		
		if (_numObjectsLoaded - 1 >= _maxObjectsToLoad) {
			this.gameObject.GetComponent<GameManagerCSharpV1>().UndoGameState();
	
			_maxObjectsToLoad = 0;
			_numObjectsLoaded = 1;

			_3DArrayList = new ArrayList();
			_2DArrayList = new ArrayList();
			_MorphArrayList = new ArrayList();
			_AudioArrayList = new ArrayList();
			//~ LoadManagerCSharpV1.Destroy(this);
			//~ this.gameObject.AddComponent<LoadManagerCSharpV1>();
		}	
		_loadbarProgress = (int)(((float)_numObjectsLoaded /(float) _maxObjectsToLoad) * (float)loadbarWidth);
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
		return noDups;
	}
	
	//function which creates a 3D gameobject by parameter
	IEnumerator CreateGameObject(string GOname, string meshPath, string ParentName) {
		GameObject GOTemp = new GameObject();
		GOTemp.name = GOname;
		if (ParentName != "") {
			GOTemp.transform.parent = GameObject.Find(ParentName).transform;
			GOTemp.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
		}
		GOTemp.AddComponent<objReaderCSharpV4>();

		GOTemp.GetComponent<objReaderCSharpV4>()._textFieldString = meshPath;
		yield return StartCoroutine(GOTemp.GetComponent<objReaderCSharpV4>().Init(GOname));
	}
	
	void AddMorphTarget(ref GameObject goTarget, ref GameObject goMorphParent) {
		if (goTarget.name != goMorphParent.name) {
			if (GameObject.Find("morphTargetHolder") != null) {
				goTarget.transform.parent = GameObject.Find("morphTargetHolder").transform;
			}
			else {
				GameObject morphTargetHolder = new GameObject();
				morphTargetHolder.name = "morphTargetHolder";
			}
			if (goMorphParent.GetComponent<MorphTargetsCSharp>() == null)
				goMorphParent.AddComponent<MorphTargetsCSharp>();
			goMorphParent.GetComponent<MorphTargetsCSharp>().sourceMesh = goMorphParent.GetComponent<MeshFilter>().mesh;
			goMorphParent.GetComponent<MorphTargetsCSharp>().AddBlendMesh(goTarget.name, goTarget.GetComponent<MeshFilter>().mesh);
			goTarget.renderer.enabled = false;
		}
	}
}
