<?php
//============================================================================
// Title:  Color Matching (Unity-Facebook)
//----------------------------------------------------------------------------
// File:   phphandler.php
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


$action    = $_POST[ 'action' ];
$unityHash = $_POST[ 'hash'   ];

$phpHash = md5( "savabhashkey" );

if( $phpHash != $unityHash )
	exit;


switch ( $action )
{
	case "EN_Ping":
		Ping();
		break;
		
	case "EN_GetUser":
		GetUser( $link );
		break;

	case "EN_PutScore":
		PutScore( $link );
		break;
		
	case "EN_GetStats":
		GetStats( $link );
		break;
		
	case "EN_GetFriends":
		GetFriends( $link );
		break;
		
	case "EN_Log":
		LogError( $link );
		break;
	
		
	default:
		return;
}



//============================================================================
// function Ping()
//----------------------------------------------------------------------------
// Tests connection.
//============================================================================
function Ping()
{
	echo "Test.";
}


//============================================================================
// function GetUser()
//----------------------------------------------------------------------------
// Gets User Data by ID.
//============================================================================
function GetUser( $link )
{		
	$fb_id = $_POST[ "id" ];
		
	$query = "SELECT * FROM fb_users WHERE fb_id = '$fb_id'";		
	$results = mysql_query( $query, $link ) or die( mysql_error() );

	if( mysql_num_rows( $results ) > 0 ) 
	{	
		while( $result = mysql_fetch_array( $results ) )
		{
			$record  =       $result[ 'fb_id'   ];	
			$record .= ';' . $result[ 'f_name'  ];
			$record .= ';' . $result[ 'pic_url' ];
			$record .= ';' . $result[ 'score'   ];
		}
		
		echo $record;
	}
	else
		echo "N/A";

}


//============================================================================
// function PutScore()
//----------------------------------------------------------------------------
// Updates score value for player.
//============================================================================
function PutScore( $link )
{			 
	$fb_id = $_POST[ 'id'    ];
	$score = $_POST[ 'score' ];
		
	$query = "UPDATE fb_users SET score = '$score' WHERE fb_id = '$fb_id'";		

	mysql_query( $query, $link ) or die( mysql_error() );
	
	echo $fb_id;
}


//============================================================================
// function GetFriends()
//----------------------------------------------------------------------------
// Gets the Facebook IDs for friends of the user who have a score.
//============================================================================
function GetFriends( $link )
{
	$fb_id = $_POST[ 'id'    ];
	$query   = "SELECT fb_users.fb_id as fb_id FROM fb_users, fb_friends WHERE fb_users.score > 0 and fb_friends.fb_id = '$fb_id' AND fb_friends.friend_id = fb_users.fb_id ORDER BY fb_users.score";
	
	$results = mysql_query( $query, $link ) or die( mysql_error() );
	$friends = mysql_num_rows( $results );

	if( $friends > 0 ) 
	{	
		while( $result = mysql_fetch_array( $results ) )
		{	
			 $ids .= ";" . $result[ 'fb_id' ];
		}
	}
	//------------------------------------------------------------------------
	// The "X" is neccessary, and removed in the Unity app.
	//------------------------------------------------------------------------
	echo "X".$ids;
}


//============================================================================
// function GetStats()
//----------------------------------------------------------------------------
// Calculates database population and rank.
//============================================================================
function GetStats( $link )
{		
	$fb_id = $_POST[ "id" ];

	$query   = "SELECT fb_id, score FROM fb_users WHERE score > 0 ORDER BY score";		
	$results = mysql_query( $query, $link ) or die( mysql_error() );

	//------------------------------------------------------------------------
	// Count is the total number of user accounts with a score.
	//------------------------------------------------------------------------
	$count   = mysql_num_rows( $results );
	
	$globalRank = 1;
	$score = 0;
	
	//------------------------------------------------------------------------
	// GlobalRank starts at 1, and increases for each row until the user's row.
	//------------------------------------------------------------------------
	if( $count > 0 ) 
	{	
		while( $result = mysql_fetch_array( $results ) )
		{
			if( $fb_id == $result[ 'fb_id' ] )
			{
				$score = $result[ 'score' ];
				break;
			}
			
			$globalRank++;
		}

		$query   = "SELECT fb_users.fb_id as fb_id, fb_users.score as score FROM fb_users, fb_friends WHERE fb_users.score > 0 and fb_friends.fb_id = '$fb_id' AND fb_friends.friend_id = fb_users.fb_id ORDER BY fb_users.score";
		$results = mysql_query( $query, $link ) or die( mysql_error() );
		
		//------------------------------------------------------------------------
		// Friends is the total number of user accounts with a score who are
		// friends with the current user.
		//------------------------------------------------------------------------
		$friends = mysql_num_rows( $results );
	
		$localRank = 1;
	
		//------------------------------------------------------------------------
		// LocalRank starts at 1, and increases for each row until the user's row.
		//------------------------------------------------------------------------
		if( $friends > 0 && $score != 0 ) 
		{	
			while( $result = mysql_fetch_array( $results ) )
			{	
				if( $score > $result[ 'score' ] )
				$localRank++;
				
				if( $fb_id == $result[ 'fb_id' ] )
					break;
			}
		}
			
		$record .=       $count;
		$record .= ';' . $friends;
		$record .= ';' . $globalRank;
		$record .= ';' . $localRank;
		
		echo $record;
	}
}

mysql_close();
?>