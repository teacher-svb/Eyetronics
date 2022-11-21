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

$link = mysql_connect( "localhost", "sam", "sam" ) or die( mysql_error() );
$db   = mysql_select_db( "sam", $link ) or die( mysql_error() );

?>