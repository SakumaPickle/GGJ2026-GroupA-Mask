using StarterAssets;   // StarterAssetsInputs �p
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement_RB : MonoBehaviour
{
	[SerializeField] private Game _gameManager;

	[SerializeField] private StarterAssetsInputs _starterInputs;
	[SerializeField] private PlayerInput _playerInput;

	[Header("Move")]
	[SerializeField] private float _walkSpeed = 2f;
	[SerializeField] private float _runSpeed = 5f;

	[Header("Acceleration (Rigidbody-style)")]
	[SerializeField] private float _accel = 20f;         // How fast to reach target speed on ground
	[SerializeField] private float _airAccel = 8f;       // How fast to change direction in air
	[SerializeField] private float _maxSlopeAngle = 55f; // Just for reference; ground normal y is used below

	[Header("Jump")]
	[SerializeField] private float _normalJumpHeight = 1.6f;
	[SerializeField] private int _maxJumpCount = 2;

	[Header("Gravity / Ground Check")]
	[SerializeField] private float _gravity = 20f;                 // Custom gravity strength
	[SerializeField] private float _groundCheckDistance = 0.3f;
	[SerializeField, Range(0f, 1f)] private float _minGroundNormalY = 0.5f;

	[Header("Ground Cast (SphereCast)")]
	[SerializeField] private float _groundCastRadius = 0.25f;
	[SerializeField] private float _groundCastUpOffset = 0.1f;
	[SerializeField] private LayerMask _groundLayers = ~0;

	[Header("Rotation")]
	[SerializeField] private float _maxYawPerSecond = 360f;

	[Header("Rigidbody")]
	[SerializeField] private bool _useBuiltinGravity = false; // If true, uses rb.useGravity instead of custom gravity
	[SerializeField] private float _groundSnapDown = 2f;       // Small downward speed to keep contact when grounded
	[SerializeField] private float _linearDragGround = 0f;     // Optional: set via Inspector if you want
	[SerializeField] private float _linearDragAir = 0f;        // Optional: set via Inspector if you want

	[Header("Input Deadzone")]
	[SerializeField, Range(0f, 0.5f)] private float _moveDeadzone = 0.08f;

	private Rigidbody _rb;
	private Collider _col;

	private InputSystem_Actions _inputActions;
	private PlayerAnimatorManager _animationManager;

	private Vector2 _moveInput;
	private bool _isRunning;

	private bool _isGrounded;
	private Vector3 _groundNormal = Vector3.up;

	private int _jumpCount;

	private void Awake()
	{
		_rb = GetComponent<Rigidbody>();
		_col = GetComponent<Collider>();

		_inputActions = new InputSystem_Actions();
		_animationManager = GetComponent<PlayerAnimatorManager>();

		_inputActions.Player.Move.performed += ctx => _moveInput = ctx.ReadValue<Vector2>();
		_inputActions.Player.Move.canceled += ctx => _moveInput = Vector2.zero;

		_inputActions.Player.Jump.started += OnJumpStarted;

		// Keep original behavior: toggle run on Sprint performed
		_inputActions.Player.Sprint.performed += ctx => ToggleRun();

		// Rigidbody recommended settings
		_rb.isKinematic = false;
		_rb.interpolation = RigidbodyInterpolation.Interpolate;
		_rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

		_rb.useGravity = _useBuiltinGravity;
		_rb.linearDamping = _linearDragGround;
	}

	private void OnEnable() => _inputActions.Enable();
	private void OnDisable() => _inputActions.Disable();

	private void FixedUpdate()
	{
		if (_gameManager != null && !_gameManager.IsPlaying)
		{
			ApplyPlanarControl(Vector3.zero);
			var av = _rb.angularVelocity;
			av.y = 0f;
			_rb.angularVelocity = av;
			return;
		}

		UpdateGroundCheck();

		// Drag switching (optional)
		_rb.linearDamping = _isGrounded ? _linearDragGround : _linearDragAir;

		Vector2 moveInput = GetEffectiveMoveInput();

		bool hasMoveInput = moveInput.sqrMagnitude > (_moveDeadzone * _moveDeadzone);
		if (!hasMoveInput)
			moveInput = Vector2.zero;

		// 1) Desired move direction (camera-relative)
		Vector3 moveDir = GetMoveDirectionFromInput(moveInput);

		// 2) Ground projection for slopes
		if (_isGrounded && moveDir.sqrMagnitude > 0.01f)
		{
			moveDir = Vector3.ProjectOnPlane(moveDir, _groundNormal).normalized;
		}

		// 3) Compute target planar velocity
		float speed = _isRunning ? _runSpeed : _walkSpeed;
		Vector3 targetPlanarVel = (moveDir.sqrMagnitude > 0.01f) ? moveDir * speed : Vector3.zero;

		// 4) Apply planar acceleration (Rigidbody-style)
		ApplyPlanarControl(targetPlanarVel);

		// 5) Gravity / snap-down
		ApplyGravityAndSnap();

		// 6) Rotation
		if (hasMoveInput)
		{
			HandleRotation(moveDir);
		}

		// Stop unwanted physics spin when there is no rotation intent.
		// This prevents "keep spinning" even when move inputs are zero.
		if (!hasMoveInput)
		{
			var av = _rb.angularVelocity;
			av.y = 0f;
			_rb.angularVelocity = av;
		}

		// 7) Animation
		_animationManager.SetIsGrounded(_isGrounded);
		_animationManager.PlayWalkAnimation(_rb.linearVelocity);
	}

	private Vector2 GetEffectiveMoveInput()
	{
		if (_starterInputs != null && _starterInputs.move.sqrMagnitude > 0.001f)
			return _starterInputs.move;

		return _moveInput;
	}

	private void UpdateGroundCheck()
	{
		Vector3 origin = _rb.position + Vector3.up * _groundCastUpOffset;
		Vector3 dir = Vector3.down;

		bool grounded = false;
		Vector3 normal = Vector3.up;

		if (Physics.SphereCast(
				origin,
				_groundCastRadius,
				dir,
				out RaycastHit hit,
				_groundCheckDistance + _groundCastUpOffset,
				_groundLayers,
				QueryTriggerInteraction.Ignore))
		{
			if (hit.normal.y >= _minGroundNormalY)
			{
				grounded = true;
				normal = hit.normal;
			}
		}

		bool wasGrounded = _isGrounded;
		_isGrounded = grounded;
		_groundNormal = normal;

		if (_isGrounded && !wasGrounded)
		{
			_jumpCount = 0;
			_animationManager.SetIsJump(false);
		}
	}

	private void ApplyPlanarControl(Vector3 targetPlanarVel)
	{
		Vector3 v = _rb.linearVelocity;

		Vector3 currentPlanar = new Vector3(v.x, 0f, v.z);
		Vector3 delta = targetPlanarVel - currentPlanar;

		float accel = _isGrounded ? _accel : _airAccel;

		// Clamp acceleration by max change per fixed step
		Vector3 maxDelta = Vector3.ClampMagnitude(delta, accel * Time.fixedDeltaTime * Mathf.Max(1f, (_isGrounded ? _runSpeed : _runSpeed)));

		// Apply as velocity change (mass-independent)
		_rb.AddForce(new Vector3(maxDelta.x, 0f, maxDelta.z), ForceMode.VelocityChange);
	}

	private void ApplyGravityAndSnap()
	{
		if (_useBuiltinGravity)
		{
			// Optional snap-down to keep stable contact on slopes/steps
			if (_isGrounded)
			{
				Vector3 v = _rb.linearVelocity;
				if (v.y < -_groundSnapDown)
				{
					v.y = -_groundSnapDown;
					_rb.linearVelocity = v;
				}
			}
			return;
		}

		// Custom gravity
		_rb.AddForce(Vector3.down * _gravity, ForceMode.Acceleration);

		// Snap-down when grounded to reduce micro-bounces
		if (_isGrounded)
		{
			Vector3 v = _rb.linearVelocity;
			if (v.y < -_groundSnapDown)
			{
				v.y = -_groundSnapDown;
				_rb.linearVelocity = v;
			}
		}
	}

	private void HandleRotation(Vector3 moveDirection)
	{
		if (moveDirection.sqrMagnitude < 0.001f)
			return;

		Vector3 targetForward = moveDirection;
		targetForward.y = 0f;

		if (targetForward.sqrMagnitude < 0.0001f)
			return;
		targetForward.Normalize();

		Quaternion targetRot = Quaternion.LookRotation(targetForward, Vector3.up);

		float maxDegrees = _maxYawPerSecond * Time.fixedDeltaTime;
		Quaternion newRot = Quaternion.RotateTowards(_rb.rotation, targetRot, maxDegrees);

		_rb.MoveRotation(newRot);
	}

	private Vector3 GetMoveDirectionFromInput(Vector2 moveInput)
	{
		if (moveInput.sqrMagnitude < 0.01f)
			return Vector3.zero;

		Camera cam = Camera.main;
		if (cam == null)
		{
			return (transform.right * moveInput.x + transform.forward * moveInput.y).normalized;
		}

		Vector3 camForward = cam.transform.forward;
		camForward.y = 0f;
		camForward.Normalize();

		Vector3 camRight = cam.transform.right;
		camRight.y = 0f;
		camRight.Normalize();

		return (camRight * moveInput.x + camForward * moveInput.y).normalized;
	}

	public void OnJumpStarted(InputAction.CallbackContext ctx)
	{
		// dontjump wait fix
		return;

		if (_jumpCount >= _maxJumpCount)
			return;

		// Keep your original rule:
		// - Allow jump if grounded
		// - Allow 2nd jump in air (jumpCount >= 1)
		if (_isGrounded || _jumpCount >= 1)
		{
			DoJump(_normalJumpHeight);
			_jumpCount++;
		}
	}

	private void DoJump(float height)
	{
		// v = sqrt(2gh)
		float jumpSpeed = Mathf.Sqrt(height * 2f * _gravity);

		// Reset current downward velocity so jump feels consistent
		Vector3 v = _rb.linearVelocity;
		if (v.y < 0f) v.y = 0f;
		_rb.linearVelocity = v;

		// Apply jump as instant velocity change
		_rb.AddForce(Vector3.up * jumpSpeed, ForceMode.VelocityChange);

		_animationManager.Animator.SetTrigger("Jump");
		_animationManager.Animator.Play("JumpStart", 0, 0f);
		_animationManager.SetIsJump(true);
		_animationManager.SetIsGrounded(false);

		_isGrounded = false;
	}

	public void OnUIJumpStart()
	{
		OnJumpStarted(new InputAction.CallbackContext());
	}

	public void ToggleRun()
	{
		_isRunning = !_isRunning;
	}
}
