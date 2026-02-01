using UnityEngine;

public class TipsDialog : MonoBehaviour
{
	void Start()
	{

	}

	void Update()
	{

	}

	public void OpenTips()
	{
		gameObject.SetActive(true);
	}


	public void OnClickTipsCloseButton()
	{
		SoundManager.Instance.PlaySE(SoundManager.Se.Cancel);
		gameObject.SetActive(false);
	}
}
