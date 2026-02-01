using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private GameObject[] prefabs;
    private Dictionary<int,GameObject[]> spawns = new Dictionary<int, GameObject[]>();

    private Dictionary<GameObject, int> EnemyTracker = new Dictionary<GameObject, int>();
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
    }

    public bool moveEnemy(GameObject enemy)
    {
        int nextRoom = (EnemyTracker[enemy] +1) % spawns.Count;
        GameObject[] nextSpawns = spawns[nextRoom];
        foreach (int i in Enumerable.Range(0,nextSpawns.Length))
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

    // Update is called once per frame
    void Update()
    {

    }
}
