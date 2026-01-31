using UnityEngine;

public class Enemy : MonoBehaviour
{
    private EnemyManager _manager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    public void setController(EnemyManager manager)
    {
        _manager = manager;
    }
    // Update is called once per frame
    void Update()
    {

    }
}
