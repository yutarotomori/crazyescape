using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CustomMesh : MonoBehaviour
{
	public	Mesh	_sharedMesh;
	
	public	Vector3	_vretices1	= new Vector3 (-0.5f,0.5f,0.0f);
	public	Vector3	_vretices2	= new Vector3 (0.5f,0.5f,0.0f);
	public	Vector3	_vretices3	= new Vector3 (0.5f,-0.5f,0.0f);
	public	Vector3	_vretices4	= new Vector3 (-0.5f,-0.5f,0.0f);
	public	bool	_useUpdate;

	protected	int[]		p_Triangles;
	private		Mesh	m_MeshValue;
	protected	Mesh		p_Mesh {
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
	protected	MeshFilter	p_Filter;
	protected	MeshRenderer	p_Renderer;
	protected	Vector3[]	p_Vertices;
	protected	Vector2[]	p_UVs;
	
	protected	virtual void	Start ()
	{
		initialize ();
		updateMesh ();
	}
	
	protected	virtual	void	Update ()
	{
		if (_useUpdate == true) {
			updateMesh ();
		}
	}
	
	private	void	initialize ()
	{
		if (_sharedMesh == null) {
			_sharedMesh	= new Mesh ();
		}
		p_Mesh		= _sharedMesh;
		p_Vertices	= new Vector3[4];
		p_Triangles	= new int[6];
		p_UVs		= new Vector2[4];
		p_Renderer	= GetComponent<MeshRenderer> ();
		
		if (p_Renderer.sharedMaterial == null) {
			p_Renderer.sharedMaterial	= Resources.GetBuiltinResource <Material> ("Sprites-Default.mat");
		}
		
		p_Mesh.Clear ();
		p_Filter	= GetComponent<MeshFilter>();
		
		p_Triangles[0]	= 0;
		p_Triangles[1]	= 1;
		p_Triangles[2]	= 2;
		p_Triangles[3]	= 2;
		p_Triangles[4]	= 3;
		p_Triangles[5]	= 0;
	}
	
	private	void	updateMesh ()
	{
		p_Vertices[0]	= _vretices1;
		p_Vertices[1]	= _vretices2;
		p_Vertices[2]	= _vretices3;
		p_Vertices[3]	= _vretices4;
		
		p_UVs[0]	= Vector2.up;
		p_UVs[1]	= Vector2.up + (Vector2.right);
		p_UVs[2]	= Vector2.right;
		p_UVs[3]	= Vector2.zero;
		
		p_Mesh.vertices		= p_Vertices;
		p_Mesh.triangles	= p_Triangles;
		p_Mesh.uv			= p_UVs;
		p_Mesh.normals		= p_Vertices;
		p_Filter.sharedMesh	= p_Mesh;
	}

	private	void	OnDestroy ()
	{
		if (p_Mesh != null) {
			DestroyImmediate (p_Mesh, true);
		}
	}
}
