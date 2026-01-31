using Cysharp.Threading.Tasks;
using UnityEngine;

public class Title : MonoBehaviour
{
	void Start()
	{
		TransitFader.Instance.FadeIn().Forget();
		SoundManager.Instance.PlayBGM(SoundManager.Bgm.Random_Walker);
	}

	public void OnClickGameStart()
	{
		SoundManager.Instance.StopBGM();
		SoundManager.Instance.PlaySE(SoundManager.Se.Decision);
		TransitFader.Instance.FadeOutAsync("GameScene").Forget();
	}
}
