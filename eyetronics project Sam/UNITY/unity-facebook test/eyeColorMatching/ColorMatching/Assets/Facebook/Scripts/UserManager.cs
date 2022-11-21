//============================================================================
// Title:  Color Matching (Unity-Facebook)
//----------------------------------------------------------------------------
// File:   UserManager.cs
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
//  UserManager
//----------------------------------------------------------------------------
// Managers Facebook Users and Scores.
//============================================================================
public class UserManager : MonoBehaviour
{
	
	
	//------------------------------------------------------------------------
	// class User
	//------------------------------------------------------------------------	
	public class User
	{
		public long      id;
		public string    f_name;
		public string    pic_url;
		public int       score = 0;
		public Texture2D pic   = new Texture2D( 50, 50 );
	}


	//------------------------------------------------------------------------
	// Global, Public
	//------------------------------------------------------------------------
	public Director  kDirector;
	public Texture2D Profile_Background;
	public Texture2D Box_Small;
	public Texture2D Box_Large;
	public Texture2D Column_Background;
	public string    iFacebookID = "0";


	//------------------------------------------------------------------------
	// Global, Private
	//------------------------------------------------------------------------	
	private int iUsers      = 0;
	private int iFriends    = 0;
	private int iGlobalRank = 0;
	private int iLocalRank  = 0;
	
	private ArrayList aUsers = new ArrayList();


	//========================================================================
	// void GetUserByID()
	//------------------------------------------------------------------------
	// Searches for a match by ID and returns a User from the User array.
	//========================================================================
	private User GetUserByID( int id )
	{
		foreach( User kUser in aUsers )
		{
			if( kUser.id == id )
				return kUser;
		}
		
		return null;
	}
	

	//========================================================================
	// void GetUserByURL()
	//------------------------------------------------------------------------
	// Searches for a match by URL and returns a User from the User array.
	//========================================================================
	private User GetUserByURL( string url )
	{
		foreach( User kUser in aUsers )
		{
			if( kUser.pic_url == url )
				return kUser;
		}
		
		return null;
	}


	//========================================================================
	// void GetUserID()
	//------------------------------------------------------------------------
	// Calls to the JavaScript GetUserID() to request the user's Facebook ID.
	//========================================================================
	public void GetUserID()
	{
		Log.Add( "Requesting UserID." );
		
		// For Running in Unity Editor
		if( Application.isEditor )
			PutUserID( iFacebookID );

		else
			Application.ExternalCall( "GetUserID" );
	

	}


	//========================================================================
	// void PutUserID()
	//------------------------------------------------------------------------
	// PHP Handle Callback for GetUserID().
	// Receives the user's Facebook ID, adds to Users array and requests
	// account information, game stats and friends ID. 
	//========================================================================
	public void PutUserID( string text )
	{
		
		
		//--------------------------------------------------------------------
		// Add new User to User Array
		//--------------------------------------------------------------------
		string[] info = text.Split( ';' );
		int id = int.Parse( info[0] );

		User kUser = new User();
		kUser.id = id;
		
		aUsers.Add( kUser );

		Log.Add( "Received UserID: " + id ); 
		
		
		//--------------------------------------------------------------------
		// Request User Information
		//--------------------------------------------------------------------
		GetUser( kUser.id );
		

		//--------------------------------------------------------------------
		// Request Game Stats and Friends ID
		//--------------------------------------------------------------------
		Log.Add( "Requesting Game Stats." );
		WWWForm wForm = new WWWForm();
		wForm.AddField("id", System.Convert.ToString( kUser.id ) );
		kDirector.PHP.Request( wForm, PHPHandler.Action.EN_GetStats,   PutStats   );
		kDirector.PHP.Request( wForm, PHPHandler.Action.EN_GetFriends, GetFriends );
	}
	
	
	//========================================================================
	// void GetFriends()
	//------------------------------------------------------------------------
	// Add up to four friends to User array.
	//========================================================================
	public void GetFriends( string data )
	{
		//--------------------------------------------------------------------
		// Divides IDs from friends.
		//--------------------------------------------------------------------
		string[] info = data.Split(';');
				
		for( int x = 1; x < info.Length; x++ )
		{
			//----------------------------------------------------------------
			// Request friend account information.
			//----------------------------------------------------------------
			User kUser = new User();
			kUser.id   = int.Parse(info[x]);
			aUsers.Add( kUser );


			//----------------------------------------------------------------
			// Request friend account information.
			//----------------------------------------------------------------
			GetUser( int.Parse(info[x]) );


			//----------------------------------------------------------------
			// Can only store and display up to four friends.
			//----------------------------------------------------------------
			if( x == 5 )
				break;
		}
	}


	//========================================================================
	// void RefreshUser()
	//------------------------------------------------------------------------
	// PHP Handle Callback for SendScore().
	// After Score is submitted, User and Score information is reloaded.
	//========================================================================
	void RefreshUser( string data )
	{
		long id = long.Parse( data );
		GetUser( id );
		Log.Add( "Requesting Game Stats." );
		WWWForm wForm = new WWWForm();
		wForm.AddField( "id", System.Convert.ToString( id ) );
		kDirector.PHP.Request( wForm, PHPHandler.Action.EN_GetStats, PutStats );
	}
	

