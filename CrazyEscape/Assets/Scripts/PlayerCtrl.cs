using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
	public delegate void OnHitItem ();
	public delegate void OnHitEnemy ();



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
	private IEnumerator moveDepthIterator;
	private OnHitItem m_OnHitCorn;
	private OnHitItem m_OnHitRock;
	private OnHitEnemy m_OnHitEnemy;
	private float m_Depth;
	[SerializeField]
	private float m_DepthMax = 3.0f;
	[SerializeField]
	private float m_DepthMin = -3.0f;


	public void Initialize (OnHitItem onHitCorn, OnHitItem onHitRock, OnHitEnemy onHitEnemy)
	{
		m_OnHitCorn = onHitCorn;
		m_OnHitRock = onHitRock;

		m_OnHitEnemy = onHitEnemy;
	}


	private void OnEnable ()
	{
		m_InputManager.AddInputLeftEvent (MoveLeft);
		m_InputManager.AddInputRightEvent (MoveRight);
		m_InputManager.AddInputJumpEvent (Jump);
	}

	private void OnDisable ()
	{
		if (m_InputManager != null) {
			m_InputManager.RemoveInputLeftEvent (MoveLeft);
			m_InputManager.RemoveInputRightEvent (MoveRight);
			m_InputManager.RemoveInputJumpEvent (Jump);
		}
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
		var modelPosition = transform.localPosition;
		for (float time = 0.0f; time < 1.0f; time += _moveSideSpeed * 60.0f * Time.deltaTime) {
			modelPosition = transform.localPosition;
			modelPosition.x = Vector3.Slerp (Vector3.right * iFrom, Vector3.right * iTo, time).x;
			transform.localPosition = modelPosition;

			yield return 0;
		}
		modelPosition.x = iTo;
		transform.localPosition = modelPosition;

		m_MoveSideIterator = null;
	}

	private IEnumerator JumpAsync ()
	{
		var modelPosition = transform.localPosition;
		var shadowPosition = _shadow.position;
		var shadowPositionY = _shadow.position.y;
		var shadowScale = _shadow.localScale;
		for (float time = 0.0f; time < 180.0f; time += _jumpSpeed * 60.0f * Time.deltaTime) {
			modelPosition = transform.localPosition;
			modelPosition.y = Mathf.Sin (time * Mathf.Deg2Rad);
			transform.localPosition = modelPosition;

			shadowPosition = _shadow.position;
			shadowPosition.y = shadowPositionY;
			_shadow.position = shadowPosition;
			_shadow.localScale = shadowScale * (1.0f - Mathf.Sin (time * Mathf.Deg2Rad) * 0.3f);
			yield return 0;
		}
		modelPosition.y = 0.0f;
		transform.localPosition = modelPosition;
		shadowPosition = _shadow.position;
		shadowPosition.y = shadowPositionY;
		_shadow.position = shadowPosition;
		_shadow.localScale = shadowScale;
		m_JumpIterator = null;
	}

	private IEnumerator moveDepthAsync (float iToDepth)
	{
		float fromDepth = transform.position.z;
		m_Depth = iToDepth;

		Vector3 position;
		for (float i = 0; i < 1.0f; i += 0.024f * 60.0f * Time.deltaTime) {
			var forward = Vector3.Slerp (Vector3.forward * fromDepth, Vector3.forward * iToDepth, i);
			position = transform.position;
			position.z = forward.z;
			transform.position = position;
			yield return 0;
		}

		position = transform.position;
		position.z = iToDepth;
		transform.position = position;

		moveDepthIterator = null;
	}

	private void moveDepth (float iToDepth)
	{
		if (moveDepthIterator != null) {
			StopCoroutine (moveDepthIterator);
		}

		moveDepthIterator = moveDepthAsync (iToDepth);
		StartCoroutine (moveDepthIterator);
	}


	private void OnTriggerEnter (Collider collider)
	{
		var item = collider.GetComponent<ItemCtrl> ();
		if (item != null) {
			if (item is CornCtrl) {
				if (m_Depth < m_DepthMax) {
					moveDepth (m_Depth + 1);
					if (m_OnHitCorn != null) {
						m_OnHitCorn.Invoke ();
					}
				}
			} else {
				if (m_Depth > m_DepthMin) {
					moveDepth (m_Depth - 1);
					if (m_OnHitRock != null) {
						m_OnHitRock.Invoke ();
					}
				}
			}
			item.gameObject.SetActive (false);
		}

		var enemy = collider.GetComponent<EnemyCtrl> ();
		if (enemy != null) {
			if (m_OnHitEnemy != null) {
				m_OnHitEnemy.Invoke ();
			}
		}
	}
}
