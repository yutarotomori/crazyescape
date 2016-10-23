using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public	class ObjectPool : MonoBehaviour
{
	static	public	ObjectPool	create (string iTag, GameObject iResource, int iCount, bool iIsGrow)
	{
		if (iResource == null) {
			Debug.LogError ("Error! Resource ["+iTag+"] is null.");
			return	null;
		}
		
		GameObject	aObject	= new GameObject ("[POOL] "+iTag);
		ObjectPool	aPool	= aObject.AddComponent<ObjectPool> ();
		aPool.initialize (iTag, iResource, iCount, iIsGrow);

		return	aPool;
	}

	public	string	_tag;
	public	Transform	_transform;
	public	GameObject	_resource;
	public	bool	_isGrow;
	public	List<PooledObject>	_objectList;
	

	public	PooledObject	release ()
	{
		return	release (_transform);
	}

	public	PooledObject	release (Transform iParent)
	{
		PooledObject	aObject	= null;

		foreach (PooledObject obj in _objectList) {
			if (obj._isFree == true) {
				aObject	= obj;
				break;
			}
		}
		if (aObject == null && _isGrow) {
			aObject	= addCache ();
		}

		if (aObject != null) {
			aObject._isFree	= false;
			aObject.transform.SetParent (iParent, false);
			aObject._object.SetActive (true);

			return	aObject;
		}

		Debug.LogWarning ("Error!: pool ["+_tag+"] is already full!");
		return	null;
	}
	
	public	bool	restore (PooledObject iObject)
	{
		if (!_objectList.Contains (iObject)) {
			Debug.LogWarning ("Error!: ["+iObject+"] is not not member of ["+_tag+"]!");
			return	false;
		}
		iObject.transform.parent	= _transform;
		iObject._isFree	= true;
		iObject._object.SetActive (false);
		return	true;
	}
	
	public	void	restoreAll ()
	{
		foreach (PooledObject obj in _objectList) {
			obj.transform.parent	= _transform;
			obj._object.SetActive (false);
		}
	}
	
	private	PooledObject	addCache ()
	{
		PooledObject	aInstant	= Instantiate<GameObject> (_resource).AddComponent <PooledObject> ();
		aInstant.transform.parent	= _transform;
		aInstant.setValue (this);
		_objectList.Add (aInstant);

		return	aInstant;
	}
	
	public	void	destroy ()
	{
		if (_resource	!= null) {
			_resource	= null;
		}
		foreach (PooledObject aObject in _objectList) {
			if (aObject == null) {
				continue;
			}
			Destroy (aObject._object);
		}
		Destroy (gameObject);
	}

	private	void	initialize (string iTag, GameObject iResource, int iCount, bool iIsGrow)
	{
		_tag		= iTag;
		_resource	= iResource;
		_isGrow		= iIsGrow;
		_objectList	= new List<PooledObject> ();
		_transform	= transform;

		for (int i=0;i<iCount;i++) {
			addCache ().restore ();
		}
	}

	private	void	OnDestroy ()
	{
		destroy ();
	}
}
