using UnityEngine;
using System.Collections;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	public static T	GetInstance()	{ return	sInstance;	}
	protected virtual void	Initialize(){}
	private void Awake()
	{
		DontDestroyOnLoad(gameObject);
		if (sInstance	== null) {
			sInstance	= (T)FindObjectOfType(typeof(T))as T;
			if (sInstance	== null) {
				Debug.LogError(gameObject.name);
			}
		} else {
			if (sInstance.GetInstanceID() > this.GetInstanceID()) {
				Destroy(sInstance.gameObject);
				sInstance	= (T)FindObjectOfType(typeof(T))as T;
			} else {
				Destroy(gameObject);
				return;
			}
		}
		Initialize();
	}
	private static T	sInstance;
}
