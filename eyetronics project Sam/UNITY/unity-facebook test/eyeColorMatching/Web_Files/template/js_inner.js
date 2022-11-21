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