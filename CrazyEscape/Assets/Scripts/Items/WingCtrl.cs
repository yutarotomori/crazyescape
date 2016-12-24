using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WingCtrl : ItemCtrl
{
	public Transform _wing;


	protected override void Update ()
	{
		base.Update ();

		_wing.Rotate (Vector3.up, 1.0f, Space.World);
	}
}
