using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class DrawLine : MonoBehaviour
{
	static	public	DrawLine create ()
	{
		return	create (Vector3.zero, Vector3.zero, 0.1f, 0.1f, Color.white, null, true, false);
	}

	static	public	DrawLine create (Vector3 iStartPoint
	                               , Vector3 iEndPoint
	                               , float iWidth
	                               , float iHeight
	                               , Color iColor
	                               , Material iMaterial
	                               , bool iVectorRight = true
	                               , bool iUseUpdate = false)
	{
		GameObject	aInstant	= new GameObject ("DrawLine");
		aInstant.AddComponent<MeshFilter> ();
		aInstant.AddComponent<MeshRenderer> ();

		DrawLine	aDrawLine	= aInstant.AddComponent <DrawLine> ();
		aDrawLine.setValue (iStartPoint, iEndPoint, iWidth, iHeight, iColor, iMaterial, iVectorRight, iUseUpdate);
		return	aDrawLine;
	}

	public	Vector3	_startPoint;
	public	Vector3	_endPoint;
	public	float	_width = 0.1f;
	public	float	_height	= 0.1f;
	public	Color	_color;
	public	Material	_material;
	public	bool	_vectorRight;
	public	bool	_useUpdate;

	private	int[]		m_Triangles;
	private	Mesh		m_MeshValue;
	private Mesh		m_Mesh {
		get {
			return	m_MeshValue;
		}
		set {
			if (m_MeshValue != null) {
				Destroy (m_MeshValue);
			}
			m_MeshValue	= value;
		}
	}
	private	MeshFilter	m_Filter;
	private	MeshRenderer	m_Renderer;
	private Vector3[]	m_Vertices;
	private Vector2[]	m_UVs;
	private	Color[]		m_Colors;

	public	void	setValue (	Vector3 iStartPoint
	                      ,		Vector3 iEndPoint
	                      ,		float iWdth
	                      ,		float iHeight
	                      ,		Color iColor
	                      ,		Material iMaterial
	                      ,		bool iVectorRight = true
	                      ,		bool iUseUpdate = false)
	{
		_startPoint	= iStartPoint;
		_endPoint	= iEndPoint;
		_width		= iWdth;
		_height		= iHeight;
		_color		= iColor;
		_material	= iMaterial;
		_vectorRight	= iVectorRight;
		_useUpdate	= iUseUpdate;

		Start ();
	}

	public	void	callUpdate ()
	{
		updateMesh ();
	}

	private	void	Start ()
	{
		if (m_Mesh != null) {
			Destroy (m_Mesh);
			m_Mesh	= null;
		}
		m_Mesh		= new Mesh();
		m_Vertices	= new Vector3[4];
		m_Triangles	= new int[6];
		m_UVs		= new Vector2[4];
		m_Colors	= new Color[4];
		m_Renderer	= GetComponent<MeshRenderer> ();

		if (_material == null) {
			m_Renderer.sharedMaterial	= Resources.GetBuiltinResource <Material> ("Sprites-Default.mat");
		} else {
			m_Renderer.sharedMaterial	= _material;
		}

		m_Mesh.Clear ();
		m_Filter	= GetComponent<MeshFilter>();

		m_Triangles[0]	= 0;
		m_Triangles[1]	= 1;
		m_Triangles[2]	= 2;
		m_Triangles[3]	= 2;
		m_Triangles[4]	= 3;
		m_Triangles[5]	= 0;

		updateMesh ();
	}

	private	void	Update ()
	{
		if (_useUpdate == true) {
			updateMesh ();
		}
	}

	private	void	updateMesh ()
	{
		if (_vectorRight == true) {
			m_Vertices[0].Set (_startPoint.x-_width*0.5f, _startPoint.y, _startPoint.z+_height*0.5f);
			m_Vertices[1].Set (_endPoint.x+_width*0.5f, + _endPoint.y, _endPoint.z+_height*0.5f);
			m_Vertices[2].Set (_endPoint.x+_width*0.5f, + _endPoint.y, _endPoint.z-_height*0.5f);
			m_Vertices[3].Set (_startPoint.x-_width*0.5f, + _startPoint.y, _startPoint.z-_height*0.5f);
		} else {
			m_Vertices[0].Set (_startPoint.x-_width*0.5f, _startPoint.y, _startPoint.z-_height*0.5f);
			m_Vertices[1].Set (_startPoint.x+_width*0.5f, + _startPoint.y, _startPoint.z+_height*0.5f);
			m_Vertices[2].Set (_endPoint.x+_width*0.5f, + _endPoint.y, _endPoint.z+_height*0.5f);
			m_Vertices[3].Set (_endPoint.x-_width*0.5f, + _endPoint.y, _endPoint.z+_height*0.5f);
		}
		
		m_UVs[0]	= Vector2.up;
		m_UVs[1]	= Vector2.up + (Vector2.right);
		m_UVs[2]	= Vector2.right;
		m_UVs[3]	= Vector2.zero;
		
		m_Mesh.vertices		= m_Vertices;
		m_Mesh.triangles	= m_Triangles;
		m_Mesh.uv			= m_UVs;
		m_Mesh.normals		= m_Vertices;
		m_Filter.sharedMesh	= m_Mesh;
		
		for (int aIndex = 0; aIndex < m_Colors.Length; aIndex++) {
			m_Colors[aIndex]	= _color;
		}
		m_Mesh.colors	= m_Colors;
	}

	private	void	OnDestroy ()
	{
		if (m_Mesh != null) {
			DestroyImmediate (m_Mesh, true);
		}
	}
}
