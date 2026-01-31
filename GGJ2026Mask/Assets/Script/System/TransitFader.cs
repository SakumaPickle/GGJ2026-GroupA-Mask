using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class TransitFader : MonoBehaviour
{
	[SerializeField] private Image _faderImage;

	private Color faderInColor = new Color(0, 0, 0, 0);
	private Color faderOutColor = new Color(0, 0, 0, 1);

	private static TransitFader _instance;

	public static TransitFader Instance
	{
		get
		{
			if (_instance == null)
			{
				Debug.Log("TransitFader Instance is null");
			}
			else
			{
				return _instance;
			}
			return null;
		}
	}

	public bool isFading;

	private void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
		}
		else
		{
			Destroy(this.gameObject);
		}

	}
	void Start()
	{

	}

	public async UniTask FadeIn()
	{
		await _faderImage.DOColor(faderInColor, 1f).ToUniTask();
		isFading = false;
	}

	public async UniTask FadeOutAsync(string sceneName)
	{
		isFading = true;
		await _faderImage.DOColor(faderOutColor, 1f).ToUniTask();

		SceneManager.LoadScene(sceneName);
	}
}
