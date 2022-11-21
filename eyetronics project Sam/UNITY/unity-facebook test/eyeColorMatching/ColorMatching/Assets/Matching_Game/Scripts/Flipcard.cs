//============================================================================
// Title:  Color Matching (Unity-Facebook)
//----------------------------------------------------------------------------
// File:   Flipcard.cs
// Author: Nick Breslin (nickbreslin@gmail.com), Waddlefarm.org
//
// Copyright (c) Nick Breslin, 2009. All Rights Reserved.
//----------------------------------------------------------------------------
//
// * This license notice may not be removed or altered.
// * All project files are free for non-commercial or commercial use.
// * I appreciate personal notification if any part of this project is used.
// * All project files are provided "as is". I assume no responsibility for
//   results from using these files, nor should any support be expected.
// * Donations are appreciated, please visit: http://www.waddlefarm.org/donate
//
//============================================================================

using UnityEngine;
using System.Collections;

//============================================================================
//  Flipcard
//----------------------------------------------------------------------------
// Flipcard is attached to each individual card. Flipcard receives input from
// user, passes it to Memory, and controls the rotation and state for the card.
//============================================================================
public class Flipcard : MonoBehaviour
{
	
	
	//------------------------------------------------------------------------
	// State (enum)
	//------------------------------------------------------------------------
	public enum State
	{
		Hidden,
		Flipping,
		Flipped
	}
	
	
	//------------------------------------------------------------------------
	// Globals (hidden, public)
	//------------------------------------------------------------------------
	[HideInInspector] public State state;
	[HideInInspector] public int   value;


	//------------------------------------------------------------------------
	// Globals (private)
	//------------------------------------------------------------------------
	private Memory memory;	
	private float  angle;
	private float  time;
	private State  previousState;


	//========================================================================
	//  Awake
	//------------------------------------------------------------------------
	// Sets starting values for the class: the card is in the "hidden" state.
	//========================================================================	
	private void Awake()
	{
		time   = 0;
		angle  = 0;
		state  = State.Hidden;
		memory = GameObject.Find( "GameManager" ).GetComponent( typeof( Memory ) ) as Memory;

		if( memory == null ) Debug.LogError( "GameManager cannot be found." );
	}
	
	
	//========================================================================
	//  Update
	//------------------------------------------------------------------------
	// Controls the orientation of the planes (card).
	//========================================================================
	private void Update() 
	{	
		// Adjusts rotation of planes.
		if( state == State.Flipping )
		{
			time += Time.deltaTime;
			
			if( time > memory.flipTime )
			{			
				if ( previousState == State.Hidden )
					state = State.Flipped;
			
				else
					state = State.Hidden;
			}
			
			float newAngle = Mathf.LerpAngle( angle, angle + 180, time * 2 );
  	  		transform.parent.eulerAngles = new Vector3( 0, newAngle, 0 );
		}

		// Ensures proper angle for "hidden" status.
		else if( state == State.Hidden )
			transform.parent.eulerAngles = new Vector3( 0, 0, 0 );
					
		// Ensures proper angle for "flipped" status.
		else if( state == State.Flipped )
			transform.parent.eulerAngles = new Vector3( 0, 180, 0 );
	}
	
	
	//========================================================================
	//  OnMouseDown
	//------------------------------------------------------------------------
	// Ensures that clicks are only registered if the card is hidden (not
	// flipped). Sends the Flipcard object to the Memory script for comparison
	// and recording.
	//========================================================================
	private void OnMouseDown()
	{	
		if( state != State.Hidden )
			return;
			
		memory.CardPicked( GetComponent( typeof( Flipcard ) ) as Flipcard );	
	}
	
	
	//========================================================================
	//  Flip
	//------------------------------------------------------------------------
	// Sets the card to flipping status.
	//========================================================================
	public void Flip()
	{
		previousState = state;
		state         = State.Flipping;
		time          = 0;
		angle         = transform.parent.eulerAngles.y;
	}
}