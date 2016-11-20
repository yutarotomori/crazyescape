using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCtrl : StageObjectCtrl
{
	protected virtual void Update ()
	{
		transform.Translate (Vector3.forward * getSpeed.Invoke () * -1.0f, Space.World);
	}


	private void OnBecameInvisible ()
	{
		gameObject.SetActive (false);
	}
}
