using Cysharp.Threading.Tasks;
using UnityEngine;

public class Game : MonoBehaviour
{
	void Start()
	{
		TransitFader.Instance.FadeIn().Forget();
		SoundManager.Instance.PlayBGM(SoundManager.Bgm.make_me_happy);
	}

	void Update()
	{
		
	}

	// 仮でボタン押下でリザルト画面に遷移
	public void OnClickResult()
	{
		SoundManager.Instance.StopBGM();
		SoundManager.Instance.PlaySE(SoundManager.Se.Decision);
		TransitFader.Instance.FadeOutAsync("ResultScene").Forget();
	}
}
