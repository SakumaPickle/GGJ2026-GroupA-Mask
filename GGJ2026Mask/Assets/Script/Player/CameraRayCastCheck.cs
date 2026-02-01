using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

public class CameraRayCastCheck : MonoBehaviour
{
	[SerializeField] private Camera _playerCamera;
	[SerializeField] private InputSystem_Actions _inputActions;
	[SerializeField] private EnemyManager _enemyManager;

	private GameObject _hitObject;
	private bool _isTouchUI;

	private void Awake()
	{
		if (_inputActions == null)
			_inputActions = new InputSystem_Actions();
	}

	private void Update()
	{
		CheckCollider();
		_isTouchUI = IsAnyTouchOverUI();
	}

	private void OnEnable()
	{
		_inputActions.Player.Interact.started += PointerDown;
		_inputActions.Player.Interact.Enable();
		EnhancedTouchSupport.Enable();
	}

	private void OnDisable()
	{
		_inputActions.Player.Interact.started -= PointerDown;
		_inputActions.Player.Interact.Disable();
		EnhancedTouchSupport.Disable();
	}

	private bool CheckCollider()
	{
		// Ray from player camera forward direction
		var ray = new Ray(
			_playerCamera.transform.position,
			_playerCamera.transform.forward
		);

		// Raycast
		if (Physics.Raycast(ray, out var hit, 100f))
		{
			var marker = hit.collider.GetComponentInParent<DespawnEnemyMarker>();
			if (marker == null)
			{
				_hitObject = null;
				return false;
			}

			_hitObject = marker.gameObject;

			if (_hitObject.CompareTag("DontSelected"))
			{
				_hitObject = null;
				return false;
			}

			return true;
		}

		_hitObject = null;
		return false;
	}

	private void PointerDown(InputAction.CallbackContext ctx)
	{
		// ヒットしている状態で何かしらアクションできるようにする

		if (_hitObject == null)
			return;

		// uitouch DontDestroy
		if (_isTouchUI)
		{
			return;
		}

		if (IsEnemyObject(_hitObject))
		{
			_enemyManager.removeEnemy(_hitObject);
		}
		Destroy(_hitObject);
		_hitObject = null;
	}

	private bool IsEnemyObject(GameObject targetObj)
	{
		if (_enemyManager == null)
		{
			return false;
		}

		var enemy = targetObj.GetComponent<Enemy>();
		return enemy != null;
	}


	private bool IsAnyTouchOverUI()
	{
		if (EventSystem.current == null)
		{
			return false;
		}

		foreach (var t in UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches)
		{
			if (EventSystem.current.IsPointerOverGameObject(t.touchId))
			{
				return true;
			}
		}

		return false;
	}
}
