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
		TransitFader.Instance.FadeOutAsync("GameScene").Forget();
	}
}
