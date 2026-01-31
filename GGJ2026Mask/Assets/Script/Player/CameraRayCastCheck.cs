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

		// 一旦ヒットチェックする
		if (Physics.Raycast(ray, out var hit, 100f))
		{
			_hitObject = hit.collider.transform.root.gameObject;
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
