using UnityEngine;
using System.Collections;


public class ControlManagerCSharpV2 : MonoBehaviour {
	
	public int correctclick = 0;
	public int correctclickurrent = 0;
	public Texture2D[] pijltjes;
	public int temp = 0;
	
	void Start () {
		
		correctclickurrent = Random.Range(3,8);
		
		GameObject body = GameObject.Find("bodies_animated");
		body.animation["dance1"].layer = 1;
		body.animation["dance2"].layer = 1;
		body.animation["dance3"].layer = 1;
		body.animation["dance4"].layer = 1;
	}
	
	void Update () {
		GameManagerCSharpV1 temp = (GameManagerCSharpV1)this.gameObject.GetComponent("GameManagerCSharpV1");
		
		switch (temp.GameState) {
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
	
	void OnPlay () {
		GameObject body = GameObject.Find("bodies_animated");
		//input van de pijltjes toetsen lezen
		if (Input.GetKeyUp(KeyCode.RightArrow) && temp == 0){
			GameObject.Find("scorekeeper").GetComponent<ScoreBoxScriptCSharpV1>().AddScore(100);
			body.animation.CrossFadeQueued ("dance1",0.3f, QueueMode.CompleteOthers);
			}
		else if (Input.GetKeyUp(KeyCode.LeftArrow) && temp == 2){
			GameObject.Find("scorekeeper").GetComponent<ScoreBoxScriptCSharpV1>().AddScore(100);
			body.animation.CrossFadeQueued ("dance2",0.3f, QueueMode.CompleteOthers);
			}
		else if (Input.GetKeyUp(KeyCode.UpArrow) && temp == 1){
			GameObject.Find("scorekeeper").GetComponent<ScoreBoxScriptCSharpV1>().AddScore(100);
			body.animation.CrossFadeQueued ("dance3",0.3f, QueueMode.CompleteOthers);
			}
		else if (Input.GetKeyUp(KeyCode.DownArrow) && temp == 3){
			GameObject.Find("scorekeeper").GetComponent<ScoreBoxScriptCSharpV1>().AddScore(100);
			body.animation.CrossFadeQueued ("dance4",0.3f, QueueMode.CompleteOthers);
		}
		else
			body.animation.CrossFadeQueued("idle_male", 0.3f, QueueMode.CompleteOthers);
		
		if (Input.GetKey(KeyCode.Escape))
			this.gameObject.GetComponent<GameManagerCSharpV1>().GoToPause();
		if (Input.GetMouseButtonDown(0)) {
			if (this.gameObject.GetComponent<GameManagerCSharpV1>()._isBeatActive){
				correctclick++;
				if(correctclick ==correctclickurrent)
					RandomizeArrow();
				GameObject.Find("scorekeeper").GetComponent<ScoreBoxScriptCSharpV1>().AddScore(1);
			}
			else if (!this.gameObject.GetComponent<GameManagerCSharpV1>()._isBeatActive && GameObject.Find("scorekeeper").GetComponent<ScoreBoxScriptCSharpV1>()._score > 0){
				//~ correctclick--;
				GameObject.Find("scorekeeper").GetComponent<ScoreBoxScriptCSharpV1>().AddScore(-1);
				}
		}
	}
	void RandomizeArrow(){
		correctclick = 0;
		//~ Random.seed(Time.frameCount);
		temp = Random.Range(0,4);
		correctclickurrent = Random.Range(3,8);
	}
	
	public void DrawGUI () {
		//teken een pijltje
		GUI.DrawTexture(new Rect(250,200,50,50), pijltjes[temp]);
	}
	
	void OnLoad () {
		
	}	
	void OnPause () {
		
	}
}
