using UnityEngine;
using UnityEngine.UI;

public class SettingDialog : MonoBehaviour
{
	[SerializeField] private Slider _bgmSlider;
	[SerializeField] private Slider _seSlider;

	private void Start()
	{
		_bgmSlider.normalizedValue = PlayerPrefs.GetFloat("BGMVolume", 0.5f);
		_seSlider.normalizedValue = PlayerPrefs.GetFloat("SEVolume", 0.5f);
	}

	private void Update()
	{
		if (gameObject.activeSelf)
		{
			var bgmVolume = _bgmSlider.normalizedValue;
			SoundManager.Instance.SetBGMVolume(bgmVolume);

			var seVolume = _seSlider.normalizedValue;
			SoundManager.Instance.SetSEVolume(seVolume);
		}
	}

	public void OnClose()
	{
		PlayerPrefs.SetFloat("BGMVolume", _bgmSlider.normalizedValue);
		PlayerPrefs.SetFloat("SEVolume", _seSlider.normalizedValue);
		gameObject.SetActive(false);
	}

	public void OnClickSettingButton()
	{
		gameObject.SetActive(true);
	}
}
