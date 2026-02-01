using Cysharp.Threading.Tasks;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;

public class Result : MonoBehaviour
{

	[SerializeField] private TextMeshProUGUI _resultText;
	[SerializeField] private GameObject _anyKeyDown;

	private bool _isClose;

	private void Awake()
	{
		_anyKeyDown.SetActive(false);
	}

	public void SetResult(int count)
	{
		_resultText.text = $"Count:{count}";
	}

	public async UniTask OpenResultAsync()
	{
		this.gameObject.SetActive(true);

		await UniTask.WaitForSeconds(0.5f);

		_anyKeyDown.SetActive(true);
		_isClose = true;
	}

	private void Update()
	{
		bool pressed = false;

		if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
			pressed = true;

		if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
			pressed = true;

		if (Gamepad.current != null)
		{
			foreach (var control in Gamepad.current.allControls)
			{
				if (control is ButtonControl button && button.wasPressedThisFrame)
				{
					pressed = true;
					break;
				}
			}
		}

		if (_isClose && pressed)
		{
			OnClickNextButton();
		}
	}

	public void OnClickNextButton()
	{
		if (TransitFader.Instance != null)
		{
			SoundManager.Instance.StopBGM();
			SoundManager.Instance.PlaySE(SoundManager.Se.Cancel);
			TransitFader.Instance.FadeOutAsync("TitleScene").Forget();
		}
		else
		{
			Debug.Log("Dummy Scene Load");
			SceneManager.LoadScene("TitleScene");
		}
	}
}
