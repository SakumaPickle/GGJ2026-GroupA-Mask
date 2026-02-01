using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
	[SerializeField] private GameObject[] prefabs;
	private Dictionary<int, GameObject[]> spawns = new Dictionary<int, GameObject[]>();

	private const int ENEMY_MAX_COUNT = 4;
	public int RemineEnemyCount => EnemyTracker.Count;
	public int EnemyMaxCount => ENEMY_MAX_COUNT;
	public bool IsInitialize => _isInitialize;

	private Dictionary<GameObject, int> EnemyTracker = new Dictionary<GameObject, int>();
	private bool _isInitialize;

	void Start()
	{
		for (int i = 0; i < 4; i++)
		{
			var currSpawns = GameObject.FindGameObjectsWithTag("spawn" + i);
			spawns.Add(i, currSpawns);

			var currSpawn = currSpawns[Random.Range(0, currSpawns.Length)];
			var enemy = Instantiate(prefabs[Random.Range(0, prefabs.Length)], currSpawn.transform.position, Quaternion.identity);
			enemy.AddComponent<Enemy>().setController(this);
			enemy.AddComponent<VisibleCheck>();
			EnemyTracker[enemy] = i;
		}

		_isInitialize = true;
	}

	public bool moveEnemy(GameObject enemy)
	{
		int nextRoom = (EnemyTracker[enemy] + 1) % spawns.Count;
		GameObject[] nextSpawns = spawns[nextRoom];
		foreach (int i in Enumerable.Range(0, nextSpawns.Length))
		{
			if (!nextSpawns[i].GetComponent<VisibleCheck>().m_Visible)
			{
				enemy.transform.position = nextSpawns[i].transform.position;
				EnemyTracker[enemy] = nextRoom;
				return true;
			}
		}

		return false;
	}

	public void removeEnemy(GameObject key)
	{
		EnemyTracker.Remove(key);
	}

	// Update is called once per frame
	void Update()
	{

	}
}
