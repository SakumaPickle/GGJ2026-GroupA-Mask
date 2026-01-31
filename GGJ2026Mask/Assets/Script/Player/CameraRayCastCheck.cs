using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

public class CameraRayCastCheck : MonoBehaviour
{
	[SerializeField] private Camera _playerCamera;
	[SerializeField] private InputSystem_Actions _inputActions;

	private GameObject _hitObject;

	private void Awake()
	{
		if (_inputActions == null)
			_inputActions = new InputSystem_Actions();
	}

	private void Update()
	{
		CheckCollider();
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
		if (IsAnyTouchOverUI(ctx))
		{
			return;
		}

		Destroy(_hitObject);
		_hitObject = null;
	}

	private bool IsAnyTouchOverUI(InputAction.CallbackContext ctx)
	{
		if (EventSystem.current == null)
		{
			return false;
		}

		// Mouse / Pen: pointerId = -1 is the common EventSystem pointer id
		if (ctx.control != null && ctx.control.device is Pointer)
		{
			return EventSystem.current.IsPointerOverGameObject();
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
