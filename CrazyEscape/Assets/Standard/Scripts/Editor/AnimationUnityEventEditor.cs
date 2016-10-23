using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(AnimationUnityEvent))]
public class AnimationUnityEventEditor : Editor
{
	private	AnimationUnityEvent	m_Object;
	private	Animation	m_Animation;
	private	int			m_ClipCount;
	private	string[]	m_ClipNames;

	private	float	m_PrevTime;
	private	float	m_PrevFrame;

	public override void OnInspectorGUI ()
	{
		m_Object	= target as AnimationUnityEvent;
		m_Animation	= m_Object.GetComponent<Animation> ();
		m_ClipCount	= m_Animation.GetClipCount ();
		m_ClipNames	= new string[m_ClipCount];
		int i=0;
		foreach (AnimationState state in m_Animation) {
			m_ClipNames[i++]	= state.name;
		}

		serializedObject.Update ();
		base.OnInspectorGUI ();
		showAnimationsPopup ();
		showPropaties ();
		serializedObject.ApplyModifiedProperties ();
	}

	private	void	showAnimationsPopup ()
	{
		if (m_ClipCount == 0) {
		} else if (m_ClipCount > 1) {
			m_Object._targetClipIndex	= EditorGUILayout.Popup ("Clip", m_Object._targetClipIndex, m_ClipNames);
		} else {
			m_Object._targetClipIndex	= 0;
			EditorGUILayout.LabelField ("Clip", m_ClipNames[m_Object._targetClipIndex]);
		}
	}

	private	void	showPropaties ()
	{
		float	time	= m_Object._time;
		int		frame	= m_Object._frame;

		time	= EditorGUILayout.FloatField ("Time", m_Object._time);
		frame	= EditorGUILayout.IntField ("Frame", m_Object._frame);

		time	= Mathf.Clamp (time, 0.0f, m_Animation.GetClip (m_ClipNames[m_Object._targetClipIndex]).length);
		frame	= Mathf.Clamp (frame, 0, (int)(
			m_Animation.GetClip (m_ClipNames[m_Object._targetClipIndex]).length *
			m_Animation.GetClip (m_ClipNames[m_Object._targetClipIndex]).frameRate)
		                     );

		if (Application.isPlaying == false) {
			if (m_PrevTime != time) {
				m_PrevTime	= time;
				frame	= (int)(time*m_Animation.clip.frameRate);
			} else if (m_PrevFrame != frame) {
				m_PrevFrame	= frame;
				time	= (frame/m_Animation.clip.frameRate);
			}
			
			m_PrevTime	= time;
			m_PrevFrame	= frame;
		}

		m_Object._time	= time;
		m_Object._frame	= frame;
	}
}
