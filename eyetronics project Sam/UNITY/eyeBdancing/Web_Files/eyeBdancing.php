<?php 
	mysql_connect("localhost", "sam", "sam");
	mysql_select_db("sam");
	$login = $_POST['eyeBcom_login'];
	$pw = $_POST['eyeBcom_pw'];
	if ($login != "" && $pw != "") {
		$query = "SELECT * FROM fb_users WHERE eyebcom_name = '$login' AND eyebcom_pw = '$pw'";
		$result = mysql_query($query) or die(mysql_error());
		while ($row = mysql_fetch_array($result))
		{
			$id = $row["eyeBcom_id"];
		}
	}
?>

<html>
	
	<head>
		<title>Unity Web Player - game</title>
		<script language='VBScript'>
		function DetectUnityWebPlayerActiveX
			on error resume next
			dim tControl, res, ua, re, matches, major
			res = 0
			set tControl = CreateObject("UnityWebPlayer.UnityWebPlayer.1")
			if IsObject(tControl) then
				if tControl.GetPluginVersion() = "2.5.0f5" then
					' 2.5.0f5 on Vista and later has an auto-update issue
					' on Internet Explorer. Detect Vista (6.0 or later)
					' and in that case treat it as not installed
					ua = Navigator.UserAgent
					set re = new RegExp
					re.Pattern = "Windows NT (\d+)\."
					set matches = re.Execute(ua)
					if matches.Count = 1 then
						major = CInt(matches(0).SubMatches(0))
						if major < 6 then
							res = 1
						end if
					end if
				else
					res = 1
				end if
			end if
			DetectUnityWebPlayerActiveX = res
		end function
		</script>
		<script language="javascript1.1" type="text/javascript">
		
			function GetUnity () {
				if (navigator.appVersion.indexOf("MSIE") != -1 && navigator.appVersion.toLowerCase().indexOf("win") != -1)
					return document.getElementById("UnityObject");
				else if (navigator.appVersion.toLowerCase().indexOf("safari") != -1)
					return document.getElementById("UnityObject");
				else
					return document.getElementById("UnityEmbed");
			}
			
			function DetectUnityWebPlayer () {
        		var tInstalled = false;
        		if (navigator.appVersion.indexOf("MSIE") != -1 && navigator.appVersion.toLowerCase().indexOf("win") != -1) {
					tInstalled = DetectUnityWebPlayerActiveX();
        		}
        		else {
            		if (navigator.mimeTypes && navigator.mimeTypes["application/vnd.unity"]) {
                		if (navigator.mimeTypes["application/vnd.unity"].enabledPlugin && navigator.plugins && navigator.plugins["Unity Player"]) {
                 			tInstalled = true;	
            			}
         			}	
        		}
        		return tInstalled;	
    		}
    		
    		function GetInstallerPath () {
    			var tDownloadURL = "";
	   			var hasXpi = navigator.userAgent.toLowerCase().indexOf( "firefox" ) != -1;
	   			
    			// Use standalone installer
    			if (1)
    			{
					if (navigator.platform == "MacIntel")
						tDownloadURL = "http://webplayer.unity3d.com/download_webplayer-2.x/webplayer-i386.dmg";
					else if (navigator.platform == "MacPPC")
						tDownloadURL = "http://webplayer.unity3d.com/download_webplayer-2.x/webplayer-ppc.dmg";
					else if (navigator.platform.toLowerCase().indexOf("win") != -1)
						tDownloadURL = "http://webplayer.unity3d.com/download_webplayer-2.x/UnityWebPlayer.exe";
					return tDownloadURL;
    			}
    			// Use XPI installer
				else
				{
					if (navigator.platform == "MacIntel")
						tDownloadURL = "http://webplayer.unity3d.com/download_webplayer-2.x/UnityWebPlayerOSX.xpi";
					else if (navigator.platform == "MacPPC")
						tDownloadURL = "http://webplayer.unity3d.com/download_webplayer-2.x/UnityWebPlayerOSX.xpi";
					else if (navigator.platform.toLowerCase().indexOf("win") != -1)
						tDownloadURL = "http://webplayer.unity3d.com/download_webplayer-2.x/UnityWebPlayerWin32.xpi";
					return tDownloadURL;
				}    			
    		}
			
			function AutomaticReload () {
				navigator.plugins.refresh();
				if (DetectUnityWebPlayer())
					window.location.reload();

				setTimeout('AutomaticReload()', 500)
			}
			
		</script>
	</head>

	<body>
		<center>
		
			<h2>Unity Web Player - game</h2>			
			
			
			<script language="javascript1.1" type="text/javaScript">
				var hasUnity = DetectUnityWebPlayer();
				var brokenUnity = false;
				if (hasUnity) {
					
					document.write('<object id="UnityObject" classid="clsid:444785F1-DE89-4295-863A-D46C3A781394" width="600" height="450"> \n');
					document.write('  <param name="src" value="game.unity3d" /> \n');
					document.write('  <embed id="UnityEmbed" src="game.unity3d" width="600" height="450" type="application/vnd.unity" pluginspage="http://www.unity3d.com/unity-web-player-2.x" /> \n');
					document.write('</object>');
					
					// if Unity does not define to GetPluginVersion on Safari on 10.6, we presume the plugin
					// failed to load because it is not compatible with 64-bit Safari.
					if (navigator.appVersion.indexOf("Safari") != -1
						&& navigator.appVersion.indexOf("Mac OS X 10_6") != -1
						&& document.getElementById("UnityEmbed").GetPluginVersion == undefined)
						brokenUnity = true;

					// 2.5.0 cannot auto update on ppc. Treat as broken.
					else if (document.getElementById("UnityEmbed").GetPluginVersion() == "2.5.0f5" 
						&& navigator.platform == "MacPPC")
						brokenUnity = true;
				}
				if (!hasUnity || brokenUnity) {
				
					var installerPath = GetInstallerPath();
					if (installerPath != "") {
						// Place a link to the right installer depending on the platform we are on. The iframe is very important! Our goals are:
						// 1. Don't have to popup new page
						// 2. This page still remains active, so our automatic reload script will refresh the page when the plugin is installed
						document.write('<div align="center" id="UnityPrompt"> \n');
						if (brokenUnity)
							document.write('  <a href= ' + installerPath + '><img src="http://webplayer.unity3d.com/installation/getunityrestart.png" border="0"/></a> \n');
						else
							document.write('  <a href= ' + installerPath + '><img src="http://webplayer.unity3d.com/installation/getunity.png" border="0"/></a> \n');
						document.write('</div> \n');
						
						// By default disable ActiveX cab installation, because we can't make a nice Install Now button
//						if (navigator.appVersion.indexOf("MSIE") != -1 && navigator.appVersion.toLowerCase().indexOf("win") != -1)
						if (0)
						{	
							document.write('<div id="InnerUnityPrompt"> <p>Title</p>');
							document.write('<p> Contents</p>');
							document.write("</div>");

							var innerUnityPrompt = document.getElementById("InnerUnityPrompt");
							
							var innerHtmlDoc =
								'<object id="UnityInstallerObject" classid="clsid:444785F1-DE89-4295-863A-D46C3A781394" width="320" height="50" codebase="http://webplayer.unity3d.com/download_webplayer-2.x/UnityWebPlayer.cab#version=2,0,0,0">\n' + 
							    '</object>';
							    
							innerUnityPrompt.innerHTML = innerHtmlDoc;
						}

						document.write('<iframe name="InstallerFrame" height="0" width="0" frameborder="0"></iframe>\n');
					}
					else {
						document.write('<div align="center" id="UnityPrompt"> \n');
						if (brokenUnity)
							document.write('  <a href="javascript: window.open("http://www.unity3d.com/unity-web-player-2.x"); "><img src="http://webplayer.unity3d.com/installation/getunityrestart.png" border="0"/></a> \n');
						else
							document.write('  <a href="javascript: window.open("http://www.unity3d.com/unity-web-player-2.x"); "><img src="http://webplayer.unity3d.com/installation/getunity.png" border="0"/></a> \n');
						document.write('</div> \n');
					}
					
					// hide broken player
					if (brokenUnity)
						document.getElementById("UnityEmbed").height = 0;
						
					// Reload when detected unity plugin - but only if no previous plugin is installed 
					// - in that case a browser restart is needed.
					if (!brokenUnity)
						AutomaticReload();
				}
			
			</script>
			<noscript>
				<object id="UnityObject" classid="clsid:444785F1-DE89-4295-863A-D46C3A781394" width="600" height="450" codebase="http://webplayer.unity3d.com/download_webplayer-2.x/UnityWebPlayer.cab#version=2,0,0,0">
					<param name="src" value="game.unity3d" />
					<embed id="UnityEmbed" src="game.unity3d" width="600" height="450" type="application/vnd.unity" pluginspage="http://www.unity3d.com/unity-web-player-2.x" />
					<noembed>
						<div align="center">
							This content requires the Unity Web Player<br /><br />
							<a href="http://www.unity3d.com/unity-web-player-2.x">Install the Unity Web Player today!</a>
						</div>
					</noembed>
				</object>
			</noscript>
			
			<h5><a href="http://unity3d.com" style="text-decoration: none; color: black;"><i>Created with Unity &raquo;</i></a></h5>
			
		</center>
		
	<script type="text/javascript" >
	<!--
	function GetUserID()
	{	
  	  GetUnity().SendMessage( "Director", "PutUserID", "<?= $id; ?>;1" );
	}
	-->
	</script>
	</body>
	
</html>

