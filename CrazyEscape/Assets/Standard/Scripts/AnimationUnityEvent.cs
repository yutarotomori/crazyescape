using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class AnimationUnityEvent : MonoBehaviour
{
	public	UnityEvent	_event;

	[HideInInspector]public	int	_targetClipIndex;
	[HideInInspector]public	float	_time;
	[HideInInspector]public	int		_frame;

	private	UnityEngine.AnimationEvent	m_AnimationEventValue;
	private	UnityEngine.AnimationEvent	m_AnimationEvent {
		get {
			if (m_AnimationEventValue == null) {
				m_AnimationEventValue	= new UnityEngine.AnimationEvent ();
				m_AnimationEventValue.functionName	= "animationEvent";
				m_AnimationEventValue.intParameter	= GetInstanceID ();
			}
			return	m_AnimationEventValue;
		}
	}

	private	Animation	m_AnimationValue;
	private	Animation	m_Animation {
		get {
			if (m_AnimationValue == null) {
				m_AnimationValue	= GetComponent<Animation> ();
			}
			return	m_AnimationValue;
		}
	}

	private	void	Awake ()
	{
		addEvent (_time);
	}

	private	bool	addEvent (float iTime)
	{
		if (m_Animation == null) {
			return	false;
		}

		m_AnimationEvent.time	= iTime;
		int aIndex = 0;
		foreach (AnimationState aState in m_Animation) {
			if (_targetClipIndex == aIndex++) {
				aState.clip.AddEvent (m_AnimationEvent);
				break;
			}
		}
		return	true;
	}

	public	void	animationEvent (int iInstanceID)
	{
		if (iInstanceID == GetInstanceID ()) {
			_event.Invoke ();
		}
	}
}
