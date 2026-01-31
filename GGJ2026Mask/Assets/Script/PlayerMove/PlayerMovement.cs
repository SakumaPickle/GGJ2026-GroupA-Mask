using StarterAssets;   // StarterAssetsInputs 用
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
	[SerializeField] private StarterAssetsInputs _starterInputs;
	[SerializeField] private PlayerInput _playerInput;

	[Header("Move")]
	[SerializeField] private float _walkSpeed = 2f;
	[SerializeField] private float _runSpeed = 5f;

	[Header("Jump")]
	[SerializeField] private float _normalJumpHeight = 1.6f;
	[SerializeField] private int _maxJumpCount = 2;

	[Header("Gravity / Ground Check")]
	[SerializeField] private float _gravity = 20f;
	[SerializeField] private float _groundCheckDistance = 0.3f;
	[SerializeField, Range(0f, 1f)] private float _minGroundNormalY = 0.5f;

	[Header("Rotation")]
	[SerializeField] private float _maxYawPerSecond = 360f;

	private Vector3 _velocity;
	private bool _isRunning;
	private bool _isGrounded;
	private Vector3 _groundNormal = Vector3.up;

	private CharacterController _controller;
	private InputSystem_Actions _inputActions;
	private PlayerAnimatorManager _animationManager;

	private Vector2 _moveInput;

	private int _jumpCount;

	private void Awake()
	{
		_controller = GetComponent<CharacterController>();
		_inputActions = new InputSystem_Actions();
		_animationManager = GetComponent<PlayerAnimatorManager>();

		_inputActions.Player.Move.performed += ctx => _moveInput = ctx.ReadValue<Vector2>();
		_inputActions.Player.Move.canceled += ctx => _moveInput = Vector2.zero;

		_inputActions.Player.Jump.started += OnJumpStarted;

		_inputActions.Player.Sprint.performed += ctx => ToggleRun();
	}

	private void OnEnable() => _inputActions.Enable();
	private void OnDisable() => _inputActions.Disable();

	private void Update()
	{
		var moveInput = GetEffectiveMoveInput();

		HandleMovement(moveInput);
		HandleRotation(moveInput);
		ApplyFinalMove();
		UpdateGroundCheck();
	}

	private Vector2 GetEffectiveMoveInput()
	{
		if (_starterInputs != null && _starterInputs.move.sqrMagnitude > 0.001f)
		{
			return _starterInputs.move;
		}

		return _moveInput;
	}

	private void UpdateGroundCheck()
	{
		var center = transform.position + _controller.center;
		var halfHeight = _controller.height * 0.5f - _controller.radius;
		var castOffset = 0.05f;
		var p1 = center + Vector3.up * (halfHeight + castOffset);
		var p2 = center - Vector3.up * (halfHeight - castOffset);

		var grounded = false;
		var groundNormal = Vector3.up;

		if (Physics.CapsuleCast(p1, p2, _controller.radius * 0.95f, Vector3.down,
				out var hit, _groundCheckDistance,
				Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
		{
			if (hit.normal.y >= _minGroundNormalY)
			{
				grounded = true;
				groundNormal = hit.normal;
			}
		}

		var wasGrounded = _isGrounded;
		_isGrounded = grounded;
		_groundNormal = groundNormal;

		_animationManager.SetIsGrounded(_isGrounded);

		if (_isGrounded && !wasGrounded)
		{
			_jumpCount = 0;

			_velocity.y = -2f;
			_animationManager.SetIsJump(false);
		}
	}

	private void HandleMovement(Vector2 moveInput)
	{
		var moveDirection = GetMoveDirectionFromInput(moveInput);

		if (moveDirection.sqrMagnitude > 0.01f)
		{
			var speed = _isRunning ? _runSpeed : _walkSpeed;
			moveDirection = Vector3.ProjectOnPlane(moveDirection, _groundNormal).normalized;

			var moveXZ = moveDirection * speed;
			_velocity.x = moveXZ.x;
			_velocity.z = moveXZ.z;
		}
		else
		{
			_velocity.x = 0f;
			_velocity.z = 0f;
		}
	}

	private void HandleRotation(Vector2 moveInput)
	{
		var moveDirection = GetMoveDirectionFromInput(moveInput);
		if (moveDirection.sqrMagnitude < 0.001f)
			return;

		var targetForward = moveDirection;
		targetForward.y = 0f;
		targetForward.Normalize();

		float rotateSpeed = _maxYawPerSecond;

		var targetRotation = Quaternion.LookRotation(targetForward, Vector3.up);
		transform.rotation = Quaternion.RotateTowards(
			transform.rotation,
			targetRotation,
			rotateSpeed * Time.deltaTime
		);
	}

	private Vector3 GetMoveDirectionFromInput(Vector2 moveInput)
	{
		if (moveInput.sqrMagnitude < 0.01f)
			return Vector3.zero;

		var cam = Camera.main;
		if (cam == null)
		{
			return (transform.right * moveInput.x + transform.forward * moveInput.y).normalized;
		}

		var camForward = cam.transform.forward;
		camForward.y = 0f;
		camForward.Normalize();

		var camRight = cam.transform.right;
		camRight.y = 0f;
		camRight.Normalize();

		return (camRight * moveInput.x + camForward * moveInput.y).normalized;
	}

	private void ApplyFinalMove()
	{
		if (_isGrounded)
		{
			if (_velocity.y < -2f)
				_velocity.y = -2f;
		}
		else
		{
			_velocity.y -= _gravity * Time.deltaTime;
		}

		_animationManager.PlayWalkAnimation(_velocity);
		_controller.Move(_velocity * Time.deltaTime);
	}

	public void OnJumpStarted(InputAction.CallbackContext ctx)
	{
		if (_jumpCount >= _maxJumpCount)
		{
			return;
		}

		if (_isGrounded || _jumpCount >= 1)
		{
			DoJump(_normalJumpHeight);
			_jumpCount++;
		}
	}

	private void DoJump(float height)
	{
		var baseSpeed = Mathf.Sqrt(height * 2f * _gravity);

		_velocity.y = baseSpeed;
		_animationManager.Animator.SetTrigger("Jump");
		_animationManager.Animator.Play("JumpStart", 0, 0f);
		_animationManager.SetIsJump(true);
	}

	public void OnUIJumpStart()
	{
		OnJumpStarted(new InputAction.CallbackContext());
	}


	public void ToggleRun()
	{
		_isRunning = !_isRunning;
	}

	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
	}
}
