using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCtrl : Singleton<MainCtrl>
{
	public int _targetFrameRate = 60;

	protected override void Initialize ()
	{
		base.Initialize ();
		Application.targetFrameRate = _targetFrameRate;
		Screen.fullScreen = false;
	}
}
