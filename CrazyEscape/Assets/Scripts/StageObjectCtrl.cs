using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageObjectCtrl : MonoBehaviour
{
	public delegate float GetSpeed ();
	public delegate bool GetActiveCondition ();


	protected GetSpeed getSpeed = () => {
		return 0.1f;
	};

	protected GetActiveCondition getActiveCondition = () => {
		return true;
	};


	public virtual void Initialize (GetSpeed getSpeed, GetActiveCondition getActiveCondition)
	{
		this.getSpeed = getSpeed;
		this.getActiveCondition = getActiveCondition;
	}
}
