using Cysharp.Threading.Tasks;
using StarterAssets;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;

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
		var inputs = GameObject.Find("UI_Canvas_StarterAssetsInputs_Joysticks");

		Debug.Log(inputs);
		// TODO:IwataÉVÅ[Éìí≤êÆèIÇÌÇ¡ÇΩÇÁíºÇ∑
		if (inputs != null)
		{
			inputs.GetComponent<StarterAssetsInputs>().cursorLocked = false;
		}

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
