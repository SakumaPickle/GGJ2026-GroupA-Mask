using Cysharp.Threading.Tasks;
using UnityEngine;

public class Result : MonoBehaviour
{

	void Start()
	{
		TransitFader.Instance.FadeIn().Forget();
	}

	void Update()
	{

	}

	public void OnClickNextButton()
	{
		SoundManager.Instance.PlaySE(SoundManager.Se.Cancel);	
		TransitFader.Instance.FadeOutAsync("TitleScene").Forget();
	}
}
