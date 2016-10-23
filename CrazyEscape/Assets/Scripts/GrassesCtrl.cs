using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassesCtrl : MonoBehaviour
{
	public Transform[] _grasses;


	private float m_Time;


	private void Update ()
	{
		var length = _grasses.Length;
		for (int i = 0; i < length; i++) {
			var grass = _grasses [i];
			var position = grass.localPosition;
			position.z = -10.0f + 10.0f * Mathf.Repeat (i - m_Time, length);
			grass.localPosition = position;
		}

		m_Time = Mathf.Repeat (m_Time + 0.03f, length);
	}
}
