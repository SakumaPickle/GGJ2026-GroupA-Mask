using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class TipsDialog : MonoBehaviour
{
	[SerializeField] private GameObject _gameStory;
	[SerializeField] private GameObject _gameControl;

	private int _pressCount;
	private bool _isGamePlay;

	void Start()
	{
		_pressCount = 0;
	}

	void Update()
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

		if (pressed)
		{
			_pressCount++;
		}

		_gameStory.SetActive(_pressCount == 0);
		_gameControl.SetActive(_pressCount == 1);

		if (!_isGamePlay && _pressCount == 2)
		{
			gameObject.SetActive(false);
			OnClickGameStart();
			_isGamePlay = true;
		}
	}

	public void OpenTips()
	{
		_gameStory.SetActive(true);
		_gameControl.SetActive(false);
		gameObject.SetActive(true);
	}

	public void OnClickGameStart()
	{
		SoundManager.Instance.StopBGM();
		SoundManager.Instance.PlaySE(SoundManager.Se.Decision);
		TransitFader.Instance.FadeOutAsync("TestScene").Forget();
	}


	public void OnClickTipsCloseButton()
	{
		SoundManager.Instance.PlaySE(SoundManager.Se.Cancel);
		gameObject.SetActive(false);
	}
}
