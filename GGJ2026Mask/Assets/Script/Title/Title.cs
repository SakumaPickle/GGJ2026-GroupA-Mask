using Cysharp.Threading.Tasks;
using UnityEngine;

public class Title : MonoBehaviour
{
	void Start()
	{
		TransitFader.Instance.FadeIn().Forget();
	}

	public void OnClickGameStart()
	{
		SoundManager.Instance.PlaySE(SoundManager.Se.Decision);
		TransitFader.Instance.FadeOutAsync("GameScene").Forget();
	}
}
