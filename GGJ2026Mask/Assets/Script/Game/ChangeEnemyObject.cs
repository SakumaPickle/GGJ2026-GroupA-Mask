using Cysharp.Threading.Tasks;
using UnityEngine;

public class ChangeEnemyObject : MonoBehaviour
{
	[SerializeField] GameObject[] enemyPrefabs;
	GameObject currentEnemy;
	private int index = 0;
	private float timer = 0f;
	private float interval = 1.5f;

	// 仮で位置指定
	private Vector3 enemyPosition = Vector3.zero;

	private void Start()
	{
		SpawnEnemy(0, enemyPosition);
	}

	// Update is called once per frame
	private void Update()
	{
		// 仮で時間でエネミーのオブジェクト切り替え
		timer += Time.deltaTime;
		if (timer >= interval)
		{
			timer = 0f;
			index = (index + 1) % enemyPrefabs.Length;
			SpawnEnemy(index, enemyPosition);
		}
	}

	private void SpawnEnemy(int i, Vector3 position)
	{
		if (currentEnemy != null)
		{
			Destroy(currentEnemy);
		}

		currentEnemy = Instantiate(enemyPrefabs[i], position, Quaternion.identity);
		Debug.Log("Enemy切り替え");
	}
}
