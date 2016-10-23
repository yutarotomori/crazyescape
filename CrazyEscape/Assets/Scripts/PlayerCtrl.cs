using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
	private enum PositionType
	{
		Left,
		Center,
		Right,
	}


	private InputManager m_InputManager {
		get {
			return InputManager.GetInstance ();
		}
	}


	public Transform _model;
	public Transform _shadow;
	public float _moveSideSpeed = 0.05f;
	public float _jumpSpeed = 5.0f;


	private PositionType m_PositionType = PositionType.Center;
	private IEnumerator m_MoveSideIterator;
	private IEnumerator m_JumpIterator;


	private void OnEnable ()
	{
		m_InputManager.AddInputLeftEvent (MoveLeft);
		m_InputManager.AddInputRightEvent (MoveRight);
		m_InputManager.AddInputJumpEvent (Jump);
	}

	private void OnDisable ()
	{
		m_InputManager.RemoveInputLeftEvent (MoveLeft);
		m_InputManager.RemoveInputRightEvent (MoveRight);
		m_InputManager.RemoveInputJumpEvent (Jump);
	}

	private void MoveLeft ()
	{
		if (m_MoveSideIterator == null) {
			switch (m_PositionType) {
			case PositionType.Left:
				m_MoveSideIterator = null;
				break;
			case PositionType.Center:
				m_MoveSideIterator = MoveSideAsync (0.0f, -0.5f);
				m_PositionType = PositionType.Left;
				break;
			case PositionType.Right:
				m_MoveSideIterator = MoveSideAsync (0.5f, 0.0f);
				m_PositionType = PositionType.Center;
				break;
			default:
				m_MoveSideIterator = null;
				break;
			}

			if (m_MoveSideIterator != null) {
				StartCoroutine (m_MoveSideIterator);
			}
		}
	}

	private void MoveRight ()
	{
		if (m_MoveSideIterator == null) {
			switch (m_PositionType) {
			case PositionType.Left:
				m_MoveSideIterator = MoveSideAsync (-0.5f, 0.0f);
				m_PositionType = PositionType.Center;
				break;
			case PositionType.Center:
				m_MoveSideIterator = MoveSideAsync (0.0f, 0.5f);
				m_PositionType = PositionType.Right;
				break;
			case PositionType.Right:
				m_MoveSideIterator = null;
				break;
			default:
				m_MoveSideIterator = null;
				break;
			}
			if (m_MoveSideIterator != null) {
				StartCoroutine (m_MoveSideIterator);
			}
		}
	}

	private void Jump ()
	{
		if (m_JumpIterator == null) {
			m_JumpIterator = JumpAsync ();
			StartCoroutine (m_JumpIterator);
		}
	}

	private IEnumerator MoveSideAsync (float iFrom, float iTo)
	{
		var modelPosition = _model.localPosition;
		var shadowPosition = _shadow.localPosition;
		for (float time = 0.0f; time < 1.0f; time += _moveSideSpeed) {
			modelPosition = _model.localPosition;
			modelPosition.x = Vector3.Slerp (Vector3.right * iFrom, Vector3.right * iTo, time).x;
			_model.localPosition = modelPosition;

			shadowPosition.x = modelPosition.x;
			_shadow.localPosition = shadowPosition;
			yield return 0;
		}
		modelPosition.x = iTo;
		_model.localPosition = modelPosition;

		shadowPosition.x = modelPosition.x;
		_shadow.localPosition = shadowPosition;
		m_MoveSideIterator = null;
	}

	private IEnumerator JumpAsync ()
	{
		var modelPosition = _model.localPosition;
		var shadowScale = _shadow.localScale;
		for (float time = 0.0f; time < 180.0f; time += _jumpSpeed) {
			modelPosition = _model.localPosition;
			modelPosition.y = Mathf.Sin (time * Mathf.Deg2Rad);
			_model.localPosition = modelPosition;
			_shadow.localScale = shadowScale * (1.0f - Mathf.Sin (time * Mathf.Deg2Rad) * 0.3f);
			yield return 0;
		}
		modelPosition.y = 0.0f;
		_model.position = modelPosition;
		_shadow.localScale = shadowScale;
		m_JumpIterator = null;
	}
}
