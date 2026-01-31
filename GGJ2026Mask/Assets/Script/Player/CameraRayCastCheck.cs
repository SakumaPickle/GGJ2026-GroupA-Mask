using UnityEngine;
using UnityEngine.InputSystem;

public class CameraRayCastCheck : MonoBehaviour
{
	[SerializeField] private InputSystem_Actions _inputActions;

	private void Awake()
	{
		if (_inputActions == null)
			_inputActions = new InputSystem_Actions();
	}

	private void OnEnable()
	{
		_inputActions.UI.Click.started += PointerDown;
		_inputActions.UI.Enable();
	}

	private void OnDisable()
	{
		_inputActions.UI.Click.started -= PointerDown;
		_inputActions.UI.Disable();
	}

	private void PointerDown(InputAction.CallbackContext ctx)
	{
		var mousePosition = Mouse.current.position.ReadValue();
		Debug.Log($"{mousePosition}");
		var ray = Camera.main.ScreenPointToRay(mousePosition);
		if (Physics.Raycast(ray, out var hit))
		{
			if (hit.collider.CompareTag("Enemy"))
			{
				Destroy(hit.collider.gameObject);
			}
		}
	}
}
