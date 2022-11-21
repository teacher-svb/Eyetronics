using UnityEngine;
using System.Collections;

public class GUITester : MonoBehaviour {

	int loadprogress = 0;
	int loadmax = 600;
	
	public Texture2D _loading_mask;
	public Texture2D _loading_diffuse;
	public Texture2D _loading_forground;
	public Texture2D _loading_background;
	Texture2D _loading_barmasked;
	
	
	void OnGUI () {
		StartCoroutine(SomeFunctionFirstThread());
		StartCoroutine(SomeFunctionSecondThread());
	}
	
	IEnumerator SomeFunctionFirstThread() {
		
		WWW txrWWW;
		GUITexture gTxr;
		
		if (GameObject.Find("LoadingForground") == null) {
			GameObject LoadingForground = new GameObject();
			LoadingForground.name = "LoadingForground";
			GameObject.Find("LoadingForground").AddComponent("GUITexture");
			txrWWW = new WWW("http://howest.stage.eyebcom.com/loading/disco_loading_top.png");
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
			txrWWW = new WWW("http://howest.stage.eyebcom.com/loading/disco_loading_bar.png");
			yield return txrWWW;
			_loading_diffuse = txrWWW.texture;
			WWW txrWWWmask = new WWW("http://howest.stage.eyebcom.com/loading/disco_loading_mask.png");
			yield return txrWWWmask;
			_loading_mask = txrWWWmask.texture;
		}
		if (_loading_mask != null && _loading_diffuse != null) {
			gTxr = (GUITexture)GameObject.Find("LoadingBar").GetComponent("GUITexture");
			gTxr.texture = CreateUsingMaskAlpha(_loading_diffuse, _loading_mask);
			gTxr.pixelInset = new Rect(Screen.width/2, Screen.height/2, 0, 0);
		}
			
		if (GameObject.Find("LoadingBackground") == null) {
			GameObject LoadingBackground = new GameObject();
			LoadingBackground.name = "LoadingBackground";
			GameObject.Find("LoadingBackground").AddComponent("GUITexture");
			txrWWW = new WWW("http://howest.stage.eyebcom.com/loading/disco_loading_base.png");
			yield return txrWWW;
			_loading_background = txrWWW.texture;
			gTxr = (GUITexture)GameObject.Find("LoadingBackground").GetComponent("GUITexture");
			gTxr.texture = _loading_background;
			gTxr.pixelInset = new Rect(Screen.width/2, Screen.height/2, 0, 0);
		}
		
		yield return 0;
	}
	
	IEnumerator SomeFunctionSecondThread() {
		loadprogress++;
		yield return 0;
	}
	
	Texture2D CreateUsingMaskAlpha(Texture2D diffuse, Texture2D mask)
	{
		if (_loading_barmasked != null)
			Texture2D.Destroy(_loading_barmasked);
	   _loading_barmasked = new Texture2D(diffuse.width, diffuse.height, TextureFormat.ARGB32, false);
	   
	   Color[] array = diffuse.GetPixels();
	   Color[] array2 = mask.GetPixels();

            for (int i = 0; i < diffuse.height; i++)
            {
                for (int j = 0; j < diffuse.width; j++)
                {
                    int offset = -loadmax-((loadmax/100)*loadprogress);
                    int t = j + (i * diffuse.width);
                    int t2 = j + offset + (i * mask.width);
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


