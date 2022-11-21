//============================================================================
// Title:  Color Matching (Unity-Facebook)
//----------------------------------------------------------------------------
// File:   Memory.cs
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
//  Memory
//----------------------------------------------------------------------------
// The Memory class is the Game Manager for the Memory Game. Assign the
// cards and the textures via the Inspector. Memory controls the Board Setup,
// Input, and Game Logic.
//============================================================================
public class Memory : MonoBehaviour
{


	//------------------------------------------------------------------------
	// Globals (public)
	//------------------------------------------------------------------------	
	public Director     kDirector;
	public GameObject[] cards;
	public float        flipTime;	
	public GUIText      results;
	public GUIText      guiScore;


	//------------------------------------------------------------------------
	// Globals (public, textures)
	//------------------------------------------------------------------------	
	public Texture matWhite;
	public Texture matBlack;
	public Texture matRed;
	public Texture matGreen;
	public Texture matBlue;
	public Texture matYellow;


	//------------------------------------------------------------------------
	// Globals (private)
	//------------------------------------------------------------------------
	private bool       isGameOver;
	private int        key;
	private bool       isLocked;
	private Flipcard[] flippedCards;
	private int        score;
	
	
	//========================================================================
	//  Awake
	//------------------------------------------------------------------------
	// Check to make sure public variables are assigned.
	//========================================================================	
	private void Awake()
	{
		if( cards.Length == 0    ) Debug.LogError( "No cards assigned."          );
		if( flipTime     == 0    ) Debug.LogError( "Flip Time cannot be 0."      );
		if( results      == null ) Debug.LogError( "Unassigned GUIText."         );
		if( matWhite     == null ) Debug.LogError( "Unassigned White material."  );
		if( matBlack     == null ) Debug.LogError( "Unassigned Black material."  );
		if( matRed       == null ) Debug.LogError( "Unassigned Red material."    );
		if( matGreen     == null ) Debug.LogError( "Unassigned Green material."  );
		if( matBlue      == null ) Debug.LogError( "Unassigned Blue material."   );
		if( matYellow    == null ) Debug.LogError( "Unassigned Yellow material." );
	}


	//========================================================================
	//  Start
	//------------------------------------------------------------------------
	// Sets starting values for the class: the card is in the "hidden" state.
	//========================================================================	
	private void Start()
	{
		flippedCards = new Flipcard[3];
		StartCoroutine( SetGame( 0 ) );
	}


	//========================================================================
	//  Update
	//------------------------------------------------------------------------
	// Sets starting values for the class: the card is in the "hidden" state.
	//========================================================================	
	private void Update()
	{
		if( isGameOver ) // All Pairs Found
			return;
			
		if( isLocked )   // Cards Are Flipping
			return;

		if( key == 2 )	 // Two Cards Are Flipped
			StartCoroutine( Compare() );	
	}


	//========================================================================
	//  Compare
	//------------------------------------------------------------------------
	// Compares the two selected cards and displays results. Checks for Win
	// Condition after every successful match.
	//========================================================================	
	private IEnumerator Compare()
	{
		key = 0; // Important: Set to 0 first so coroutine 
				 // doesn't get called again from Update().
		
		
		score++;
		guiScore.text = "Score: " + score;
		 		
		
		//---------------------------------
		// Failed Match
		if( flippedCards[1].value != flippedCards[2].value )
		{	
			results.text = "Try again.";
			
			// Lock controls during a results pause, and for the reset flip
			StartCoroutine( Lock( flipTime + 0.1f) );		
			yield return new WaitForSeconds( 0.1f );
	
			flippedCards[1].Flip();
			flippedCards[2].Flip();
		}
		
		
		//---------------------------------
		// Successful Match
		else
		{	
			results.text = "Well done!";
			yield return new WaitForSeconds( flipTime );
			CheckWin();
		}
		

		//---------------------------------
		// Clear Results text as long as
		// game is not over.
		//---------------------------------
		if( !isGameOver )
		{
			yield return new WaitForSeconds( flipTime );
			results.text = "";
		}
	}


