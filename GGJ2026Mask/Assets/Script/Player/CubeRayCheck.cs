using UnityEngine;
using UnityEngine.InputSystem;

public class CubeRayCheck : MonoBehaviour
{
	[SerializeField] private Color _hoverColor = Color.red;
	[SerializeField] private Renderer _renderer;

	private Color _originalColor;

	void Start()
	{
		_originalColor = _renderer.material.color;
	}

	void Update()
	{
		// マウス座標取得（Input System方式）
		var mousePos = Mouse.current.position.ReadValue();

		// カメラからレイを飛ばす
		var ray = Camera.main.ScreenPointToRay(mousePos);

		// 自分に当たったら色を変える
		if (Physics.Raycast(ray, out var hit))
		{
			if (hit.transform == this.transform)
			{
				_renderer.material.color = _hoverColor;
			}
			else
			{
				_renderer.material.color = _originalColor;
			}
		}
		else
		{
			_renderer.material.color = _originalColor;
		}
	}
}
