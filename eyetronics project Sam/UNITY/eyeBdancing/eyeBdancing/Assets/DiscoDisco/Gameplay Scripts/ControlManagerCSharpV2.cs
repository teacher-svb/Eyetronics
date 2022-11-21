using UnityEngine;
using System.Collections;

public class ControlManagerCSharpV2 : MonoBehaviour {
	
	int correctclick = 0;
	int currentclick = 0;
	public Texture2D[] pijltjes;
	public int temp = 0;
	
	bool hasClicked;
	
	void Start () {
		currentclick = Random.Range(3,8);
		
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
		int score = (GameObject.Find("scorekeeper").GetComponent<ScoreBoxScriptCSharpV1>()._feverScore + 1) * 10;
		//input van de pijltjes toetsen lezen
		if (Input.GetKeyUp(KeyCode.RightArrow) && temp == 0 && hasClicked) {
			body.animation.CrossFadeQueued ("dance1",0.3f, QueueMode.CompleteOthers);
			GameObject.Find("scorekeeper").GetComponent<ScoreBoxScriptCSharpV1>().AddScore(score, false);
			hasClicked = false;
		}
		else if (Input.GetKeyUp(KeyCode.LeftArrow) && temp == 2 && hasClicked) {
			body.animation.CrossFadeQueued ("dance2",0.3f, QueueMode.CompleteOthers);
			GameObject.Find("scorekeeper").GetComponent<ScoreBoxScriptCSharpV1>().AddScore(score, false);
			hasClicked = false;
		}
		else if (Input.GetKeyUp(KeyCode.UpArrow) && temp == 1 && hasClicked) {
			body.animation.CrossFadeQueued ("dance3",0.3f, QueueMode.CompleteOthers);
			GameObject.Find("scorekeeper").GetComponent<ScoreBoxScriptCSharpV1>().AddScore(score, false);
			hasClicked = false;
		}
		else if (Input.GetKeyUp(KeyCode.DownArrow)  && temp == 3 && hasClicked) {
			body.animation.CrossFadeQueued ("dance4",0.3f, QueueMode.CompleteOthers);
			GameObject.Find("scorekeeper").GetComponent<ScoreBoxScriptCSharpV1>().AddScore(score, false);
			hasClicked = false;
		}
		else
			body.animation.CrossFadeQueued("idle_male", 0.3f, QueueMode.CompleteOthers);
		
		if (Input.GetKey(KeyCode.Escape))
			this.gameObject.GetComponent<GameManagerCSharpV1>().GoToPause();
		if (Input.GetMouseButtonDown(0)) {
			if (this.gameObject.GetComponent<GameManagerCSharpV1>()._isBeatActive) {
				GameObject.Find("scorekeeper").GetComponent<ScoreBoxScriptCSharpV1>().AddScore(1, true);
				//~ if (correctclick % 3 == 0 )
				hasClicked = true;
				correctclick++;
				if (correctclick == currentclick)
					RandomizeArrow();
			}
			else if (!this.gameObject.GetComponent<GameManagerCSharpV1>()._isBeatActive && GameObject.Find("scorekeeper").GetComponent<ScoreBoxScriptCSharpV1>()._score > 0)
				GameObject.Find("scorekeeper").GetComponent<ScoreBoxScriptCSharpV1>().AddScore(-1, true);
		}
	}
	
	void RandomizeArrow(){
		correctclick = 0;
		//~ Random.seed(Time.frameCount);
		temp = Random.Range(0,4);
		currentclick = Random.Range(3,8);
	}
	
	public void DrawGUI () {
		//teken een pijltje
		GUI.DrawTexture(new Rect(Screen.width/2 - 25,Screen.height - 70,50,50), pijltjes[temp]);
	}
	
	void OnPause () {
		
	}
	
	void OnLoad () {
		
	}
}
