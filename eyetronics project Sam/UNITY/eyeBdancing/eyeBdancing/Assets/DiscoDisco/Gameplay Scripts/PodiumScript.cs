using UnityEngine;
using System.Collections;

public class PodiumScript : MonoBehaviour {
	public Texture[] _txrArr;
	
	int _teller = 0;
	
	public void ChangeTexture() {
		_teller++;
		gameObject.GetComponent<MeshRenderer>().material.mainTexture = _txrArr[_teller % 5];
	}
}
