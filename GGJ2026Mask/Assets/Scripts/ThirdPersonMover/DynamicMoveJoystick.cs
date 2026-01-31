using StarterAssets;
using UnityEngine;
using UnityEngine.EventSystems;

public class DynamicMoveJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
	[Header("Starter Assets")]
	[SerializeField] private StarterAssetsInputs _starterInputs;

	[Header("UI Parts (Move Joystick)")]
	[SerializeField] private RectTransform _joystickBase;
	[SerializeField] private RectTransform _joystickHandle;
	[SerializeField] private Canvas _canvas;

	[Header("Control")]
	[SerializeField] private float _maxRadius = 120f;
	[SerializeField] private bool _hideWhenIdle = false;

	private bool active;
	private Vector2 baseScreenPos;
	private int activePointerId = int.MinValue;

	private Vector2 initialBaseAnchoredPos;
	private bool initialCaptured;

	private void Awake()
	{
		if (!_canvas)
		{
			_canvas = GetComponentInParent<Canvas>();
		}

		CaptureInitialIfNeeded();
		ResetToInitial();
	}

	private void CaptureInitialIfNeeded()
	{
		if (initialCaptured)
		{
			return;
		}
		if (!_joystickBase)
		{
			return;
		}

		initialBaseAnchoredPos = _joystickBase.anchoredPosition;
		initialCaptured = true;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		// ç∂â∫ 1/4 ÇÃÇ›
		if (!IsInBottomLeftQuarter(eventData.position))
		{
			return;
		}

		CaptureInitialIfNeeded();

		active = true;
		activePointerId = eventData.pointerId;

		// êGÇ¡ÇΩèÍèäÇ÷à⁄ìÆ
		baseScreenPos = eventData.position;
		MoveBaseToScreen(baseScreenPos);

		UpdateStick(eventData.position);
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (!active)
		{
			return;
		}
		if (eventData.pointerId != activePointerId)
		{
			return;
		}

		UpdateStick(eventData.position);
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (!active)
		{
			return;
		}
		if (eventData.pointerId != activePointerId)
		{
			return;
		}

		active = false;
		activePointerId = int.MinValue;

		ResetToInitial();
	}

	private void ResetToInitial()
	{
		if (_joystickHandle)
		{
			_joystickHandle.anchoredPosition = Vector2.zero;
		}

		if (_joystickBase && initialCaptured)
		{
			_joystickBase.anchoredPosition = initialBaseAnchoredPos;
		}

		PushMove(Vector2.zero);

		if (_hideWhenIdle && _joystickBase)
		{
			_joystickBase.gameObject.SetActive(false);
		}
		else if (_joystickHandle)
		{
			_joystickBase.gameObject.SetActive(true);
		}
	}

	private void UpdateStick(Vector2 currentScreenPos)
	{
		var delta = currentScreenPos - baseScreenPos;
		var clamped = Vector2.ClampMagnitude(delta, _maxRadius);

		if (_joystickHandle)
		{
			_joystickHandle.anchoredPosition = clamped;
		}

		var normalized = clamped / _maxRadius;
		PushMove(normalized);
	}

	private bool IsInBottomLeftQuarter(Vector2 screenPos)
	{
		return screenPos.x < Screen.width * 0.5f && screenPos.y < Screen.height * 0.5f;
	}

	private void MoveBaseToScreen(Vector2 screenPos)
	{
		var parentRT = _joystickBase.parent as RectTransform;

		RectTransformUtility.ScreenPointToWorldPointInRectangle(
			parentRT,
			screenPos,
			null, // Overlay
			out var worldPoint
		);

		_joystickBase.position = worldPoint;
		_joystickHandle.anchoredPosition = Vector2.zero;

		if (_hideWhenIdle)
		{
			_joystickBase.gameObject.SetActive(true);
		}
	}


	private void PushMove(Vector2 v)
	{
		if (_starterInputs == null)
		{
			return;
		}
		_starterInputs.move = v;
	}
}
