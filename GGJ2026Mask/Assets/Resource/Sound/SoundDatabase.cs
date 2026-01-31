using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Audio/SoundDatabase")]
public class SoundDatabase : ScriptableObject
{
	public SoundItem[] bgms;
	public SoundItem[] ses;
}

[Serializable]
public class SoundItem
{
	public string name;        // Ž¯•Ê—p
	public AudioClip clip;
	[Range(0f, 1f)]
	public float volume = 1f;
	public bool loop;
}