	//========================================================================
	//  OnGUI
	//------------------------------------------------------------------------
	// Sets starting values for the class: the card is in the "hidden" state.
	//========================================================================	
	private void OnGUI()
	{
		if( !isGameOver )
			return;
			
		if( GUI.Button( new Rect( 207, 0, 200, 70 ), "Play Again?" ) )
		{
			foreach( GameObject card in cards )
				( card.transform.Find( "PlaneQuestion" ).GetComponent(
					typeof( Flipcard ) ) as Flipcard).Flip();
		
			StartCoroutine( SetGame( flipTime ) );
		}

	}


	//========================================================================
	//  SetGame (Coroutine)
	//------------------------------------------------------------------------
	// Sets starting values for the class: the card is in the "hidden" state.
	//========================================================================
	private IEnumerator SetGame( float delay )
	{
		yield return new WaitForSeconds( delay );
		
		
		//------------------------------------------
		// Initialize Game Globals
		//------------------------------------------
		key          = 0;
		results.text = "";
		isLocked     = false;
		isGameOver   = false;
		score        = 0;
		
		
		//------------------------------------------
		// Randomize Cards
		//------------------------------------------
		int[] record = new int[cards.Length/2];
		
		for( int i = 0; i < cards.Length; i++ )
		{
			// Get Random Value
			int rand = GetRandomInt( record );
			record[rand]++;
			
			// Select Texture
			Texture temp = matWhite;
			if( rand == 0 ) temp = matWhite;
			if( rand == 1 ) temp = matBlack;
			if( rand == 2 ) temp = matRed;
			if( rand == 3 ) temp = matBlue;
			if( rand == 4 ) temp = matGreen;
			if( rand == 5 ) temp = matYellow;
			
			// Assign Texture and Random Value
			cards[i].transform.Find( "PlaneColor" ).renderer.material.mainTexture = temp;
			
			( cards[i].transform.Find( "PlaneQuestion" ).GetComponent(
				typeof( Flipcard ) ) as Flipcard ).value = rand;
		}
	}
	

	//========================================================================
	//  CardPicked
	//------------------------------------------------------------------------
	// Ignores card selection if controls are Locked (card is flipping) or
	// if two cards are already selected.
	//
	// Increases the selection key, records the card, flips the card, and 
	// locks input for the duration of the flip.
	//========================================================================
	public void CardPicked( Flipcard temp )
	{
		if( isLocked || ( key == 2 ) )
			return;
		
		key++;					  
		flippedCards[key] = temp;
		temp.Flip();
		
		StartCoroutine( Lock( flipTime ) );
	}


	//========================================================================
	//  Lock (Coroutine)
	//------------------------------------------------------------------------
	// Locks the controls for a specific amount of time, then unlocks. Used
	// to block user input while cards are flipping.
	//========================================================================
	private IEnumerator Lock( float time )
	{
		isLocked = true;
		yield return new WaitForSeconds( time );
		isLocked = false;
	}


	//========================================================================
	//  AllCardsAre
	//------------------------------------------------------------------------
	// Returns whether or not all cards are at the parameter state.
	//========================================================================	
	private bool AllCardsAre( Flipcard.State state )
	{
		foreach( GameObject card in cards )
		{
			if( ( card.transform.Find( "PlaneQuestion" ).GetComponent(
				typeof( Flipcard ) ) as Flipcard ).state != state )
			
				return false;
		}
		return true;
	}
	

	//========================================================================
	//  void CheckWin()
	//------------------------------------------------------------------------
	// Checks for Win Condition, then Sends Score
	//========================================================================		
	private void CheckWin()
	{
		if( AllCardsAre( Flipcard.State.Flipped ) )
		{
			results.text = "You Won!";
			isGameOver   = true;
			isLocked     = true;
			
			kDirector.kUserManager.SendScore( score );
		}
	}
	

	//========================================================================
	//  GetRandomInt
	//------------------------------------------------------------------------
	// Randomly selects a value based on the size of the array passed in. The
	// array being passed in is a record of how many times that specific value
	// had been previously selected.
	//
	// The random value will only be returned if it has been selected less
	// than twic before. If the selected value has already been selected twice
	// then the loop is called until a valid value is selected.
	//========================================================================	
	private int GetRandomInt( int[] array )
	{
		while( true )
		{
			int x = Random.Range( 0, array.Length );
			
			if ( array[x] < 2 )
				return x;
		}
	}
}