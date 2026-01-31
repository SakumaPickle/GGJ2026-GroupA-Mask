using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private GameObject[] prefabs;
    private Dictionary<int,GameObject[]> spawns = new Dictionary<int, GameObject[]>();

    void Start()
    {
        for (int i = 1; i <= 4; i++)
        {
            var currSpawns = GameObject.FindGameObjectsWithTag("spawn" + i);
            spawns.Add(i, currSpawns);

            var currSpawn = currSpawns[Random.Range(0, currSpawns.Length)];
            var enemy = Instantiate(prefabs[Random.Range(0, prefabs.Length)], currSpawn.transform.position, Quaternion.identity);
            enemy.AddComponent<Enemy>().setController(this);
            enemy.AddComponent<VisibleCheck>();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
