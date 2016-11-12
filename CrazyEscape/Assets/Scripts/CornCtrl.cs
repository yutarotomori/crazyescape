using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CornCtrl : ItemCtrl
{
	public Transform _corn;

	protected override void Update ()
	{
		base.Update ();

		_corn.Rotate (Vector3.up, 1.0f, Space.World);
	}
}
