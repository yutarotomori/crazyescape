using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TitleCtrl : MonoBehaviour
{
	public Camera _camera;
	public Transform _stageCameraPoint;


	private UnityAction onStarted;


	public void Initialize(UnityAction onStarted)
	{
		this.onStarted = onStarted;
	}

	private void Update ()
	{
		if (Input.anyKeyDown) {
			StartCoroutine (StartAsync ());
		}
	}

	private IEnumerator StartAsync()
	{
		var fromPoint = _camera.transform.position;
		var fromAngle = _camera.transform.eulerAngles;

		for (float i = 0.0f; i < 1.0f; i += 0.05f) {
			var newAngle = fromAngle;
			newAngle.x = Mathf.LerpAngle (fromAngle.x, _stageCameraPoint.eulerAngles.x, i);
			newAngle.y = Mathf.LerpAngle (fromAngle.y, _stageCameraPoint.eulerAngles.y, i);
			_camera.transform.position = Vector3.Slerp (fromPoint, _stageCameraPoint.position, i);
			_camera.transform.eulerAngles = newAngle;
			yield return 0;
		}
		_camera.transform.position = _stageCameraPoint.position;
		_camera.transform.eulerAngles = _stageCameraPoint.eulerAngles;

		if (onStarted != null) {
			onStarted.Invoke ();
		}
	}
}
