using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverCtrl : MonoBehaviour
{
	public Button _restart;


	private void Start ()
	{
		_restart.onClick.AddListener (() => {
			SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
		});
	}
}
