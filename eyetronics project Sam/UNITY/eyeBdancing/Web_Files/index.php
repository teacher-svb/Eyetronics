<?php
require_once "facebook-platform/php/facebook.php";

$appapikey = "a5448e7ec2b2b86c191326fcb19a01d0";
$appsecret = "d6efc5076463b7eef5a03f7992f4f4bb";
$facebook  = new Facebook( $appapikey, $appsecret );
$userId    = $facebook->require_login();

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
			<?php 
				require( "template/page.php" ); 
			?>
		</center>
		<script type="text/javascript" >
		<!--
		function GetUserID()
		{	
		  GetUnity().SendMessage( "Director", "PutUserFb_ID", "<?= $userId; ?>;1" );
		}
		-->
		</script>
	</body>
</html>