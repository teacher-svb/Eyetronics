<?php
//============================================================================
// Title:  Color Matching (Unity-Facebook)
//----------------------------------------------------------------------------
// File:   index.php
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

require_once "facebook-platform/php/facebook.php";

$appapikey = "38a2440deb76b2d11d63dfa09e01670a";
$appsecret = "e5f8c637cbf98180cf2e57efc3aaa1fc";
$facebook  = new Facebook( $appapikey, $appsecret );
$userId    = $facebook->require_login();

//=======================================
//  Update Database
//=======================================
require( "api/process.php" );
?>
<html>
<head>
	<script type="text/vbscript" src="template/vbscript.vb"></script>
	<script type="text/javascript" src="template/js_outer.js"></script>
</head>
<body>
	<center>
		<script type="text/javascript" src="template/js_inner.js"></script>
		<?php require( "template/page.php" ); ?>
	</center>
	<script type="text/javascript" >
	<!--
	function GetUserID()
	{	
  	  GetUnity().SendMessage( "Director", "PutUserID", "<?= $userId; ?>;1" );
	}
	-->
	</script>
</body>
</html>