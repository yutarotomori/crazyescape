using UnityEngine;
using System.Collections;

public class GradationMesh : CustomMesh
{
	public	Color	_color1	= Color.white;
	public	Color	_color2	= Color.white;
	public	Color	_color3	= Color.white;
	public	Color	_color4	= Color.white;

	private	Color[]	m_Colors;

	protected override void Start ()
	{
		base.Start ();

		m_Colors	= new Color[4];

		updateColor ();
	}
	
	protected override void Update ()
	{
		base.Update ();
		
		updateColor ();
	}
	
	private	void	updateColor ()
	{
		m_Colors[0]	= _color1;
		m_Colors[1]	= _color2;
		m_Colors[2]	= _color3;
		m_Colors[3]	= _color4;

		p_Mesh.colors	= m_Colors;
	}
}
