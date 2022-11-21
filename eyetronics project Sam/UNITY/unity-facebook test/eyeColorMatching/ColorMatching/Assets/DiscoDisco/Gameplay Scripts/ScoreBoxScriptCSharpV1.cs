using UnityEngine;
using System.Collections;

public class ScoreBoxScriptCSharpV1 : MonoBehaviour {
	
	public GameObject[] _wheelArray;
	public int[] _scoreArray;
	public int _score;
	public int _feverScore;
	float _feverScoreFloat;

	// Use this for initialization
	void Start () {
		_scoreArray = new int[_wheelArray.Length];
		for (int i = 0; i < _scoreArray.Length; ++i)
			_scoreArray[i] = 0;
	}
	
	// Update is called once per frame
	void Update () {
		float smooth = 5.0f;
		//~ if (GameObject.Find("GameManager").GetComponent<GameManagerCSharpV1>().GameState == GameManagerCSharpV1.gameStates.GS_playing) {
			for (int i = 0; i < _scoreArray.Length; ++i) {
				float tiltAngle = 36.0f * _scoreArray[i];
				Quaternion target = Quaternion.Euler (tiltAngle+270, 90,90);
				// Dampen towards the target rotation
				_wheelArray[i].transform.localRotation = Quaternion.Slerp(_wheelArray[i].transform.localRotation, target, Time.deltaTime * smooth);
			}
		//~ }
	}
	
	public void AddScore(int scoreToAdd) {
		_score += scoreToAdd;
		_feverScoreFloat = (scoreToAdd > 0) ? _feverScoreFloat+1.0f : 0.0f;
		
		_feverScore = System.Convert.ToInt32(_feverScoreFloat/2);
		if (_feverScore > 5) _feverScore = 5;
		
		string s = "" + _score;
		int temp = s.Length;
		s = "0";
		for (int i = 0; i < 5 - temp - 1; ++i)
			s = s + "0";
		 s = s + _score;
		
		for (int i = 0; i < s.Length; ++i)
			_scoreArray[i] = s[s.Length - 1 - i] - '0';
	}
}
