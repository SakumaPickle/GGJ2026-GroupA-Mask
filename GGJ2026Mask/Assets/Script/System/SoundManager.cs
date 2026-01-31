using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{

	public enum Bgm
	{
		Nostalgia,
	}

	public enum Se
	{
		Decision,
		Cancel
	}

	public static SoundManager Instance { get; private set; }

	public SoundDatabase database;
	public AudioMixer audioMixer;

	AudioSource bgmSource;
	AudioSource seSource;

	void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;

		bgmSource = gameObject.AddComponent<AudioSource>();
		seSource = gameObject.AddComponent<AudioSource>();

		bgmSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("BGM")[0];
		seSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("SE")[0];
	}

	public void PlayBGM(Bgm bgm)
	{
		var data = database.bgms[(int)bgm];
		bgmSource.clip = data.clip;
		bgmSource.volume = data.volume;
		bgmSource.loop = data.loop;
		bgmSource.Play();
	}

	public void StopBGM()
	{
		if (bgmSource.isPlaying)
		{
			bgmSource.Stop();
		}
	}

	public void PauseBGM()
	{
		if (bgmSource.isPlaying)
		{
			bgmSource.Pause();
		}
	}

	public void ResumeBGM()
	{
		if (!bgmSource.isPlaying && bgmSource.clip != null)
		{
			bgmSource.UnPause();
		}
	}

	public void PlaySE(Se se)
	{
		var data = database.ses[(int)se];
		seSource.PlayOneShot(data.clip, data.volume);
	}

	public void SetBGMVolume(float value)
	{
		audioMixer.SetFloat("BGMVolume", Mathf.Log10(value) * 20);
	}

	public void SetSEVolume(float value)
	{
		audioMixer.SetFloat("SEVolume", Mathf.Log10(value) * 20);
	}
}
