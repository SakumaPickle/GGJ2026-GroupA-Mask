using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class Game : MonoBehaviour
{
	[SerializeField] private float _gameTime = 60f;
	[SerializeField] private TextMeshProUGUI _timeText;

	private bool _isFinish = false;

	void Start()
	{
		// TransitFader.Instance.FadeIn().Forget();
		// SoundManager.Instance.PlayBGM(SoundManager.Bgm.Nostalgia);
	}

	void Update()
	{
		if (_isFinish)
		{
			return;
		}

		if (_gameTime > 0)
		{
			_gameTime -= Time.deltaTime;
			_timeText.text = $"Time:{_gameTime:F0}";
		}

		_isFinish = _gameTime <= 0;




	}

	private void TimeUp()
	{
		// タイムアップ演出作る



	}

	// 仮でボタン押下でリザルト画面に遷移
	public void OnClickResult()
	{
		SoundManager.Instance.StopBGM();
		SoundManager.Instance.PlaySE(SoundManager.Se.Decision);
		TransitFader.Instance.FadeOutAsync("ResultScene").Forget();
	}
}
