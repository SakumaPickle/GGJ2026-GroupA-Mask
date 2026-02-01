using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

public class CameraRayCastCheck : MonoBehaviour
{
	[SerializeField] private Camera _playerCamera;
	[SerializeField] private InputSystem_Actions _inputActions;

	[Header("Raycast")]
	[SerializeField] private float _rayDistance = 100f;
	[SerializeField] private LayerMask _targetLayers = ~0;
	[SerializeField] private QueryTriggerInteraction _triggerInteraction = QueryTriggerInteraction.Ignore;

	[Header("Outline (URP)")]
	[SerializeField] private Material _outlineMaterial;

	[Tooltip("子Colliderに当たっても親(marker)を拾う前提。outlineはmarker配下のMeshRenderer全部を対象にする")]
	[SerializeField] private bool _outlineAllChildMeshRenderers = true;

	[Tooltip("Runtimeでアウトライン用の子オブジェクトを自動生成してON/OFFします")]
	[SerializeField] private bool _useRuntimeOutlineChild = true;

	private GameObject _hitObject;
	private bool _isTouchUI;

	private DespawnEnemyMarker _currentMarker;

	// markerごとに生成したアウトライン子を管理
	private readonly Dictionary<DespawnEnemyMarker, GameObject> _outlineChildByMarker = new Dictionary<DespawnEnemyMarker, GameObject>(64);

	// 今表示中のアウトライン子（切替用）
	private GameObject _currentOutlineChild;

	private void Awake()
	{
		if (_inputActions == null)
			_inputActions = new InputSystem_Actions();

		if (_playerCamera == null)
			_playerCamera = Camera.main;
	}

	private void Update()
	{
		CheckCollider();
		_isTouchUI = IsAnyTouchOverUI();
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

		SetCurrentOutlineVisible(false);
		_currentMarker = null;
		_currentOutlineChild = null;
	}

	private bool CheckCollider()
	{
		var ray = new Ray(_playerCamera.transform.position, _playerCamera.transform.forward);

		if (Physics.Raycast(ray, out var hit, _rayDistance, _targetLayers, _triggerInteraction))
		{
			var marker = hit.collider.GetComponentInParent<DespawnEnemyMarker>();
			if (marker == null)
			{
				_hitObject = null;
				ClearCurrentTarget();
				return false;
			}

			_hitObject = marker.gameObject;

			if (_hitObject.CompareTag("DontSelected"))
			{
				_hitObject = null;
				ClearCurrentTarget();
				return false;
			}

			if (ReferenceEquals(marker, _currentMarker))
				return true;

			_currentMarker = marker;

			// 対象が変わったのでアウトライン切替
			if (_useRuntimeOutlineChild)
			{
				ShowOutlineByRuntimeChild(marker, true);
			}

			return true;
		}

		_hitObject = null;
		ClearCurrentTarget();
		return false;
	}

	private void PointerDown(InputAction.CallbackContext ctx)
	{
		if (_hitObject == null) return;
		if (_isTouchUI) return;

		// 破棄前にアウトラインOFF
		SetCurrentOutlineVisible(false);
		_currentMarker = null;
		_currentOutlineChild = null;

		Destroy(_hitObject);
		_hitObject = null;
	}

	private void ClearCurrentTarget()
	{
		SetCurrentOutlineVisible(false);
		_currentMarker = null;
		_currentOutlineChild = null;
	}

	private void SetCurrentOutlineVisible(bool visible)
	{
		if (_currentOutlineChild != null)
		{
			_currentOutlineChild.SetActive(visible);
		}
	}

	private void ShowOutlineByRuntimeChild(DespawnEnemyMarker marker, bool visible)
	{
		if (_outlineMaterial == null)
		{
			SetCurrentOutlineVisible(false);
			return;
		}

		// 前の対象のアウトラインをOFF
		SetCurrentOutlineVisible(false);

		// まだ生成していないなら生成
		if (!_outlineChildByMarker.TryGetValue(marker, out var outlineChild) || outlineChild == null)
		{
			outlineChild = CreateOutlineChild(marker);
			_outlineChildByMarker[marker] = outlineChild;
		}

		_currentOutlineChild = outlineChild;
		_currentOutlineChild.SetActive(visible);
	}

	private GameObject CreateOutlineChild(DespawnEnemyMarker marker)
	{
		var root = marker.gameObject;

		var outlineRoot = new GameObject("__OutlineHull");
		outlineRoot.transform.SetParent(root.transform, false);
		outlineRoot.SetActive(false);

		// marker配下の MeshRenderer を対象に、同じメッシュでアウトライン専用Rendererを複製
		if (_outlineAllChildMeshRenderers)
		{
			var sources = root.GetComponentsInChildren<MeshRenderer>(true);
			for (int i = 0; i < sources.Length; i++)
			{
				var srcR = sources[i];
				if (srcR == null) continue;

				var srcF = srcR.GetComponent<MeshFilter>();
				if (srcF == null || srcF.sharedMesh == null) continue;

				// 元Rendererと同じ階層構造で置きたい場合は、相対パスを作る必要があるが
				// まずは「同じローカルTRS」を複製するだけでOK（密着して見える）
				var child = new GameObject(srcR.gameObject.name + "__Outline");
				child.transform.SetParent(outlineRoot.transform, false);

				// 位置合わせ：元のローカルTRSをそのままコピー
				child.transform.localPosition = srcR.transform.localPosition;
				child.transform.localRotation = srcR.transform.localRotation;
				child.transform.localScale = srcR.transform.localScale;

				var mf = child.AddComponent<MeshFilter>();
				mf.sharedMesh = srcF.sharedMesh;

				var mr = child.AddComponent<MeshRenderer>();
				mr.sharedMaterial = _outlineMaterial; // 重要：アウトライン材1枚だけ
				mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
				mr.receiveShadows = false;
				mr.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
				mr.allowOcclusionWhenDynamic = false;
			}
		}
		else
		{
			var srcR = root.GetComponent<MeshRenderer>();
			if (srcR != null)
			{
				var srcF = srcR.GetComponent<MeshFilter>();
				if (srcF != null && srcF.sharedMesh != null)
				{
					var child = new GameObject(srcR.gameObject.name + "__Outline");
					child.transform.SetParent(outlineRoot.transform, false);
					child.transform.localPosition = srcR.transform.localPosition;
					child.transform.localRotation = srcR.transform.localRotation;
					child.transform.localScale = srcR.transform.localScale;

					var mf = child.AddComponent<MeshFilter>();
					mf.sharedMesh = srcF.sharedMesh;

					var mr = child.AddComponent<MeshRenderer>();
					mr.sharedMaterial = _outlineMaterial;
					mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
					mr.receiveShadows = false;
					mr.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
					mr.allowOcclusionWhenDynamic = false;
				}
			}
		}

		return outlineRoot;
	}

	private bool IsAnyTouchOverUI()
	{
		if (EventSystem.current == null) return false;

		foreach (var t in UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches)
		{
			if (EventSystem.current.IsPointerOverGameObject(t.touchId))
				return true;
		}

		return false;
	}
}
