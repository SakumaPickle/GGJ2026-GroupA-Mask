using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class Game : MonoBehaviour
{
	[SerializeField] private float _gameTime = 60f;
	[SerializeField] private TextMeshProUGUI _timeText;
	[SerializeField] private Transform _startText;
	[SerializeField] private Transform _finishText;

	private bool _isFinish = false;
	private bool _isInitialize = false;

	void Start()
	{
		// todo: WaitEnemyLoad

		if (TransitFader.Instance != null)
		{
			TransitFader.Instance.FadeIn().Forget();
			SoundManager.Instance.PlayBGM(SoundManager.Bgm.Nostalgia);
		}

		StartEffect().Forget();
	}

	void Update()
	{
		if (!_isInitialize || _isFinish)
		{
			return;
		}

		if (_gameTime > 0)
		{
			_gameTime -= Time.deltaTime;
			_timeText.text = $"Time:{_gameTime:F0}";
		}

		_isFinish = _gameTime <= 0;

		if (_isFinish)
		{
			TimeUp();
		}


	}

	private async UniTask StartEffect()
	{
		await _startText.DOLocalMoveX(0, 1f).ToUniTask(cancellationToken: destroyCancellationToken);

		await UniTask.WaitForSeconds(0.5f);

		await _startText.DOLocalMoveX(1000, 1f).ToUniTask(cancellationToken: destroyCancellationToken);

		_isInitialize = true;
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
