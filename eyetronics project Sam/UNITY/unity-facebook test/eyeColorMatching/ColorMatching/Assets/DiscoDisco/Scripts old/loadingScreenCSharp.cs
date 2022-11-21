using UnityEngine;
using System.Collections;

public class loadingScreenCSharp : MonoBehaviour {
	
	public int _loadprogress = 0;
	int _oldLoadprogress = 1;
	int _loadmax = 700;
	
	public Texture2D _loading_mask;
	public Texture2D _loading_diffuse;
	public Texture2D _loading_forground;
	public Texture2D _loading_background;
	
	public string _loading_mask_string = "http://howest.stage.eyebcom.com/loading/disco_loading_mask.png";
	public string _loading_diffuse_string = "http://howest.stage.eyebcom.com/loading/disco_loading_bar.png";
	public string _loading_forground_string = "http://howest.stage.eyebcom.com/loading/disco_loading_top.png";
	public string _loading_background_string = "http://howest.stage.eyebcom.com/loading/disco_loading_base.png";
	
	public string[] _loadingtexts = {"dancemoves", "surroundsystem", "stage", "coolness", "the robot", "lederhosen", "tights", "superfly avatar", "controls"};
	int[] _loadingtextskeeper ;
	
	Texture2D _loading_barmasked;
	
	void Start () {
		_loadingtextskeeper = new int[_loadingtexts.Length];
		for (int i = 0; i < _loadingtextskeeper.Length; ++i)
			_loadingtextskeeper[i] = -1;
		StartCoroutine(AddProgress(19));
	}
	
	
	void OnGUI () {
		StartCoroutine(SomeFunctionFirstThread());
		if (GUI.Button(new Rect(10, 10, 100, 20), "click") && (_loadmax/100)*_oldLoadprogress < _loadmax) {
			StartCoroutine(AddProgress(19));
		}
		if ((_loadmax/100)*_loadprogress >= _loadmax)
		{	Debug.Log("no more");
		}
	}
	
	IEnumerator SomeFunctionFirstThread() {
		
		WWW txrWWW;
		GUITexture gTxr;
		
		if (GameObject.Find("LoadingForground") == null) {
			GameObject LoadingForground = new GameObject();
			LoadingForground.name = "LoadingForground";
			GameObject.Find("LoadingForground").AddComponent("GUITexture");
			txrWWW = new WWW(_loading_forground_string);
			yield return txrWWW;
			_loading_forground = txrWWW.texture;
			gTxr = (GUITexture)GameObject.Find("LoadingForground").GetComponent("GUITexture");
			gTxr.texture = _loading_forground;
			gTxr.pixelInset = new Rect(Screen.width/2, Screen.height/2, 0, 0);
		}
		
		if (GameObject.Find("LoadingBar") == null) {
			GameObject LoadingBar = new GameObject();
			LoadingBar.name = "LoadingBar";
			GameObject.Find("LoadingBar").AddComponent("GUITexture");
			txrWWW = new WWW(_loading_diffuse_string);
			yield return txrWWW;
			_loading_diffuse = txrWWW.texture;
			WWW txrWWWmask = new WWW(_loading_mask_string);
			yield return txrWWWmask;
			_loading_mask = txrWWWmask.texture;
		}
		if (_oldLoadprogress != _loadprogress && _loading_mask != null && _loading_diffuse != null) {
			gTxr = (GUITexture)GameObject.Find("LoadingBar").GetComponent("GUITexture");
			gTxr.texture = CreateUsingMaskAlpha(_loading_diffuse, _loading_mask);
			gTxr.pixelInset = new Rect(Screen.width/2, Screen.height/2, 0, 0);
			_loadprogress = _oldLoadprogress;
		}
			
		if (GameObject.Find("LoadingBackground") == null) {
			GameObject LoadingBackground = new GameObject();
			LoadingBackground.name = "LoadingBackground";
			GameObject.Find("LoadingBackground").AddComponent("GUITexture");
			txrWWW = new WWW(_loading_background_string);
			yield return txrWWW;
			_loading_background = txrWWW.texture;
			gTxr = (GUITexture)GameObject.Find("LoadingBackground").GetComponent("GUITexture");
			gTxr.texture = _loading_background;
			gTxr.pixelInset = new Rect(Screen.width/2, Screen.height/2, 0, 0);
		}
		
		yield return 0;
	}
	
	IEnumerator AddProgress(int loadprogress) {
		_oldLoadprogress+= loadprogress;
		
		int randomNmb = Random.Range(0, _loadingtexts.Length - 1);
		string temp = "loading\n" + _loadingtexts[randomNmb];
		
		GUIText gTxt = (GUIText)GameObject.Find("LoadingText").GetComponent("GUIText");
		gTxt.text = temp;
		
		if (_loadingtextskeeper[_loadingtextskeeper.Length - 1] == -1) {
			for (int i = 0; i < _loadingtexts.Length; ++i) {
				if (_loadingtextskeeper[i] == randomNmb) {
					randomNmb = Random.Range(0, _loadingtexts.Length - 1);
					i = 0;
				}
				else if (_loadingtextskeeper[i] == -1)
					break;
			}
			for (int i = 0; i < _loadingtexts.Length; ++i) {
				if (_loadingtextskeeper[i] == -1) {
					_loadingtextskeeper[i] = randomNmb;
					break;
				}
			}
		}
		
		string temptext = "";
		foreach (int item in _loadingtextskeeper)
			temptext = temptext + item;
		
		Debug.Log(temptext);
		
		yield return 0;
	}
	
	Texture2D CreateUsingMaskAlpha(Texture2D diffuse, Texture2D mask)
	{
		if (_loading_barmasked != null)
			Texture2D.Destroy(_loading_barmasked);
	   _loading_barmasked = new Texture2D(diffuse.width, diffuse.height, TextureFormat.ARGB32, false);
	   int offset = -(mask.width/2) + ((diffuse.width-_loadmax)/2) + ((_loadmax/100)*_loadprogress);
	   Color[] array = diffuse.GetPixels();
	   Color[] array2 = mask.GetPixels();

            for (int i = 0; i < diffuse.height; i++)
            {
                for (int j = 0; j < diffuse.width; j++)
                {
                    int t = j + (i * diffuse.width);
                    int t2 = j - offset + (i * mask.width);
					if (t2 < 0)
						array[t].a = 1;
					else if (t2 < array2.Length)
						array[t].a = array[t].a*array2[t2].a;
					else
						array[t].a = 0;
                }
            }
	   _loading_barmasked.SetPixels(array);
	   _loading_barmasked.Apply();

	   return _loading_barmasked;
	} 
}


