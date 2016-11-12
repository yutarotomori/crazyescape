using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassesCtrl : StageObjectCtrl
{
	public Transform[] _grasses;


	private float m_Time;


	public override void Initialize (GetSpeed getSpeed)
	{
		base.Initialize (getSpeed);
	}


	private void Update ()
	{
		var length = _grasses.Length;
		for (int i = 0; i < length; i++) {
			var grass = _grasses [i];
			var position = grass.localPosition;
			position.z = -10.0f + 10.0f * Mathf.Repeat (i - m_Time, length);
			grass.localPosition = position;
		}

		m_Time = Mathf.Repeat (m_Time + getSpeed.Invoke () * 0.1f * 60.0f * Time.deltaTime, length);
	}
}
