//============================================================================
// Title:  Color Matching (Unity-Facebook)
//----------------------------------------------------------------------------
// File:   PHPHandler.cs
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
// class PHPHandler()
//----------------------------------------------------------------------------
// Manages interfacing between Unity and PHPHandler.php
//============================================================================
public class PHPHandler : MonoBehaviour
{

	
	//------------------------------------------------------------------------
	// enum Action
	//------------------------------------------------------------------------
	public enum Action
	{
		EN_Ping,
		EN_GetUser,
		EN_PutScore,
		EN_GetStats,
		EN_GetTxrUrl,
		EN_GetMeshUrl
	}


	//------------------------------------------------------------------------
	// enum Query
	//------------------------------------------------------------------------
	public enum Query
	{
		EN_Sending,
		EN_Success,
		EN_Fail
	}


	//------------------------------------------------------------------------
	// Globals, public
	//------------------------------------------------------------------------	
	public Director kDirector;
	public delegate void Callback( string data );
	public delegate void CallbackTexture( WWW data );
	public string sBaseUrl;
	public string sHash;
	public string sTestString;
	
	
	//------------------------------------------------------------------------
	// Globals, GUI Windows
	//------------------------------------------------------------------------
	[HideInInspector] public Query eQuery;
	
	
	//========================================================================
	// void Start()
	//------------------------------------------------------------------------
	// Performs empty check and begins connection test.
	//========================================================================
	void Awake()
	{	
		if( kDirector   == null ) Debug.LogError( "BaseUrl cannot be empty." );
		if( sBaseUrl    == ""   ) Debug.LogError( "BaseUrl cannot be empty." );
		if( sHash       == ""   ) Debug.LogError( "Hash cannot be empty."    );
		if( sTestString == ""   ) Debug.LogError( "Hash cannot be empty."    );

		//Encrypt Hash
		sHash = Md5Sum( sHash );
	}	


	//========================================================================
	// Start
	//------------------------------------------------------------------------
	// Pings the PHPHandler.php script/server.
	//========================================================================	
	public void Init()
	{
		StartCoroutine( Ping( true ) );
	}


	//========================================================================
	// void Request() -- Overloads
	//------------------------------------------------------------------------
	// These are the overload methods for our primary Request() method. They
	// allow for various ways to call the request, each passing the initial
	// call to another method, until finally the primary method.
	//========================================================================
	public void Request( Action eAction )
	{
		Request( eAction, null );
	}

	public void Request( WWWForm wForm, Action eAction )
	{
		Request( wForm, eAction, null );
	}
	
	public void Request( Action eAction, Callback callback )
	{
		WWWForm wForm = new WWWForm();
		Request( wForm, eAction, callback );
	}
	

	//========================================================================
	// void Request()
	//------------------------------------------------------------------------
	// Primary Request(). Attaches the action and the hash (security) into the
	// WWWForm and calls the coroutine, Handle().
	//========================================================================	
	public void Request( WWWForm wForm, Action eAction, Callback callback )
	{
		wForm.AddField( "action", "" + eAction ); 
		wForm.AddField( "hash", sHash );	

		StartCoroutine( Handle( new WWW( sBaseUrl, wForm ), callback ) );
	}


	//========================================================================
	// void RequestObj()
	//------------------------------------------------------------------------
	// Attaches the action and the hash (security) into the WWWForm and calls
	// the coroutine, HandleTexture().
	//========================================================================	
	public void RequestObj( string url, CallbackTexture callback )
	{	
		StartCoroutine( HandleObj( new WWW( url ), callback ) );
	}


	//========================================================================
	// void RequestTexture()
	//------------------------------------------------------------------------
	// Attaches the action and the hash (security) into the WWWForm and calls
	// the coroutine, HandleTexture().
	//========================================================================	
	public void RequestTexture( string url, CallbackTexture callback )
	{	
		StartCoroutine( HandleTexture( new WWW( url ), callback ) );
	}	


	//========================================================================
	// IEnumerator Handle()
	//------------------------------------------------------------------------
	// Handle is called as a coroutine, as the length of time to receive a
	// response from the PHP script is undetermined. A coroutine is needed so
	// the application does not hang. Executes a callback once the retrieval
	// of data is complete.
	//
	// Pass the data variable of the www to the callback.
	//========================================================================	
	public IEnumerator Handle( WWW www, Callback callback )
	{	
		yield return www;

    	if( callback != null )    
			callback( www.data );
		
		www.Dispose();
	}


	//========================================================================
	// IEnumerator HandleObj()
	//------------------------------------------------------------------------
	// Handle is called as a coroutine, as the length of time to receive a
	// response from the PHP script is undetermined. A coroutine is needed so
	// the application does not hang. Executes a callback once the retrieval
	// of data is complete.
	//
	// Pass the entire www result to the callback.
	//========================================================================	
	public IEnumerator HandleObj( WWW www, CallbackTexture callback )
	{	
		yield return www;

    	if( callback != null )  
			callback( www );
		
		www.Dispose();
	}


	//========================================================================
	// IEnumerator HandleTexture()
	//------------------------------------------------------------------------
	// Handle is called as a coroutine, as the length of time to receive a
	// response from the PHP script is undetermined. A coroutine is needed so
	// the application does not hang. Executes a callback once the retrieval
	// of data is complete.
	//
	// Pass the entire www result to the callback.
	//========================================================================	
	public IEnumerator HandleTexture( WWW www, CallbackTexture callback )
	{	
		yield return www;

    	if( callback != null )  
			callback( www );
		
		www.Dispose();
	}
	

	//========================================================================
	// IEnumerator Ping()
	//------------------------------------------------------------------------
	// Initiates Ping attempt and displays connection result to the Log.
	//========================================================================	
	IEnumerator Ping( bool bIsStart )
	{
		Log.Add( "Pinging Server..." );

		eQuery = PHPHandler.Query.EN_Sending;		
		Request( PHPHandler.Action.EN_Ping, PingResult );
		
		while( eQuery == PHPHandler.Query.EN_Sending )
			yield return new WaitForSeconds( 0 );			

		
		string sConnection = "Connection ";
		Log.Type eType = Log.Type.EN_Message;
		
		if( eQuery == PHPHandler.Query.EN_Fail )
		{
			sConnection += "Failed.";
			eType = Log.Type.EN_Error;
		}
		
		else if( eQuery == PHPHandler.Query.EN_Success )
			sConnection += "Established.";
		
		Log.Add( sConnection, eType );
		
		yield return new WaitForSeconds( 0 );
		
		if( bIsStart ) {
			kDirector.kUserManager.GetUserID();
		}
	}


	//========================================================================
	// void PingResult()
	//------------------------------------------------------------------------
	// Callback Method. Compares the retrieved data to a predetermined string.
	// This is done www.error only catches an connection problems, not 404 
	// page errors.
	//========================================================================	
	public void PingResult( string data )
	{
		if( data == sTestString )
			eQuery = Query.EN_Success;
			
		else
			eQuery = Query.EN_Fail;
	}
	

	//========================================================================
	// string Md5Sum()
	//------------------------------------------------------------------------
	// Converts a string to md5 encryption.
	//========================================================================
	public string Md5Sum( string strToEncrypt )
	{
	    System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
	    byte[] bytes = ue.GetBytes(strToEncrypt);

	    System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
	    byte[] hashBytes = md5.ComputeHash( bytes );

	    string hashString = "";

	    for ( int i = 0; i < hashBytes.Length; i++ )
	        hashString += System.Convert.ToString( hashBytes[i], 16 ).PadLeft( 2, '0' );

	    return hashString.PadLeft( 32, '0' );
	}
}