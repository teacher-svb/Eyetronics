using UnityEngine;
using System.Collections;

public class LoadObjectsCSharp : MonoBehaviour {
	
	GameObject[] GOArrayTargets = new GameObject[16];

	GameObject GOSource_RT;
	
	IEnumerator LoadObjects() {
		MorphTargetsCSharp MorphersTemp = (MorphTargetsCSharp)GOSource_RT.GetComponent("MorphTargetsCSharp");
		if (MorphersTemp == null) {
			GOSource_RT.AddComponent("MorphTargetsCSharp");
			MorphersTemp = (MorphTargetsCSharp)GOSource_RT.GetComponent("MorphTargetsCSharp");
		}
		
		MeshFilter MFTemp = (MeshFilter)GOSource_RT.GetComponent("MeshFilter");
		MorphersTemp.sourceMesh = MFTemp.mesh;
		
		yield return StartCoroutine(CreateGameObject("dimitri", "neutral", false));
		Debug.Log("target added");
		yield return StartCoroutine(CreateGameObject("target blink_Left", "blink_Left", true));
		Debug.Log("target added");
		yield return StartCoroutine(CreateGameObject("target blink_Right", "blink_Right", true));
		Debug.Log("target added");
		yield return StartCoroutine(CreateGameObject("target sad", "sad", true));
		Debug.Log("target added");
		yield return StartCoroutine(CreateGameObject("target scared", "scared", true));
		Debug.Log("target added");
		yield return StartCoroutine(CreateGameObject("target smile", "smile", true));
		Debug.Log("target added");
		yield return StartCoroutine(CreateGameObject("source", "sam", true));
		Debug.Log("source added");
		
		//~ MorphersTemp.Init();
	}
	
	void AddMorphTarget(ref GameObject goTarget) {
		MeshFilter MFTemp = (MeshFilter)goTarget.GetComponent("MeshFilter");
		MorphTargetsCSharp MorphersTemp = (MorphTargetsCSharp)GOSource_RT.GetComponent("MorphTargetsCSharp");
		
		MorphersTemp.AddBlendMesh(goTarget.name, MFTemp.mesh);
		goTarget.renderer.enabled = false;
	}
	
	IEnumerator CreateGameObject(string GOname, string meshName, bool isTarget) {
		GameObject GOTemp = new GameObject();
		GOTemp.name = GOname;
		GOTemp.AddComponent("objReaderCSharpV3");
		objReaderCSharpV3 objReaderTemp = (objReaderCSharpV3)GOTemp.GetComponent("objReaderCSharpV3");
		objReaderTemp._textFieldString = "http://howest.stage.eyebcom.com/" + meshName + ".obj";
		if (!isTarget)
			objReaderTemp._textureLink = "http://howest.stage.eyebcom.com/sam.jpg";
		yield return StartCoroutine(objReaderTemp.SomeFunction());
		if (isTarget) {
			for (int i = 0; i < GOArrayTargets.Length; ++i) {
				if (GOArrayTargets[i] == null) {
					GOArrayTargets[i] = GOTemp;
					AddMorphTarget(ref GOArrayTargets[i]);
					break;
				}
			}
		}
		else
			GOSource_RT = GOTemp;
	}
}
