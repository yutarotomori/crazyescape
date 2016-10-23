using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class InputManager : Singleton<InputManager>
{
	public delegate void OnInputEvent ();

	private event OnInputEvent m_OnInputLeft;
	private event OnInputEvent m_OnInputRight;
	private event OnInputEvent m_OnInputJump;


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
}
