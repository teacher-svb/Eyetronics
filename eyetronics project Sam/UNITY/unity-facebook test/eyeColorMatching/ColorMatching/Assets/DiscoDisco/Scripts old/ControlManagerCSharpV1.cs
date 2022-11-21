using UnityEngine;
using System.Collections;

public class ControlManagerCSharpV1 : MonoBehaviour {
	
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
		GameManagerCSharpV1 temp = (GameManagerCSharpV1)this.gameObject.GetComponent("GameManagerCSharpV1");
		GameObject body = GameObject.Find("bodies_idle");
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
		else if (Input.GetKey(KeyCode.RightArrow))	//right
		{
			body.animation.Play ("right");
		//doe dancemove 	
		}
		else if (Input.GetKey(KeyCode.LeftArrow))	//left
		{
			body.animation.Play ("left");
		//doe dancemove 	
		}
		else if (Input.GetKey(KeyCode.UpArrow))	//up
		{
			body.animation.Play ("up");
		//doe dancemove 	
		}
		else if (Input.GetKey(KeyCode.DownArrow))	//down
		{
			body.animation.Play ("down");
		//doe dancemove 	
		}			
		//einde 1 knop
		//eind van de pijltjes toetsen
		if (Input.GetKey(KeyCode.Escape))
			temp.GoToPause();
		if (Input.GetMouseButtonDown(0)) {
			if (temp._isBeatActive)
				//fever increasen ipv points
				temp._points++;
			else
				//fever resetten naar 0 ipv points--
				temp._points--;
			//als je onder nul gaat, vastzetten op nul
			if (temp._points < 0)
				temp._points = 0;
		}
	}
	
	void OnPause () {
		
	}
	
	void OnLoad () {
		
	}
}
