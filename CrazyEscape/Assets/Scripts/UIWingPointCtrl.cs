using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWingPointCtrl : MonoBehaviour
{
	public Image _blank;


	public bool isEnable {
		get {
			return !_blank.enabled;
		}
	}


	public void setEnable(bool iIsEnable)
	{
		_blank.enabled = !iIsEnable;
	}
}
