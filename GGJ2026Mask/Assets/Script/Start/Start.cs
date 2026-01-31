using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Satrt : MonoBehaviour
{
	[SerializeField] private GameObject _dontDestoroy;
	void Start()
	{
		DontDestroyOnLoad(_dontDestoroy);

		var bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 0.5f);
		SoundManager.Instance.SetBGMVolume(bgmVolume);

		var seVolume = PlayerPrefs.GetFloat("SEVolume", 0.5f);
		SoundManager.Instance.SetSEVolume(seVolume);

		SceneManager.LoadScene("TitleScene");
	}
}
