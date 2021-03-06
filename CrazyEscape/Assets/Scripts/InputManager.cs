﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class InputManager : Singleton<InputManager>
{
	public delegate void OnInputEvent ();


	private TouchManager m_TouchManager {
		get {
			return TouchManager.GetInstance ();
		}
	}

	private event OnInputEvent m_OnInputLeft;
	private event OnInputEvent m_OnInputRight;
	private event OnInputEvent m_OnInputJump;

	private bool m_IsSwipe;
	private int m_FingerID;


	public void AddInputLeftEvent (OnInputEvent iEvent)
	{
		if (m_OnInputLeft == null) {
			m_OnInputLeft = new OnInputEvent (iEvent);
		} else {
			m_OnInputLeft += iEvent;
		}
	}

	public void RemoveInputLeftEvent (OnInputEvent iEvent)
	{
		if (m_OnInputLeft != null) {
			m_OnInputLeft -= iEvent;
		}
	}

	public void AddInputRightEvent (OnInputEvent iEvent)
	{
		if (m_OnInputRight == null) {
			m_OnInputRight = new OnInputEvent (iEvent);
		} else {
			m_OnInputRight += iEvent;
		}
	}

	public void RemoveInputRightEvent (OnInputEvent iEvent)
	{
		if (m_OnInputRight != null) {
			m_OnInputRight -= iEvent;
		}
	}

	public void AddInputJumpEvent (OnInputEvent iEvent)
	{
		if (m_OnInputJump == null) {
			m_OnInputJump = new OnInputEvent (iEvent);
		} else {
			m_OnInputJump += iEvent;
		}
	}

	public void RemoveInputJumpEvent (OnInputEvent iEvent)
	{
		if (m_OnInputJump != null) {
			m_OnInputJump -= iEvent;
		}
	}

	public void SetLeftEvent (List<EventTrigger.Entry> iTriggers)
	{
		EntryInputEvent (iTriggers, EventTriggerType.PointerDown, (arg0) => {
			if (m_OnInputLeft != null) {
				m_OnInputLeft.Invoke ();
			}
		});
	}

	public void SetRightEvent (List<EventTrigger.Entry> iTriggers)
	{
		EntryInputEvent (iTriggers, EventTriggerType.PointerDown, (arg0) => {
			if (m_OnInputRight != null) {
				m_OnInputRight.Invoke ();
			}
		});
	}

	public void SetJumpEvent (List<EventTrigger.Entry> iTriggers)
	{
		EntryInputEvent (iTriggers, EventTriggerType.PointerDown, (arg0) => {
			if (m_OnInputJump != null) {
				m_OnInputJump.Invoke ();
			}
		});
	}


	static private void EntryInputEvent (
		List<EventTrigger.Entry> iTriggers,
		EventTriggerType iTriggerType,
		UnityAction<BaseEventData> iEvent)
	{
		var entry = new EventTrigger.Entry ();
		entry.eventID = iTriggerType;
		entry.callback = new EventTrigger.TriggerEvent ();
		entry.callback.AddListener (iEvent);
		iTriggers.Add (entry);
	}


	private void Update ()
	{
		CheckButton ();
		CheckTouch ();
	}

	private void CheckButton ()
	{
		if (Input.GetKeyDown (KeyCode.A) || Input.GetKeyDown (KeyCode.LeftArrow)) {
			m_OnInputLeft.Invoke ();
		}
		if (Input.GetKeyDown (KeyCode.D) || Input.GetKeyDown (KeyCode.RightArrow)) {
			m_OnInputRight.Invoke ();
		}
		if (Input.GetKeyDown (KeyCode.W) || Input.GetKeyDown (KeyCode.UpArrow) || Input.GetKeyDown (KeyCode.Space)) {
			m_OnInputJump.Invoke ();
		}
	}

	private void CheckTouch ()
	{
		if (m_IsSwipe) {
			if (m_TouchManager.EqualPhase (TouchPhase.Ended, m_FingerID)) {
				m_IsSwipe = false;
			}
		} else {
			if (m_TouchManager.EqualPhase (TouchPhase.Moved, out m_FingerID)) {
				if (m_TouchManager.Swipe4Direction (m_FingerID).x <= -1.0f
				    && Mathf.Abs (m_TouchManager.Movement (m_FingerID).x) > 50.0f) {
					m_OnInputLeft.Invoke ();
					m_IsSwipe = true;
				} else if (m_TouchManager.Swipe4Direction ().x >= 1.0f
				    && Mathf.Abs (m_TouchManager.Movement (m_FingerID).x) > 50.0f) {
					m_OnInputRight.Invoke ();
					m_IsSwipe = true;
				}
//				if (m_TouchManager.Swipe4Direction ().y >= 1.0f) {
//					m_OnInputJump.Invoke ();
//					m_IsSwipe = true;
//				}
			}
		}
	}
}