	//========================================================================
	// void GetUser()
	//------------------------------------------------------------------------
	// Requests Account information.
	//========================================================================
	void GetUser( long id )
	{
		Log.Add( "Requesting User Data: " + id );
		WWWForm wForm = new WWWForm();
		wForm.AddField( "id", System.Convert.ToString( id ) );
		kDirector.PHP.Request( wForm, PHPHandler.Action.EN_GetUser, PutUserInfo );
	}
	
	
	//========================================================================
	// void PutUserInfo()
	//------------------------------------------------------------------------
	// PHP Handle Callback for GetUser().
	// Finds User in Users array by ID and assigns the received information.
	//========================================================================
	public void PutUserInfo( string data )
	{	
		string[] info = data.Split(';');
		int id = int.Parse(info[0]);

		Log.Add( "Received User Data." + + id );

		User kUser = new User();
		kUser = GetUserByID( id );
		kUser.f_name = info[1];
		

		if( kUser.f_name != "N/A" )
		{
			kUser.pic_url = info[2];
			kUser.score   = int.Parse( info[3] );
			
			//----------------------------------------------------------------
			// Requesting User Picture
			//----------------------------------------------------------------
			WWWForm wForm = new WWWForm();
			wForm.AddField( "id", System.Convert.ToString( kUser.id ) );
		
			Log.Add( "Requesting User Image: " + kUser.id );
			kDirector.PHP.RequestTexture( kUser.pic_url, PutUserPic );
		}
	}


	//========================================================================
	// void PutUserPic()
	//------------------------------------------------------------------------
	// PHP Handle Callback for SendScore().
	// Finds User in Users array by URL and assigns the received texture.
	//========================================================================
	public void PutUserPic( WWW www )
	{	
		User kUser = new User();
		kUser = GetUserByURL( www.url );
		kUser.pic = www.texture;
			
		Log.Add( "Received User Image: " + kUser.id );
	}


	//========================================================================
	// void PutStats()
	//------------------------------------------------------------------------
	// PHP Handle Callback for SendScore().
	// Receives and assigns values for Stat Box.
	//========================================================================
	public void PutStats( string data )
	{
		string[] info = data.Split(';');
		iUsers        = int.Parse( info[0] );
		iFriends      = int.Parse( info[1] );
		iGlobalRank   = int.Parse( info[2] );
		iLocalRank    = int.Parse( info[3] );

		Log.Add( "Received Game Stats." );
	}
	
	
	//========================================================================
	// void SendScore()
	//------------------------------------------------------------------------
	// If new score is lower than the recorded score, submit to database.
	//========================================================================
	public void SendScore( int score )
	{
		if( score < ( (User)aUsers[0]).score || ( (User)aUsers[0]).score == 0 )
		{
			Log.Add( "Sending Score." );
			WWWForm wForm = new WWWForm();
			wForm.AddField( "id", System.Convert.ToString( ((User)aUsers[0]).id ) );
			wForm.AddField( "score", score );
			kDirector.PHP.Request( wForm, PHPHandler.Action.EN_PutScore, RefreshUser );	
		}
	}


	//========================================================================
	// void DrawAccountBox()
	//------------------------------------------------------------------------
	// Draw User and Friend Boxes.
	//========================================================================	
	void DrawAccountBox( int key, int y )
	{
		GUI.color = Color.white;
		GUI.DrawTexture( new Rect( Screen.width - 180, y + 10, 180, 70 ), Box_Large );
		GUI.DrawTexture( new Rect( Screen.width - 170, y + 20,  50, 50 ), ((User)aUsers[key]).pic );
		GUI.DrawTexture( new Rect( Screen.width - 173, y + 17,  56, 56 ), Profile_Background );
		GUI.Label( new Rect( Screen.width - 110, y + 30, 300, 30 ), ((User)aUsers[key]).f_name );
		GUI.color = new Color( 1, 0.5f, 1f );
		GUI.Label( new Rect( Screen.width - 110, y + 45, 300, 30 ), "Best Score: " + ((User)aUsers[key]).score );
	}


	//========================================================================
	// void DrawStatBox()
	//------------------------------------------------------------------------
	// Draw Stat Box
	//========================================================================	
	void DrawStatBox( int y )
	{
		GUI.color = Color.white;
		GUI.DrawTexture( new Rect( Screen.width - 180, y + 10, 180, 50 ), Box_Small );
		GUI.color = new Color( 0.5f, 0.5f, 1 );
		GUI.Label( new Rect( Screen.width - 170, y + 15, 300, 30 ), "Users: " + iUsers );
		GUI.color = new Color( 0.5f, 1, 1 );
		GUI.Label( new Rect( Screen.width - 170, y + 35, 300, 30 ), "Friends: " + iFriends );
		GUI.color = new Color( 1, 1, 0.5f );
		GUI.Label( new Rect( Screen.width - 90, y + 15, 300, 30 ), "Global Rank: " + iGlobalRank );
		GUI.color = new Color( 0.5f, 1, 0.5f );
		GUI.Label( new Rect( Screen.width - 90, y + 35, 300, 30 ), "Local Rank: " + iLocalRank );
	}
	
	
	//========================================================================
	// void OnGUI()
	//------------------------------------------------------------------------
	// Draw User, Stat and Friend Boxes
	//========================================================================
	void OnGUI()
	{	
		GUI.DrawTexture( new Rect( Screen.width - 170, 0, 180, 450 ), Column_Background );
		
		
		//--------------------------------------------------------------------
		// User Box
		//--------------------------------------------------------------------
		if( aUsers.Count > 0 )
			DrawAccountBox( 0, -10 );


		//--------------------------------------------------------------------
		// Stat/Rank Box
		//--------------------------------------------------------------------		
		DrawStatBox( 70 );


		//--------------------------------------------------------------------
		// Friend Boxes
		//--------------------------------------------------------------------
		int y = 130;
		for( int x = 1; x < aUsers.Count; x++ )
		{
			DrawAccountBox( x, y );
			y += 80;
		}		
	}
}