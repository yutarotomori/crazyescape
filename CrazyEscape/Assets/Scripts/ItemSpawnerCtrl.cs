using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawnerCtrl : StageObjectCtrl
{
	public Transform[] _spawnPoints;


	public float _interval = 1.0f;
	public float _cornRate = 0.3f;
	public float _rockRate = 0.3f;
	public float _nothingRate = 0.3f;


	private GameObject m_CornResource;
	private GameObject m_RockResource;
	private List<GameObject> m_CornPool;
	private List<GameObject> m_RockPool;
	private bool m_WasInitialized;

	public override void Initialize (GetSpeed getSpeed)
	{
		base.Initialize (getSpeed);

		m_WasInitialized = true;
	}


	private IEnumerator Start ()
	{
		var cornReq = Resources.LoadAsync<GameObject> (ResourcePath._corn);
		var rockReq = Resources.LoadAsync<GameObject> (ResourcePath._rock);

		yield return cornReq;
		yield return rockReq;

		m_CornResource = (GameObject)cornReq.asset;
		m_RockResource = (GameObject)rockReq.asset;

		m_CornPool = new List<GameObject> ();
		m_RockPool = new List<GameObject> ();

		yield return SpawnAsync ();
	}


	private IEnumerator SpawnAsync ()
	{
		while (!m_WasInitialized) {
			yield return 0;
		}

		while (true) {
			yield return new WaitForSeconds (_interval);

			var random = Random.Range (0.0f, _cornRate + _rockRate + _nothingRate);
			if (random <= _cornRate) {
				SpawnItem (m_CornResource, m_CornPool);
			} else if (random <= _cornRate + _rockRate) {
				SpawnItem (m_RockResource, m_RockPool);
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
			instance.GetComponent<ItemCtrl> ().Initialize (getSpeed);
			pool.Add (instance);
		}

		instance.transform.position = _spawnPoints [index].position;
	}
}
