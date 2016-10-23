using UnityEngine;
using System.Collections;

public class PooledObject : MonoBehaviour
{
	public	bool	_isFree	= true;
	public	ObjectPool	_pool;
	public	GameObject	_object;

	public	void	setValue (ObjectPool iPool)
	{
		_isFree	= true;
		_pool	= iPool;
		_object	= gameObject;
	}

	public	bool	restore ()
	{
		return	_pool.restore (this);
	}

	private	void	OnDisable ()
	{
		if (_isFree == false) {
			if (transform.parent == _pool._transform) {
				_isFree	= true;
			}
		}
	}
}
