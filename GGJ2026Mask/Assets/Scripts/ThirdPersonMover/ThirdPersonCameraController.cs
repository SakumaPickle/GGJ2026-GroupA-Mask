using StarterAssets;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

public class ThirdPersonCameraController : MonoBehaviour
{
	[SerializeField] private StarterAssetsInputs _starterInputs;
	[SerializeField] private Transform _target;
	[SerializeField] private float _distance = 5f;
	[SerializeField] private float _minDistance = 3f;
	[SerializeField] private float _maxDistance = 10f;
	[SerializeField] private float _gamePadYawSpeed = 360f;
	[SerializeField] private float _stickYawSpeed = 180f;

	[SerializeField] private float _zoomSpeed = 15f;
	[SerializeField] private float _fixedPitch = 45f;
	[SerializeField] private Vector3 _targetOffset = new Vector3(0f, 1.5f, 0f);

	private InputSystem_Actions _input;
	private Vector2 _lookInput;
	private float _yaw;

	private float _wheelZoomAxis;
	private float _pinchZoomAxis;

	private bool _isPinching;
	private float _prevPinchDistance;

	private void Awake()
	{
		_input = new InputSystem_Actions();

		_input.Player.Look.performed += ctx => _lookInput = ctx.ReadValue<Vector2>();
		_input.Player.Look.canceled += ctx => _lookInput = Vector2.zero;

		_input.Player.CameraZoom.performed += ctx => _wheelZoomAxis = ctx.ReadValue<float>();
		_input.Player.CameraZoom.canceled += ctx => _wheelZoomAxis = 0f;
	}

	private void OnEnable()
	{
		_input.Enable();
		EnhancedTouchSupport.Enable();
	}

	private void OnDisable()
	{
		_input.Disable();
		EnhancedTouchSupport.Disable();
	}

	private void LateUpdate()
	{
		if (_target == null)
		{
			return;
		}

		var look = GetEffectiveLookInput();
		var hasStarterLook =
			_starterInputs != null &&
			_starterInputs.look.sqrMagnitude > 0.001f;

		if (!_isPinching)
		{
			var yawDelta = 0f;

			if (Gamepad.current != null)
			{
				var rightStick = Gamepad.current.rightStick.ReadValue();
				if (rightStick.sqrMagnitude > 0.01f)
				{
					yawDelta = rightStick.x * _gamePadYawSpeed * Time.deltaTime;
				}

				_yaw += yawDelta;
			}

			if ((hasStarterLook) && look.sqrMagnitude > 0.0001f)
			{
				if (hasStarterLook)
				{
					yawDelta = look.x * _stickYawSpeed * Time.deltaTime;
				}

				_yaw += yawDelta;
			}
		}

		var rot = Quaternion.Euler(_fixedPitch, _yaw, 0f);
		var focusPos = _target.position + _targetOffset;
		var camPos = focusPos + rot * new Vector3(0f, 0f, -_distance);

		transform.position = camPos;
		transform.rotation = rot;
	}

	private Vector2 GetEffectiveLookInput()
	{
		if (_starterInputs != null && _starterInputs.look.sqrMagnitude > 0.001f)
		{
			return _starterInputs.look;
		}

		return _lookInput;
	}
}