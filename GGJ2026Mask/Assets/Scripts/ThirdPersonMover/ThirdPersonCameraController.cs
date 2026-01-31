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

		UpdatePinchZoom();

		var zoomAxis = _wheelZoomAxis + _pinchZoomAxis;
		ApplyZoom(zoomAxis);

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

	private void ApplyZoom(float axis)
	{
		if (Mathf.Approximately(axis, 0f))
		{
			return;
		}

		_distance -= axis * _zoomSpeed * Time.deltaTime;
		_distance = Mathf.Clamp(_distance, _minDistance, _maxDistance);
	}

	private void UpdatePinchZoom()
	{
		_pinchZoomAxis = 0f;

		var touches = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches;

		if (IsAnyTouchOverUI())
		{
			_isPinching = false;
			return;
		}

		if (touches.Count < 2)
		{
			_isPinching = false;
			return;
		}

		var t0 = touches[0];
		var t1 = touches[1];

		var p0 = t0.screenPosition;
		var p1 = t1.screenPosition;

		float distance = Vector2.Distance(p0, p1);

		if (!_isPinching)
		{
			_isPinching = true;
			_prevPinchDistance = distance;
			return;
		}

		float delta = distance - _prevPinchDistance;
		_prevPinchDistance = distance;

		_pinchZoomAxis = delta * 0.01f;
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