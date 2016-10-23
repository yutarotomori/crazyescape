using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIInputCtrl : MonoBehaviour
{
	private InputManager m_InputManager {
		get {
			return InputManager.GetInstance ();
		}
	}


	public EventTrigger _leftTrigger;
	public EventTrigger _rightTrigger;
	public EventTrigger _jumpTrigger;


	private void Start ()
	{
		m_InputManager.SetLeftEvent (_leftTrigger.triggers);
		m_InputManager.SetRightEvent (_rightTrigger.triggers);
		m_InputManager.SetJumpEvent (_jumpTrigger.triggers);
	}
}
