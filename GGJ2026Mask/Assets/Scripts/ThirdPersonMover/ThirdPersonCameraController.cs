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
	[SerializeField] private float _gamePadPitchSpeed = 100f;
	[SerializeField] private float _stickYawSpeed = 10f;
	[SerializeField] private float _stickPitchSpeed = 10f;


	[SerializeField] private float _zoomSpeed = 15f;
	[SerializeField] private float _fixedPitch = 45f;
	[SerializeField] private float _minPitch = -40f;
	[SerializeField] private float _maxPitch = 70f;

	[SerializeField] private Vector3 _targetOffset = new Vector3(0f, 1.5f, 0f);

	private InputSystem_Actions _input;
	private Vector2 _lookInput;
	private float _yaw;
	private float _pitch;

	private bool _isPinching;

	private void Awake()
	{
		_input = new InputSystem_Actions();

		_input.Player.Look.performed += ctx => _lookInput = ctx.ReadValue<Vector2>();
		_input.Player.Look.canceled += ctx => _lookInput = Vector2.zero;
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
			float yawDelta = 0f;
			float pitchDelta = 0f;

			// 🎮 Gamepad
			if (Gamepad.current != null)
			{
				var rightStick = Gamepad.current.rightStick.ReadValue();
				if (rightStick.sqrMagnitude > 0.01f)
				{
					yawDelta = rightStick.x * _gamePadYawSpeed * Time.deltaTime;
					pitchDelta = -rightStick.y * _gamePadPitchSpeed * Time.deltaTime;
				}
			}

			// 🖱 / スティック（StarterAssets / Touch / Mouse）
			if (hasStarterLook && look.sqrMagnitude > 0.0001f)
			{
				yawDelta = look.x * _stickYawSpeed * Time.deltaTime;
				pitchDelta = look.y * _stickPitchSpeed * Time.deltaTime;
			}

			_yaw += yawDelta;
			_pitch += pitchDelta;
			_pitch = Mathf.Clamp(_pitch, _minPitch, _maxPitch);
		}

		var rot = Quaternion.Euler(_pitch, _yaw, 0f);

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