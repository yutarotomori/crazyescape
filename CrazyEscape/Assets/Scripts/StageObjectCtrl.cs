using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageObjectCtrl : MonoBehaviour
{
	public delegate float GetSpeed ();


	protected GetSpeed getSpeed = () => {
		return 0.1f;
	};


	public virtual void Initialize (GetSpeed getSpeed)
	{
		this.getSpeed = getSpeed;
	}
}
