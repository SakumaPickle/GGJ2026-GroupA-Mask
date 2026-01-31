using UnityEngine;
using UnityEngine.SceneManagement;

public class Satrt : MonoBehaviour
{
	[SerializeField] private GameObject _dontDestoroy;
	void Start()
	{
		DontDestroyOnLoad(_dontDestoroy);
		SceneManager.LoadScene("TitleScene");
	}
}
