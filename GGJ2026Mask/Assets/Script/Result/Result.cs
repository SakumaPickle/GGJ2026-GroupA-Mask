using Cysharp.Threading.Tasks;
using UnityEngine;

public class Result : MonoBehaviour
{

	void Start()
	{
		TransitFader.Instance.FadeIn().Forget();
		SoundManager.Instance.PlayBGM(SoundManager.Bgm.Art_Break);
	}

	void Update()
	{

	}

	public void OnClickNextButton()
	{
		SoundManager.Instance.StopBGM();
		SoundManager.Instance.PlaySE(SoundManager.Se.Cancel);	
		TransitFader.Instance.FadeOutAsync("TitleScene").Forget();
	}
}
