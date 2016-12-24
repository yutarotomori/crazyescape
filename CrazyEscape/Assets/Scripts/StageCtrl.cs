using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageCtrl : MonoBehaviour
{
	[System.Serializable]
	public struct UI
	{
		public GameObject _titleUI;
		public GameObject _stageUI;
		public Text _mileage;
		public Image _HPGauge;
		public GameObject _gameOver;
		public UIWingPointCtrl[] _wingPoints;
	}

	public GameMode _gameMode;
	public float _speed = 0.1f;
	public PlayerCtrl _player;
	public ItemSpawnerCtrl _itemSpawner;
	public GrassesCtrl _grasses;
	public TitleCtrl _title;

	public UI _UI;


	private float m_Mileage;


	private void Start ()
	{
		MainCtrl.GetInstance ().PlayBGM (ResourcePath.bgm_stage);

		_player.Initialize (
			AdvanceUnit,
			AddWingPoint,
			OnHitObstacle,
			OnHitEnemy
		);

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
			_UI._titleUI.SetActive (false);
			_UI._stageUI.SetActive (true);
			_gameMode = GameMode.Stage;
		});

		_UI._gameOver.SetActive (false);

		_UI._titleUI.SetActive (_gameMode == GameMode.Title);
		_UI._stageUI.SetActive (_gameMode == GameMode.Stage);
	}

	private void AdvanceUnit()
	{
		_speed *= 1.15f;
		_player.AdvanceDepth ();
	}

	private void RetreatUnit()
	{
		_player.RetreatDepth ();
	}

	private void AddWingPoint()
	{
		for (int i = 0; i < _UI._wingPoints.Length; i++) {
			if (!_UI._wingPoints [i].isEnable) {
				_UI._wingPoints [i].setEnable (true);
				break;
			}
		}
	}

	private void OnHitCorn()
	{
		AdvanceUnit ();
	}

	private void OnHitObstacle()
	{
		bool hasWing = false;
		for (int i = _UI._wingPoints.Length - 1; i >= 0; i--) {
			if (_UI._wingPoints [i].isEnable) {
				_UI._wingPoints [i].setEnable (false);
				hasWing = true;
				break;
			}
		}

		if (!hasWing) {
			RetreatUnit ();
			_speed /= 1.15f;
		}
	}

	private void OnHitEnemy()
	{
		_UI._gameOver.SetActive (true);
		MainCtrl.GetInstance ().PlayBGM (ResourcePath.bgm_gameOver);
	}


	private void Update ()
	{
		if (_gameMode == GameMode.Stage) {
			m_Mileage += _speed;
			_UI._mileage.text = string.Format ("{0:0.0} m", m_Mileage);

			_UI._HPGauge.fillAmount = _player._hp;
		}
	}
}
