using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
	[SerializeField] private GameObject settingDialog;
	[SerializeField] private GameObject Tips;

	private bool _isTipsOpen;
	private bool _isGameStart;

	void Start()
	{
		if (TransitFader.Instance == null)
		{
			SceneManager.LoadScene("StartScene");
		}

		TransitFader.Instance.FadeIn().Forget();
		SoundManager.Instance.PlayBGM(SoundManager.Bgm.Random_Walker);
	}

	public void OnClickGameStart()
	{
		SoundManager.Instance.StopBGM();
		SoundManager.Instance.PlaySE(SoundManager.Se.Decision);
		TransitFader.Instance.FadeOutAsync("TestScene").Forget();
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
			var pad = Gamepad.current;

			if (pad.buttonSouth.wasPressedThisFrame || // A
				pad.buttonEast.wasPressedThisFrame || // B
				pad.buttonWest.wasPressedThisFrame || // X
				pad.buttonNorth.wasPressedThisFrame)   // Y
			{
				pressed = true;
			}
		}

		if (!_isGameStart && _isTipsOpen && pressed)
		{
			Tips.SetActive(false);
			OnClickGameStart();
		}

		if (!_isTipsOpen && !TransitFader.Instance.isFading && pressed)
		{
			OnClickTipsButton();
			_isTipsOpen = true;
		}
	}

	public void OnClicSettingButton()
	{
		SoundManager.Instance.PlaySE(SoundManager.Se.Decision);
		settingDialog.SetActive(true);
	}

	public void OnClickTipsButton()
	{
		SoundManager.Instance.PlaySE(SoundManager.Se.Decision);
		Tips.SetActive(true);
	}
}
