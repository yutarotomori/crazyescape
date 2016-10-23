using UnityEngine;
using System.Collections;

public class AspectUtility : MonoBehaviour {
	// 縦横比です。インスペクタから修正します。
	public float m_x_aspect = 16.0f;
	public float m_y_aspect = 9.0f;

	private	void	Update ()
	{
		Camera	camera	= GetComponent<Camera>();
		Rect	rect	= CalcAspect(m_x_aspect, m_y_aspect);

		camera.rect	= rect;
	}
	// アスペクト比計算
	private	Rect	CalcAspect(float width, float height){
		float target_aspect = width / height;
		float window_aspect = (float)Screen.width / (float)Screen.height;
		float scale_height = window_aspect / target_aspect;
		Rect rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);

		if (1.0f > scale_height) {
			rect.x = 0;
			rect.y = (1.0f - scale_height) / 2.0f;
			rect.width = 1.0f;
			rect.height = scale_height;
		} else {
			float scale_width = 1.0f / scale_height;
			rect.x = (1.0f - scale_width) / 2.0f;
			rect.y = 0.0f;
			rect.width = scale_width;
			rect.height = 1.0f;
		}
		return rect;
	}
}
