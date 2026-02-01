using Cysharp.Threading.Tasks;
using DG.Tweening;
using NUnit.Framework.Internal;
using TMPro;
using UnityEngine;

public class Game : MonoBehaviour
{
	[SerializeField] private float _gameTime = 60f;
	[SerializeField] private TextMeshProUGUI _timeText;
	[SerializeField] private Transform _startText;
	[SerializeField] private Transform _finishText;
	[SerializeField] private EnemyManager _enemyManager;
	[SerializeField] private TextMeshProUGUI _remineEnemyText;
	[SerializeField] private Result _result;

	private bool _isFinish = false;
	private bool _isInitialize = false;

	private int _getCount;

	public bool IsPlaying => _isInitialize && !_isFinish;

	void Start()
	{
		EnemyLoadWaitFadeAsync().Forget();
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

		_isFinish = _gameTime <= 0 || _enemyManager.RemineEnemyCount <= 0;

		_getCount = _enemyManager.EnemyMaxCount - _enemyManager.RemineEnemyCount;
		_remineEnemyText.text = $"Count:{_getCount}";

		if (_isFinish)
		{
			FinishAsync().Forget();
		}
	}

	private async UniTask EnemyLoadWaitFadeAsync()
	{
		await UniTask.WaitUntil(() => _enemyManager.IsInitialize, cancellationToken: destroyCancellationToken);

		if (TransitFader.Instance != null)
		{
			TransitFader.Instance.FadeIn().Forget();
			SoundManager.Instance.PlayBGM(SoundManager.Bgm.make_me_happy);

			await UniTask.WaitUntil(() => !TransitFader.Instance.isFading, cancellationToken: destroyCancellationToken);
		}

		await StartEffectAsync();
	}

	private async UniTask StartEffectAsync()
	{
		await _startText.DOLocalMoveX(0, 1f).ToUniTask(cancellationToken: destroyCancellationToken);

		await UniTask.WaitForSeconds(0.5f);

		await _startText.DOLocalMoveX(1000, 1f).ToUniTask(cancellationToken: destroyCancellationToken);

		_isInitialize = true;
	}

	private async UniTask FinishAsync()
	{
		await _finishText.DOLocalMoveX(0, 1f).ToUniTask(cancellationToken: destroyCancellationToken);

		await UniTask.WaitForSeconds(0.5f);

		await _finishText.DOLocalMoveX(1000, 1f).ToUniTask(cancellationToken: destroyCancellationToken);

		_result.SetResult(_getCount);
		_result.OpenResultAsync().Forget();

	}


}
