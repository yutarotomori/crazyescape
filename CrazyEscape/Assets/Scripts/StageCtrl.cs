using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageCtrl : MonoBehaviour
{
	public GameMode _gameMode;
	public float _speed = 0.1f;
	public PlayerCtrl _player;
	public ItemSpawnerCtrl _itemSpawner;
	public GrassesCtrl _grasses;
	public TitleCtrl _title;


	[Header ("UI")]
	public GameObject _titleUI;
	public GameObject _stageUI;

	public GameObject _gameOver;


	private void Start ()
	{
		_player.Initialize (() => {
			_speed *= 1.3f;
		}, () => {
			_speed /= 1.3f;
		}, () => {
			_gameOver.SetActive (true);
		});

		_itemSpawner.Initialize (() => {
			return _speed;
		}, () => {
			return _gameMode == GameMode.Stage;
		});

		_grasses.Initialize (() => {
			return _speed;
		}, () => {
			return _gameMode == GameMode.Stage;
		});

		_title.Initialize (() => {
			_title.gameObject.SetActive (false);
			_titleUI.SetActive (false);
			_stageUI.SetActive (true);
			_gameMode = GameMode.Stage;
		});

		_gameOver.SetActive (false);

		_titleUI.SetActive (_gameMode == GameMode.Title);
		_stageUI.SetActive (_gameMode == GameMode.Stage);
	}
}
