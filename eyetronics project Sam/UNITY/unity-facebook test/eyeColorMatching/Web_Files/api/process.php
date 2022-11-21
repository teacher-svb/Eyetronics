<?php
//============================================================================
// Title:  Color Matching (Unity-Facebook)
//----------------------------------------------------------------------------
// File:   process.php
// Author: Nick Breslin (nickbreslin@gmail.com), Waddlefarm.org
//
// Copyright (c) Nick Breslin, 2009-2010. All Rights Reserved.
//----------------------------------------------------------------------------
//
// * This license notice may not be removed or altered.
// * All project files are free for non-commercial or commercial use.
// * I appreciate personal notification if any part of this project is used.
// * All profile files are provided "as is". No support or compatibility is to
//    assumed. I assume no responsible for results from using these files.
// * Donations are appreciated, please visit: http://www.waddlefarm.org/donate
//
//============================================================================

require( "database.php" );


//============================================================================
// function GetFBInfo()
//----------------------------------------------------------------------------
// Query Facebook for Account Information from Facebook.
//============================================================================
function GetFBInfo( $facebook, $id )
{
	$info=array();
	$info = $facebook->api_client->users_getInfo( $id, array( 'first_name', 'pic_square', 'is_app_user' ) );
	return $info[0]['first_name'] . ";" . $info[0]['pic_square'] . ";" . $info[0]['is_app_user'];	
}


//============================================================================
// function CheckTable()
//----------------------------------------------------------------------------
// Returns true of the ID belongs to a user account.
//============================================================================
function CheckTable( $id, $table, $link )
{	
	$query = "SELECT * FROM $table WHERE fb_id = '$id'";	
	
	$results = mysql_query( $query, $link ) or die( mysql_error() );

	if( mysql_num_rows( $results ) > 0 )
		return true;
		
	else
		return false;
}


//============================================================================
// function UpdateUser()
//----------------------------------------------------------------------------
// Requests Facebook information, process data and updates User table.
//============================================================================
function UpdateUser( $facebook, $id, $link )
{
	$infoString = GetFBInfo( $facebook, $id );

	$info = Explode( ";", $infoString );
	
	$f_name      = $info[0];
	$pic_url     = $info[1];
	$is_app_user = $info[2];
	
	
	//------------------------------------------------------------------------
	// If User is in Table and an App User, update row.
	//------------------------------------------------------------------------
	if ( CheckTable( $id, "fb_users", $link ) )
	{
		if( $is_app_user == "1" )
			$query = "UPDATE fb_users SET f_name = '$f_name', pic_url = '$pic_url', is_app_user = '$is_app_user' WHERE fb_id = '$id'";		
	}
	
	
	//------------------------------------------------------------------------
	// User is new to the app, and a new row is inserted.
	//------------------------------------------------------------------------
	else
		$query = "INSERT INTO fb_users ( fb_id, f_name, pic_url, is_app_user ) VALUES ( '$id', '$f_name', '$pic_url', '$is_app_user' )";
	
	mysql_query( $query, $link ) or die( mysql_error() );
}


//============================================================================
// function UpdateAllUser()
//----------------------------------------------------------------------------
// Remove non-app users from the database.
//============================================================================
function UpdateAllUsers( $link )
{


	//------------------------------------------------------------------------
	// If User is in Table and an App User, update row. If not, delete row.
	//------------------------------------------------------------------------
	$query = "SELECT * FROM fb_users";	
	
	$results = mysql_query( $query, $link ) or die( mysql_error() );

	if( mysql_num_rows( $results ) > 0 )
	{
		while( $result = mysql_fetch_array( $results ) )
		{
			if( $result[ 'is_app_user' ] == "0" )
			{
				$id = $result[ 'fb_id' ];
				$query = "DELETE FROM fb_users WHERE fb_id = '$id'";
				mysql_query( $query, $link ) or die( mysql_error() );
			}
		}
	}
}


//============================================================================
// function GetFriends()
//----------------------------------------------------------------------------
// Removed all previous friend connections, then updates user accounts of
// friends an re-establishes connections.
//============================================================================
function GetFriends( $facebook, $id, $link )
{


	//------------------------------------------------------------------------
	// Remove all previous friend connections.
	//------------------------------------------------------------------------
	$query = "DELETE FROM fb_friends WHERE fb_id = '$id'";	
	mysql_query( $query, $link ) or die( mysql_error() );


	//------------------------------------------------------------------------
	// Updates user accounts of friends and established friend connections.
	//------------------------------------------------------------------------
	$friends = $facebook->api_client->friends_get();

	foreach ( $friends as $friend )
	{
		if ( CheckTable( $friend, "fb_users", $link ) )
		{
			UpdateUser( $facebook, $friend, $link );

			$query = "INSERT INTO fb_friends ( fb_id, friend_id ) VALUES ( '$id', '$friend' )";	
			mysql_query( $query, $link ) or die( mysql_error() );
		}
	}
}


//============================================================================
// Updates the user and friend connections.
//============================================================================
UpdateUser( $facebook, $userId, $link );
UpdateAllUsers( $link );
GetFriends( $facebook, $userId, $link );
mysql_close();
?>