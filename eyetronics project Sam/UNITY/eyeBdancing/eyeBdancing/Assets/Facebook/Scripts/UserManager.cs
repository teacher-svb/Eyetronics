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
		public long      fb_id;
		public long      eyeBcom_id;
		public string    f_name;
		public string    pic_url;
		public string    txr_url;
		public string    mesh_url;
		public int       score = 0;
		public Texture2D pic   = new Texture2D( 512, 512 );
		public Texture2D txr   = new Texture2D(512,512);
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
	public string    iEyeBcomID = "0";
	
	private ArrayList aUsers = new ArrayList();


	//========================================================================
	// void GetUserByID()
	//------------------------------------------------------------------------
	// Searches for a match by ID and returns a User from the User array.
	//========================================================================
	private User GetUserByID( int id )
	{
		Debug.Log("GetUserByID");
		foreach( User kUser in aUsers )
		{
			if( kUser.eyeBcom_id == id )
				return kUser;
		}
		
		return null;
	}


	//========================================================================
	// void GetUserByFb_ID()
	//------------------------------------------------------------------------
	// Searches for a match by ID and returns a User from the User array.
	//========================================================================
	private User GetUserByFb_ID( int id )
	{
		Debug.Log("GetUserByFb_ID");
		foreach( User kUser in aUsers )
		{
			if( kUser.fb_id == id )
				return kUser;
		}
		
		return null;
	}
	

	//========================================================================
	// void GetUserByURL()
	//------------------------------------------------------------------------
	// Searches for a match by URL and returns a User from the User array.
	//========================================================================
	private User GetUserByPicURL( string url )
	{
		Debug.Log("GetUserByPicURL");
		foreach( User kUser in aUsers )
		{
			if( kUser.pic_url == url )
				return kUser;
		}
		
		return null;
	}
	

	//========================================================================
	// void GetUserByURL()
	//------------------------------------------------------------------------
	// Searches for a match by URL and returns a User from the User array.
	//========================================================================
	private User GetUserByTxrURL( string url )
	{
		Debug.Log("GetUserByTxrURL");
		foreach( User kUser in aUsers )
		{
			if( kUser.txr_url == url )
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
		Debug.Log( "GetUserID" );
		
		// For Running in Unity Editor
		if( Application.isEditor )
			PutUserID( iEyeBcomID );

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
		Debug.Log( "PutUserID" );
		
		
		//--------------------------------------------------------------------
		// Add new User to User Array
		//--------------------------------------------------------------------
		string[] info = text.Split( ';' );
		long id = long.Parse( info[0] );

		User kUser = new User();
		kUser.eyeBcom_id = id;
		aUsers.Add( kUser );
		
		
		//--------------------------------------------------------------------
		// Request User Information
		//--------------------------------------------------------------------
		GetUser( kUser.eyeBcom_id );
		

		//--------------------------------------------------------------------
		// Request Game Stats and Friends ID
		//--------------------------------------------------------------------
		//~ WWWForm wForm = new WWWForm();
		//~ wForm.AddField("id", System.Convert.ToString( kUser.fb_id ) );
		//~ wForm.AddField("eyeBcom_id", System.Convert.ToString( kUser.eyeBcom_id ) );
		//~ kDirector.PHP.Request( wForm, PHPHandler.Action.EN_GetStats,   PutStats   );
		//~ kDirector.PHP.Request( wForm, PHPHandler.Action.EN_GetFriends, GetFriends );
	}


	//========================================================================
	// void PutUserID()
	//------------------------------------------------------------------------
	// PHP Handle Callback for GetUserID().
	// Receives the user's Facebook ID, adds to Users array and requests
	// account information, game stats and friends ID. 
	//========================================================================
	public void PutUserFb_ID( string text )
	{
		Debug.Log( "PutUserFb_ID" );
		
		
		//--------------------------------------------------------------------
		// Add new User to User Array
		//--------------------------------------------------------------------
		string[] info = text.Split( ';' );
		long id = long.Parse( info[0] );

		User kUser = new User();
		kUser.fb_id = id;
		aUsers.Add( kUser );

		
		
		//--------------------------------------------------------------------
		// Request User Information
		//--------------------------------------------------------------------
		GetUserFB( kUser.fb_id );
		

		//--------------------------------------------------------------------
		// Request Game Stats and Friends ID
		//--------------------------------------------------------------------
		//~ WWWForm wForm = new WWWForm();
		//~ wForm.AddField("id", System.Convert.ToString( kUser.fb_id ) );
		//~ wForm.AddField("id", System.Convert.ToString( kUser.fb_id ) );
		//~ kDirector.PHP.Request( wForm, PHPHandler.Action.EN_GetStats,   PutStats   );
		//~ kDirector.PHP.Request( wForm, PHPHandler.Action.EN_GetFriends, GetFriends );
	}
	
	
	//========================================================================
	// void GetFriends()
	//------------------------------------------------------------------------
	// Add up to four friends to User array.
	//========================================================================
	public void GetFriends( string data )
	{
		Debug.Log( "GetFriends" );
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
			kUser.fb_id   = int.Parse(info[x]);
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
		Debug.Log( "RefreshUser" );
		long id = long.Parse( data );
		GetUser( id );
		
		WWWForm wForm = new WWWForm();
		wForm.AddField( "eyeBcom_id", System.Convert.ToString( id ) );
		kDirector.PHP.Request( wForm, PHPHandler.Action.EN_GetStats, PutStats );
	}
	

	//========================================================================
	// void GetUser()
	//------------------------------------------------------------------------
	// Requests Account information.
	//========================================================================
	void GetUser( long id )
	{
		Debug.Log( "GetUser" );
		WWWForm wForm = new WWWForm();
		wForm.AddField( "eyeBcom_id", System.Convert.ToString( id ) );
		kDirector.PHP.Request( wForm, PHPHandler.Action.EN_GetUser, PutUserInfo );
	}
	

	//========================================================================
	// void GetUser()
	//------------------------------------------------------------------------
	// Requests Account information.
	//========================================================================
	void GetUserFB( long id )
	{
		Debug.Log( "GetUserFB" );
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
		Debug.Log( "PutUserInfo" );
		string[] info = data.Split(';');
		int eyeBcom_id = 0;
		if (info[0] != "N/A")
			eyeBcom_id = int.Parse(info[0]);
			int fb_id = int.Parse(info[1]);

		User kUser = new User();
		kUser = GetUserByID( eyeBcom_id );
		if (kUser == null)
		{
			kUser = GetUserByFb_ID( fb_id );
			kUser.eyeBcom_id = eyeBcom_id;
		}
		else
			kUser.fb_id = fb_id;
		kUser.f_name = info[2];
		
		
		if( kUser.eyeBcom_id != 0 )
		{
			kUser.pic_url = info[3];
			kUser.mesh_url = info[4];
			kUser.txr_url = info[5];
			kUser.score   = int.Parse( info[6] );
			//----------------------------------------------------------------
			// Requesting User Picture
			//----------------------------------------------------------------
			
			// WWWForm wForm = new WWWForm();
			//~ wForm.AddField( "id", System.Convert.ToString( kUser.eyeBcom_id ) );
			kDirector.PHP.RequestTexture( kUser.pic_url, PutUserPic );
			Debug.Log("" + kUser.txr_url + kUser.eyeBcom_id + ".jpg");
			kDirector.PHP.RequestTexture( kUser.txr_url, PutUserTxr );
			kDirector.PHP.RequestObj(kUser.mesh_url, PutUserObj );
		}
	}


	//========================================================================
	// void PutUserObj()
	//------------------------------------------------------------------------
	// PHP Handle Callback for SendScore().
	// Finds User in Users array by URL and assigns the received texture.
	//========================================================================
	public void PutUserObj( WWW www )
	{	
		Debug.Log(www.url);
		Debug.Log( "PutUserObj" );
		GameObject.Find("GameCamera").GetComponent<LoadManagerCSharpV1>().AddMorphTargetToLoad(www.url, "neutral", "character");
		StartCoroutine(GameObject.Find("GameCamera").GetComponent<LoadManagerCSharpV1>().Init());
	}


	//========================================================================
	// void PutUserPic()
	//------------------------------------------------------------------------
	// PHP Handle Callback for SendScore().
	// Finds User in Users array by URL and assigns the received texture.
	//========================================================================
	public void PutUserPic( WWW www )
	{	
		Debug.Log( "PutUserPic" );
		User kUser = new User();
		kUser = GetUserByPicURL( www.url );
		kUser.pic = www.texture;
	}


	//========================================================================
	// void PutUserTxr()
	//------------------------------------------------------------------------
	// PHP Handle Callback for SendScore().
	// Finds User in Users array by URL and assigns the received texture.
	//========================================================================
	public void PutUserTxr( WWW www )
	{	
		Debug.Log( "PutUserTxr" );
		User kUser = new User();
		kUser = GetUserByTxrURL( www.url );
		//~ string test = www.url.Substring(www.url.LastIndexOf('/') + 1, www.url.Length - 4 - (www.url.LastIndexOf('/') + 1));
		//~ Debug.Log(test);
		//~ kUser = GetUserByID(int.Parse(test));
		kUser.txr = www.texture;
		GameObject.Find("neutral").GetComponent<MeshRenderer>().material.SetTexture("_MainTex", kUser.txr);
		GameObject.Find("neutral(Clone)").GetComponent<MeshRenderer>().material.SetTexture("_MainTex", kUser.txr);
	}


	//========================================================================
	// void PutStats()
	//------------------------------------------------------------------------
	// PHP Handle Callback for SendScore().
	// Receives and assigns values for Stat Box.
	//========================================================================
	public void PutStats( string data )
	{
		Debug.Log( "PutStats" );
		//~ string[] info = data.Split(';');
		//~ iUsers        = int.Parse( info[0] );
		//~ iFriends      = int.Parse( info[1] );
		//~ iGlobalRank   = int.Parse( info[2] );
		//~ iLocalRank    = int.Parse( info[3] );

	}
	
	
	//========================================================================
	// void SendScore()
	//------------------------------------------------------------------------
	// If new score is lower than the recorded score, submit to database.
	//========================================================================
	public void SendScore( int score )
	{
		Debug.Log( "SendScore" );
		if( score < ( (User)aUsers[0]).score || ( (User)aUsers[0]).score == 0 )
		{
			WWWForm wForm = new WWWForm();
			wForm.AddField( "eyeBcom_id", System.Convert.ToString( ((User)aUsers[0]).eyeBcom_id ) );
			wForm.AddField( "score", score );
			kDirector.PHP.Request( wForm, PHPHandler.Action.EN_PutScore, RefreshUser );	
		}
	}
	
	void OnGUI () {
		
	}
}