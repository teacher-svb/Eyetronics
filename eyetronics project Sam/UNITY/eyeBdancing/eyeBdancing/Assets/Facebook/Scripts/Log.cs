//============================================================================
// Title:  Color Matching (Unity-Facebook)
//----------------------------------------------------------------------------
// File:   Log.cs
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
// Log
//----------------------------------------------------------------------------
// Runtime log, displaying entries in different colors based on type.
//============================================================================
public class Log : MonoBehaviour
{


	//------------------------------------------------------------------------
	// class Entry
	//------------------------------------------------------------------------
	private class Entry 
	{	
		public string sValue;
		public Type   eType;
	}


	//------------------------------------------------------------------------
	// enum tType
	//------------------------------------------------------------------------
	static public enum Type
	{	
		EN_Message,
		EN_Warning,
		EN_Error
	}
	
	
	//------------------------------------------------------------------------
	// Globals, Static
	//------------------------------------------------------------------------
	static ArrayList aEntries = new ArrayList();
	static Vector2   fScroll  = Vector2.zero;
	

	//------------------------------------------------------------------------
	// Globals, Public
	//------------------------------------------------------------------------	
	public Director kDirector;	
	public bool     bShow = false;


	//------------------------------------------------------------------------
	// Globals, Private
	//------------------------------------------------------------------------
	private Color[] aColors = new Color[3];
	
		
	//------------------------------------------------------------------------
	// Globals, GUI Windows
	//------------------------------------------------------------------------
	private Rect rectButton = new Rect( 243, 435,  64,  32 );
	private Rect rectWindow = new Rect(   0,   0, 312, 450 );
	private Rect rectScroll = new Rect(   0,   0, 302, 300 );
	
	private float fBuffer  = 17;
	private float fPadding = 10;


	//========================================================================
	// void Awake()
	//------------------------------------------------------------------------
	// Set colors for Entry types.
	//========================================================================	
	void Awake()
	{
		aColors[( int )Type.EN_Message] = Color.white;
		aColors[( int )Type.EN_Warning] = Color.yellow;
		aColors[( int )Type.EN_Error  ] = new Color( 1, 0, 0, 1 );
	}
	
	//========================================================================
	// void Add()
	//------------------------------------------------------------------------
	// Adds an entry to the Log.
	//========================================================================	
	static public void Add( string sValue, Type eType )
	{
		Entry kEntry  = new Entry();
		kEntry.sValue = sValue;
		kEntry.eType  = eType;
		aEntries.Add( kEntry );	
	}


	//========================================================================
	// void Add()
	//------------------------------------------------------------------------
	// Adds a Message entry to the Log.
	//========================================================================	
	static public void Add( string sValue )
	{
		Add( sValue, Type.EN_Message );	
	}


	//========================================================================
	// void Warning()
	//------------------------------------------------------------------------
	// Adds a Warning entry to the Log.
	//========================================================================
	static public void Warning( string sValue )
	{
		Add( sValue, Type.EN_Warning );	
	}


	//========================================================================
	// void Error()
	//------------------------------------------------------------------------
	// Adds an Error entry to the Log.
	//========================================================================
	static public void Error( string sValue )
	{
		Add( sValue, Type.EN_Error );	
	}
	
		
	//========================================================================
	//  LogWindow
	//------------------------------------------------------------------------
	// Content for the Log Window.
	//========================================================================	
	void LogWindow( int iWindow )
	{		
		GUIStyle guiStyle = new GUIStyle();
				
		//--------------------------------------------------------------------
		// Scrollable Region
		//--------------------------------------------------------------------		
		fScroll = GUI.BeginScrollView ( rectScroll, fScroll, new Rect( rectScroll.x , rectScroll.y , rectScroll.width , aEntries.Count * fBuffer )); 
		
		float x = 0;
		foreach( Entry kEntry in aEntries )
		{
			x += fBuffer;
			guiStyle.normal.textColor = aColors[( int )kEntry.eType];
			GUI.Label( new Rect ( fPadding, x - ( 5 * x / fBuffer ), rectScroll.width - ( 2 * fPadding ), fBuffer ), kEntry.sValue, guiStyle );
		}
			
		GUI.EndScrollView();
	}


	//========================================================================
	// void OnGUI()
	//------------------------------------------------------------------------
	// If the public Inspector boolean is true, the Log is displayed.
	//========================================================================	
	void OnGUI()
	{	
		if( bShow )
			rectWindow = GUI.Window( 0, rectWindow, LogWindow, "" );
	}
}