using UnityEngine;
using System.Collections;

public class ControlManagerCSharpV2 : MonoBehaviour {
	
	//~ bool _isDancing = false;
	
	void Start () {
		GameObject body = GameObject.Find("bodies_animated");
		body.animation["dance1"].layer = 1;
		body.animation["dance2"].layer = 1;
		body.animation["dance3"].layer = 1;
		body.animation["dance4"].layer = 1;
	}
	
	// Update is called once per frame
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
		//alles ingeduwd
			if (Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.DownArrow))
			{
			//doe dancemove 	
			}
			//3 knoppen
			else if (Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.UpArrow)) //zonder down
			{
			//doe dancemove 	
			}
			else if (Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.DownArrow)) //zonder up
			{
			//doe dancemove 	
			}
			else if (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.UpArrow)) //zonder left
			{
			//doe dancemove 	
			}
			else if (Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.UpArrow)) //zonder right
			{
			//doe dancemove 	
			}
			//einde 3 knoppen
			//begin 2 knoppen
			else if (Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.RightArrow)) //left right
			{
			//doe dancemove 	
			}
			else if (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.DownArrow)) //up down
			{
			//doe dancemove 	
			}
			else if (Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.UpArrow)) //left up
			{
			//doe dancemove 	
			}
			else if (Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.DownArrow)) //left down
			{
			//doe dancemove 	
			}
			else if (Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.UpArrow)) //right up
			{
			//doe dancemove 	
			}
			else if (Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.DownArrow)) //right down
			{
			//doe dancemove 	
			}
			//einde 2 knoppen
			//begin 1 knop
			else if (Input.GetKeyUp(KeyCode.RightArrow) && !body.animation["idle_male"].enabled)	//right
			{
				body.animation.CrossFadeQueued ("dance1",0.3f, QueueMode.CompleteOthers);
				//~ _isDancing = true;
				//~ body.animation.CrossFadeQueued("idle_male");
			//doe dancemove 	
			}
			else if (Input.GetKeyUp(KeyCode.LeftArrow)&& !body.animation["idle_male"].enabled)	//left
			{
				body.animation.CrossFadeQueued ("dance2",0.3f, QueueMode.CompleteOthers);
				//~ _isDancing = true;
			//doe dancemove 	
			}
			else if (Input.GetKeyUp(KeyCode.UpArrow) && !body.animation["idle_male"].enabled)	//up
			{
				body.animation.CrossFadeQueued ("idle_male",0.3f, QueueMode.CompleteOthers);
				//~ _isDancing = true;
			//doe dancemove 	
			}
			else if (Input.GetKeyUp(KeyCode.DownArrow) && !body.animation["idle_male"].enabled)	//down
			{
				body.animation.CrossFadeQueued ("dance4",0.3f, QueueMode.CompleteOthers);
				//~ _isDancing = true;
			//doe dancemove 	
			}
		else {
				body.animation.CrossFadeQueued("idle_male", 0.3f, QueueMode.CompleteOthers);
			//~ _dancemovePlaying = false;
		}
		//~ if (body.animation["idle_male"].enabled)
			//~ Debug.Log("blub");
		//einde 1 knop
		if (Input.GetKey(KeyCode.Escape))
			this.gameObject.GetComponent<GameManagerCSharpV1>().GoToPause();
		if (Input.GetMouseButtonDown(0)) {
			if (this.gameObject.GetComponent<GameManagerCSharpV1>()._isBeatActive)
				GameObject.Find("scorekeeper").GetComponent<ScoreBoxScriptCSharpV1>().AddScore(1);
			else if (!this.gameObject.GetComponent<GameManagerCSharpV1>()._isBeatActive && GameObject.Find("scorekeeper").GetComponent<ScoreBoxScriptCSharpV1>()._score > 0)
				GameObject.Find("scorekeeper").GetComponent<ScoreBoxScriptCSharpV1>().AddScore(-1);
		}
	}
	
	void OnPause () {
		
	}
	
	void OnLoad () {
		
	}
}
