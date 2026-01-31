using UnityEngine;
using UnityEngine.InputSystem;

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
	}

	private void OnDisable()
	{
		_inputActions.Player.Interact.started -= PointerDown;
		_inputActions.Player.Interact.Disable();
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

		if (_hitObject != null)
		{
			Destroy(_hitObject);
			_hitObject = null;
		}
	}
}
