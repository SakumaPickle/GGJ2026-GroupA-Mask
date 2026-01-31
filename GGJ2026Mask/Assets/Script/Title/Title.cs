using Cysharp.Threading.Tasks;
using UnityEngine;

public class Title : MonoBehaviour
{
	[SerializeField] private GameObject settingDialog;
	[SerializeField] private GameObject Tips;

	void Start()
	{
		TransitFader.Instance.FadeIn().Forget();
		SoundManager.Instance.PlayBGM(SoundManager.Bgm.Random_Walker);
	}

	public void OnClickGameStart()
	{
		SoundManager.Instance.StopBGM();
		SoundManager.Instance.PlaySE(SoundManager.Se.Decision);
		TransitFader.Instance.FadeOutAsync("TestScene").Forget();
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
