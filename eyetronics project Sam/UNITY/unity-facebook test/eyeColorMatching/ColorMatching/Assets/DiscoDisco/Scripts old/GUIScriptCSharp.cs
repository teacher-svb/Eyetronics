using UnityEngine;
using System.Collections;

public class GUIScriptCSharp : MonoBehaviour {

	public string _textFieldString = "http://howest.stage.eyebcom.com/dimitri.obj";
	public string _textureLink = "http://howest.stage.eyebcom.com/sam.jpg";
	
	GameObject GOSource_RT;
	GameObject GOTarget_RT;
	
	
	GameObject[] GOArrayTargets = new GameObject[16];
	
	float[] _sliderValue = new float[16];
	
	bool _isLoading;

	
	void OnGUI () {
		if (!_isLoading) {
			for (int i = 0; i < GOArrayTargets.Length; ++i) {
				if (GOArrayTargets[i] != null) {
					_sliderValue[i] = GUI.HorizontalSlider( new Rect(10, (i*60)+10, 200, 30), _sliderValue[i], -0.005f, 0.005f);
					MorphTargetsCSharp MorphersTemp = (MorphTargetsCSharp)GOSource_RT.GetComponent("MorphTargetsCSharp");
					MorphersTemp.attributeProgress[i] = _sliderValue[i];
					GUI.Label(new Rect(220, (i*60)+10, 200, 30), "Progress value:           " + MorphersTemp.attributeProgress[i]);
//					GUI.Label(new Rect(220, (i*60)+30, 200, 30), "ProgressCounter value: " + MorphersTemp.attributeProgressCounter[i]);
					MorphersTemp.SetMorph();
				}
			}
		}
		else {
//			StartCoroutine(loading());
			for (int i = 0; i < _sliderValue.Length; ++i)
				_sliderValue[i] = 0.0f;
		}
		StartCoroutine(SomeFunction());
	}

	IEnumerator loading() {
		WWW txrWWW;
		GUITexture gTxr;
		if (GameObject.Find("LoadingBackground") == null) {
			GameObject LoadingBackground = new GameObject();
			LoadingBackground.name = "LoadingBackground";
			GameObject.Find("LoadingBackground").AddComponent("GUITexture");
			txrWWW = new WWW("http://howest.stage.eyebcom.com/loading/disco_loading_base.png");
			yield return txrWWW;
			gTxr = (GUITexture)GameObject.Find("LoadingBackground").GetComponent("GUITexture");
			gTxr.texture = txrWWW.texture;
		}
		
		if (GameObject.Find("LoadingBar") == null) {
			GameObject LoadingBar = new GameObject();
			LoadingBar.name = "LoadingBar";
			GameObject.Find("LoadingBar").AddComponent("GUITexture");
			txrWWW = new WWW("http://howest.stage.eyebcom.com/loading/disco_loading_bar.png");
			yield return txrWWW;
			WWW txrWWWmask = new WWW("http://howest.stage.eyebcom.com/loading/disco_loading_mask.png");
			yield return txrWWWmask;
			gTxr = (GUITexture)GameObject.Find("LoadingBar").GetComponent("GUITexture");
			gTxr.texture = CreateUsingMaskAlpha(txrWWW.texture, txrWWWmask.texture);
		}
		
		if (GameObject.Find("LoadingForground") == null) {
			GameObject LoadingForground = new GameObject();
			LoadingForground.name = "LoadingForground";
			GameObject.Find("LoadingForground").AddComponent("GUITexture");
			txrWWW = new WWW("http://howest.stage.eyebcom.com/loading/disco_loading_top.png");
			yield return txrWWW;
			gTxr = (GUITexture)GameObject.Find("LoadingForground").GetComponent("GUITexture");
			gTxr.texture = txrWWW.texture;
		}
		GameObject.Find("LoadingForground").guiTexture.pixelInset = new Rect(-Screen.width/2, -Screen.height/2, Screen.width, Screen.height);
		GameObject.Find("LoadingBar").guiTexture.pixelInset = new Rect(-Screen.width/2, -Screen.height/2, Screen.width, Screen.height);
		GameObject.Find("LoadingBackground").guiTexture.pixelInset = new Rect(-Screen.width/2, -Screen.height/2, Screen.width, Screen.height);
		
		
		GameObject.Find("Loadingscreen").guiTexture.pixelInset = new Rect(-Screen.width/2, -Screen.height/2, Screen.width, Screen.height);
	}
	
	Texture2D CreateUsingMaskAlpha(Texture2D diffuse, Texture2D mask)
	{
	   Texture2D result = new Texture2D(diffuse.width, diffuse.height, TextureFormat.ARGB32, false);
	   
	   Color[] diffuseColors = diffuse.GetPixels();
	   Color[] maskColors = mask.GetPixels();

	   for(int i = 0; i < diffuseColors.Length; i++)
	   {
		  diffuseColors[i].a *= maskColors[i].a;
		   diffuseColors[i].a = Mathf.Sqrt(diffuseColors[i].a);
	   }
		  
	   result.SetPixels(diffuseColors);
	   result.Apply();

	   return result;
	} 
	
