using UnityEngine;
using System.Collections;

public class headAttacher : MonoBehaviour {
	public Transform _myTarget;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		_myTarget.position = transform.position;
		_myTarget.rotation = transform.rotation;
		_myTarget.Translate(0,0.02f,0,0);
		_myTarget.Rotate(Vector3.up, 90);
		_myTarget.Rotate(Vector3.left, 90);
	}
}
