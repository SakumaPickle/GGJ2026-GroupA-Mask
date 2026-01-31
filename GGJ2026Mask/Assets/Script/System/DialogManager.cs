using UnityEngine;

public class DialogManager : MonoBehaviour
{
	[SerializeField] GameObject settingDialog;

	public void OnClicSettingButton()
	{
		settingDialog.SetActive(true);
	}
}
