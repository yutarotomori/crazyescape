using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawnerCtrl : StageObjectCtrl
{
	public Transform[] _spawnPoints;


	public float _interval = 1.0f;
	public float _cornRate = 0.3f;
	public float _rockRate = 0.3f;
	public float _logsRate = 0.2f;
	public float _featherRate = 0.1f;
	public float _nothingRate = 0.3f;


	private GameObject m_CornResource;
	private GameObject m_RockResource;
	private GameObject m_LogsResource;
	private GameObject m_FeatherResource;
	private List<GameObject> m_CornPool;
	private List<GameObject> m_RockPool;
	private List<GameObject> m_LogsPool;
	private List<GameObject> m_FeatherPool;
	private bool m_WasInitialized;

	public override void Initialize (GetSpeed getSpeed, GetActiveCondition getActiveCondition)
	{
		base.Initialize (getSpeed, getActiveCondition);

		m_WasInitialized = true;
	}


	private IEnumerator Start ()
	{
		var cornReq = Resources.LoadAsync<GameObject> (ResourcePath.corn);
		var rockReq = Resources.LoadAsync<GameObject> (ResourcePath.rock);
		var logsReq = Resources.LoadAsync<GameObject> (ResourcePath.logs);
		var fertherReq = Resources.LoadAsync<GameObject> (ResourcePath.wing);

		yield return cornReq;
		yield return rockReq;
		yield return logsReq;
		yield return fertherReq;

		m_CornResource = (GameObject)cornReq.asset;
		m_RockResource = (GameObject)rockReq.asset;
		m_LogsResource = (GameObject)logsReq.asset;
		m_FeatherResource = (GameObject)fertherReq.asset;

		m_CornPool = new List<GameObject> ();
		m_RockPool = new List<GameObject> ();
		m_LogsPool = new List<GameObject> ();
		m_FeatherPool = new List<GameObject> ();

		yield return SpawnAsync ();
	}


	private IEnumerator SpawnAsync ()
	{
		while (!m_WasInitialized || !getActiveCondition.Invoke()) {
			yield return 0;
		}

		while (true) {
			yield return new WaitForSeconds (_interval);

			var random = Random.Range (0.0f, _cornRate + _rockRate + _logsRate + _nothingRate + _featherRate);
			if (random <= _cornRate) {
				SpawnItem (m_CornResource, m_CornPool);
			} else if (random <= _cornRate + _rockRate) {
				SpawnItem (m_RockResource, m_RockPool);
			} else if (random <= _cornRate + _rockRate + _logsRate) {
				SpawnItem (m_LogsResource, m_LogsPool);
			} else if (random <= _cornRate + _rockRate + _logsRate + _featherRate) {
				SpawnItem (m_FeatherResource, m_FeatherPool);
			} else {
			}
		}
	}

	private void SpawnItem (GameObject resource, List<GameObject> pool)
	{
		var index = Random.Range (0, _spawnPoints.Length);
		GameObject instance = null;

		for (int i = 0; i < pool.Count; i++) {
			if (!pool [i].activeSelf) {
				instance = pool [i];
				instance.SetActive (true);
				break;
			}
		}

		if (instance == null) {
			instance = Instantiate<GameObject> (resource);
			instance.GetComponent<ItemCtrl> ().Initialize (getSpeed, getActiveCondition);
			pool.Add (instance);
		}

		instance.transform.position = _spawnPoints [index].position;
	}
}
