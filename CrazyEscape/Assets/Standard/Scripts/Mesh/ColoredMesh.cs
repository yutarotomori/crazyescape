using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class ColoredMesh : CustomMesh
{
	public	Color	_color;
	private	Color		m_Color;
	private	Color[]		m_Colors;

	protected override void Start ()
	{
		base.Start ();
		m_Colors	= new Color[4];
		updateColor ();
	}

	protected override void Update ()
	{
		base.Update ();

		if (_color != m_Color) {
			updateColor ();
		}
	}

	private	void	updateColor ()
	{
		m_Color	= _color;
		for (int aIndex = 0; aIndex < m_Colors.Length; aIndex++) {
			m_Colors[aIndex]	= _color;
		}
		p_Mesh.colors	= m_Colors;
	}
}
