using Cysharp.Threading.Tasks;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;

public class Result : MonoBehaviour
{

	[SerializeField] private TextMeshProUGUI _resultText;

	public void SetResult(int count)
	{
		_resultText.text = $"Count:{count}";
	}

	public void OpenResult()
	{
		this.gameObject.SetActive(true);
	}

	public void OnClickNextButton()
	{
		SoundManager.Instance.StopBGM();
		SoundManager.Instance.PlaySE(SoundManager.Se.Cancel);
		TransitFader.Instance.FadeOutAsync("TitleScene").Forget();
	}
}
