using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageCtrl : MonoBehaviour
{
//	public Transform _playerPoint;
//	public Transform _enemyPoint;

	public float _speed = 0.1f;
	public PlayerCtrl _player;
	public ItemSpawnerCtrl _itemSpawner;
	public GrassesCtrl _grasses;


	[Header ("UI")]
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
		});

		_grasses.Initialize (() => {
			return _speed;
		});


		_gameOver.SetActive (false);
	}
}
