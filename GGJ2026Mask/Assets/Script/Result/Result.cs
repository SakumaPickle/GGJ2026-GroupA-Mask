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
		TransitFader.Instance.FadeOutAsync("TitleScene").Forget();
	}
}
