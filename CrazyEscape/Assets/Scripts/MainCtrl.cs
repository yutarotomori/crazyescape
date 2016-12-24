using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCtrl : Singleton<MainCtrl>
{
	static public float fixTime {
		get {
			return Application.targetFrameRate * Time.deltaTime;
		}
	}


	public int _targetFrameRate = 60;
	public AudioSource _BGMSource;


	public void PlayBGM(string bgmPath)
	{
		StartCoroutine(PlayBGMAsync(bgmPath));
	}


	private IEnumerator PlayBGMAsync(string bgmPath)
	{
		if (_BGMSource.clip != null && _BGMSource.isPlaying) {
			_BGMSource.Stop ();
			_BGMSource.clip = null;
		}

		var req = Resources.LoadAsync<AudioClip> (bgmPath);
		yield return req;

		_BGMSource.clip = req.asset as AudioClip;
		_BGMSource.Play ();
	}

	protected override void Initialize ()
	{
		base.Initialize ();
		Application.targetFrameRate = _targetFrameRate;
		Screen.fullScreen = false;
	}
}
