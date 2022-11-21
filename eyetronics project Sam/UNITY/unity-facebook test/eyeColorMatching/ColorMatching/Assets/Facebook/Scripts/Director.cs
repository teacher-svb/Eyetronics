//============================================================================
// Title:  Color Matching (Unity-Facebook)
//----------------------------------------------------------------------------
// File:   Director.cs
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
// Director
//----------------------------------------------------------------------------
// Provides linkage between public scripts.
//============================================================================
public class Director : MonoBehaviour
{
	public PHPHandler  PHP;
	public UserManager kUserManager;
	public Memory      kMemory;
}