	IEnumerator SomeFunction() {
			GameObject.Find("Loadingscreen").guiTexture.pixelInset = new Rect(Screen.width*1000, Screen.height*1000, 0, 0);

			if (GUI.Button(new Rect(300,70,100,20), "Click"))
			{
				_isLoading = true;
				yield return StartCoroutine(CreateGameObject("dimitri", "dimitri", false));
				Debug.Log("target added");
				yield return StartCoroutine(CreateGameObject("target blink_Left", "blink_Left", true));
				Debug.Log("target added");
				yield return StartCoroutine(CreateGameObject("target blink_Right", "blink_Right", true));
				Debug.Log("target added");
				yield return StartCoroutine(CreateGameObject("target sad", "sad", true));
				Debug.Log("target added");
				yield return StartCoroutine(CreateGameObject("target scared", "scared", true));
				Debug.Log("target added");
				yield return StartCoroutine(CreateGameObject("target smile", "smile", true));
				Debug.Log("target added");
				yield return StartCoroutine(CreateGameObject("source", "sam", true));
				Debug.Log("source added");
				Debug.Log("gathering targets...");
				
				
				MorphTargetsCSharp MorphersTemp = (MorphTargetsCSharp)GOSource_RT.GetComponent("MorphTargetsCSharp");
				if (MorphersTemp == null) {
					GOSource_RT.AddComponent("MorphTargetsCSharp");
					MorphersTemp = (MorphTargetsCSharp)GOSource_RT.GetComponent("MorphTargetsCSharp");
				}
				MeshFilter MFTemp = (MeshFilter)GOSource_RT.GetComponent("MeshFilter");
				MorphersTemp.sourceMesh = MFTemp.mesh;
				 
				for (int i = 0; i < GOArrayTargets.Length; ++i) {
					if (GOArrayTargets[i] != null)
						StartCoroutine(AddMorphTarget(i));
					else
						break;
				}
				//~ MorphersTemp.Init();
				
				_isLoading = false;
			}
	}
	
	IEnumerator AddMorphTarget(int counter) {
		MeshFilter MFTemp = (MeshFilter)GOArrayTargets[counter].GetComponent("MeshFilter");
		MorphTargetsCSharp MorphersTemp = (MorphTargetsCSharp)GOSource_RT.GetComponent("MorphTargetsCSharp");
		
		MorphersTemp.AddBlendMesh(GOArrayTargets[counter].name, MFTemp.mesh);
		GOArrayTargets[counter].renderer.enabled = false;
		yield return 0;
	}
	
	void AddMorphTargetToSource() {
		if ((MorphTargetsCSharp)GOSource_RT.GetComponent("MorphTargetsCSharp") == null)
			GOSource_RT.AddComponent("MorphTargetsCSharp");
		MorphTargetsCSharp MorphersTemp = (MorphTargetsCSharp)GOSource_RT.GetComponent("MorphTargetsCSharp");
		MeshFilter MFTemp = (MeshFilter)GOSource_RT.GetComponent("MeshFilter");
		MorphersTemp.sourceMesh = MFTemp.mesh;
		
		for (int i = 0; i < GOArrayTargets.Length; ++i) {
			if (GOArrayTargets[i] != null) {
				MFTemp = (MeshFilter)GOArrayTargets[i].GetComponent("MeshFilter");
				
				MorphersTemp.AddBlendMesh(MFTemp.mesh.name, MFTemp.mesh);
				//~ MorphersTemp.Init();
				GOArrayTargets[i].renderer.enabled = false;
			}
		}
		_isLoading = false;
	}
	
	IEnumerator CreateGameObject(string GOname, string meshName, bool isTarget) {
		GameObject GOTemp = new GameObject();
		GOTemp.name = GOname;
		GOTemp.AddComponent("objReaderCSharpV3");
		objReaderCSharpV3 objReaderTemp = (objReaderCSharpV3)GOTemp.GetComponent("objReaderCSharpV3");
		objReaderTemp._textFieldString = "http://howest.stage.eyebcom.com/" + meshName + ".obj";
		if (!isTarget)
			objReaderTemp._textureLink = "http://howest.stage.eyebcom.com/sam.jpg";
		yield return StartCoroutine(objReaderTemp.SomeFunction());
		if (isTarget) {
			for (int i = 0; i < GOArrayTargets.Length; ++i) {
				if (GOArrayTargets[i] == null) {
					GOArrayTargets[i] = GOTemp;
					break;
				}
			}
		}
		else
			GOSource_RT = GOTemp;
	}
}
