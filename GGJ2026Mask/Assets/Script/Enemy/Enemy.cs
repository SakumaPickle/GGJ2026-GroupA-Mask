using UnityEngine;

public class Enemy : MonoBehaviour
{
	private EnemyManager _manager;
	private VisibleCheck _visibleCheck;

	[SerializeField] private float timer = 10f;

	private float _countDown;
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		_countDown = timer;
		_visibleCheck = GetComponent<VisibleCheck>();
	}

	public void setController(EnemyManager manager)
	{
		_manager = manager;
	}
	// Update is called once per frame
	void Update()
	{
		_countDown -= Time.deltaTime;
		if (_countDown <= 0f && !_visibleCheck.m_Visible)
		{
			if (_manager.moveEnemy(gameObject))
			{
				_countDown = timer;
			}
			else
			{
				_countDown = timer / 2;
			}
		}
	}
}
