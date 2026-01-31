using Cysharp.Threading.Tasks;
using UnityEngine;

public class Game : MonoBehaviour
{
	void Start()
	{
		TransitFader.Instance.FadeIn().Forget();
	}

	void Update()
	{
		
	}

	// 仮でボタン押下でリザルト画面に遷移
	public void OnClickResult()
	{
		TransitFader.Instance.FadeOutAsync("ResultScene").Forget();
	}
}
