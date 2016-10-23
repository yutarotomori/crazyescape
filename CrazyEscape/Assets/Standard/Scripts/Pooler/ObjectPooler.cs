using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPooler : Singleton<ObjectPooler>
{
	private	Dictionary<string,ObjectPool>	m_Pools	= new Dictionary<string, ObjectPool>();


	public	Transform	register (string iTag, GameObject iResource)
	{
		return	register (iTag, iResource, 0, true);
	}

	public	Transform	register (string iTag, GameObject iResource, int iMaxCount)
	{
		return	register (iTag, iResource, iMaxCount, true);
	}

	public	Transform	register (string iTag, GameObject iResource, int iMaxCount, bool iIsGrow)
	{
		if (isRegistered (iTag)) {
			Debug.LogWarning ("Error!: ["+iTag+"] was already registered!");
		} else {
			try {
				m_Pools.Add (iTag, ObjectPool.create (iTag, iResource, iMaxCount, iIsGrow));
				m_Pools[iTag]._transform.parent	= transform;
			} catch (System.NullReferenceException e) {
				Debug.LogWarning ("Error!: ["+iTag+"] was not found!");
				Debug.LogError (e);
				return	null;
			}
		}
		return	m_Pools[iTag]._transform;
	}


	public	bool	isRegistered (string iTag)
	{
		return	m_Pools.ContainsKey (iTag);
	}


	public	bool	restore (string iTag, PooledObject iObject)
	{
		if (! m_Pools.ContainsKey (iTag)) {
			Debug.LogWarning ("Error! ["+iTag+"] was not registered yet!");
			return	false;
		}
		return	m_Pools[iTag].restore (iObject);
	}

	public	bool	restoreAll (string iTag)
	{
		if (! m_Pools.ContainsKey (iTag)) {
			Debug.LogWarning ("Error! ["+iTag+"] was not registered yet!");
			return	false;
		}
		m_Pools[iTag].restoreAll ();
		return	true;
	}

	public	PooledObject	release (string iTag)
	{
		if (!m_Pools.ContainsKey (iTag)) {
			Debug.LogWarning ("Error! ["+iTag+"] was not registered yet!");
			return	null;
		}
		return	m_Pools[iTag].release ();
	}

	public	PooledObject	release (string iTag, Transform iParent)
	{
		if (!m_Pools.ContainsKey (iTag)) {
			Debug.LogWarning ("Error! ["+iTag+"] was not registered yet!");
			return	null;
		}
		return	m_Pools[iTag].release (iParent);
	}
}
