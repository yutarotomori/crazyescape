using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashCtrl : MonoBehaviour
{

	// Use this for initialization
	IEnumerator Start () {
		yield return new WaitForSeconds (1.0f);
		SceneManager.LoadScene ("Stage");
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
