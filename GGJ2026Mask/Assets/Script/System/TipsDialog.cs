using UnityEngine;

public class TipsDialog : MonoBehaviour
{
	void Start()
	{

	}

	void Update()
	{

	}


	public void OnClickTipsCloseButton()
	{
		SoundManager.Instance.PlaySE(SoundManager.Se.Cancel);
		gameObject.SetActive(false);
	}
}
