using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SFX
{
	Hit,
	EndMatch
}

public enum Music
{
	Level
}

public class AudioManager : MonoBehaviour
{

	[HideInInspector]
	public static AudioManager instance = null;

#pragma warning disable CS0649
	[Header("Audio Sources")]
	[SerializeField] private AudioSource musicSource;
	[SerializeField] private AudioSource[] sfxSourcePool;
	private int sfxPoolIndex = 0;
	[Space(20)]
	[Header("SFX")]
	[SerializeField] private AudioClip hitEffect;
	[SerializeField] private float hitVolume = 0.6f;
	[SerializeField] private AudioClip endMatchEffect;
	[SerializeField] private float endMatchVolume = 0.6f;
	[Range(0, 2)] [SerializeField] private float sfxGlobalVolume = 1f;
	[Range(0, 1)] [SerializeField] private float sfxPitchRandomization = 0f;
	[Space(20)]
	[Header("Music")]
	[SerializeField] private AudioClip Level;
	[SerializeField] private float level1Volume = 1f;
	[Range(0, 0.5f)] [SerializeField] private float musicPitchDistortionMultiplier = 0.5f;
	private Coroutine musicDistortionCoroutine;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(this);
			return;
		}

		DontDestroyOnLoad(this);

		musicSource = gameObject.AddComponent<AudioSource>();
		sfxSourcePool = new AudioSource[10];
		for(int i = 0; i < sfxSourcePool.Length; i++)
        {
			sfxSourcePool[i] = gameObject.AddComponent<AudioSource>();
		}
    }

    public void PlaySFX(SFX sfx, bool randomizePitch = true)
	{
		AudioClip clip = null;
		float volume = 0f;
		switch (sfx)
		{
			case SFX.Hit:
				clip = hitEffect;
				volume = hitVolume;
				break;
			case SFX.EndMatch:
				clip = endMatchEffect;
				volume = endMatchVolume;
				break;
		}

		if (randomizePitch)
		{
			sfxSourcePool[sfxPoolIndex].pitch = 1 + Random.Range(-sfxPitchRandomization, sfxPitchRandomization);
		}
		sfxSourcePool[sfxPoolIndex].volume = sfxGlobalVolume * volume;
		sfxSourcePool[sfxPoolIndex].PlayOneShot(clip);
		sfxPoolIndex = (sfxPoolIndex + 1) % sfxSourcePool.Length;
	}

	public void PlayMusic(Music track)
	{
		musicSource.Stop();
		AudioClip trackClip;
		float volume;
		switch (track)
		{
			case Music.Level:
				trackClip = Level;
				volume = level1Volume;
				break;
			default:
				return;
		}
		musicSource.clip = trackClip;
		musicSource.volume = volume;
		musicSource.loop = true;
		musicSource.Play();
	}

	public void StartMusicDistortion()
	{
		if (musicDistortionCoroutine != null)
		{
			return;
		}
		musicDistortionCoroutine = StartCoroutine(_StartMusicDistortion());
	}

	public void StopMusicDistortion()
	{
		if (musicDistortionCoroutine != null)
		{
			StopCoroutine(musicDistortionCoroutine);
			musicDistortionCoroutine = null;
		}
		StartCoroutine(_StopMusicDistortion());
	}

	private IEnumerator _StartMusicDistortion()
	{
		while (true)
		{
			musicSource.pitch = (1 - musicPitchDistortionMultiplier) + (musicPitchDistortionMultiplier * Mathf.Sin(Time.time));
			yield return new WaitForEndOfFrame();
		}
	}
	private IEnumerator _StopMusicDistortion()
	{
		float t = 0;
		float start = musicSource.pitch;
		while (Mathf.Abs(musicSource.pitch - 1) > float.Epsilon)
		{
			t += Time.deltaTime / 0.2f;
			musicSource.pitch = Mathf.SmoothStep(start, 1, t);
			yield return new WaitForEndOfFrame();
		}
		musicSource.pitch = 1;
	}
}
