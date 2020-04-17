﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public enum AudioChannel { Master, Sfx, Music };

	float masterVolumePercent = 1;
	float sfxVolumePercent = 1;
	float musicVolumePercent = 1;

	AudioSource sfx2DSource;
	AudioSource[] musicSources;
	int activeMusicSourceIndex;

	public static AudioManager instance;

	Transform audioListener;
	Transform playerT;

	SoundLibrary library;

	private void Awake()
	{
		if (instance != null)
		{
			Destroy(gameObject);
		}

		instance = this;
		DontDestroyOnLoad(gameObject);
		library = GetComponent<SoundLibrary>();
		musicSources = new AudioSource[2];
		for (int i = 0; i < 2; i++)
		{
			GameObject newMusicSource = new GameObject("Music Source " + (i + 1));
			musicSources[i] = newMusicSource.AddComponent<AudioSource>();
			newMusicSource.transform.parent = transform;
		}

		GameObject newSfx2Dsource = new GameObject("2D sfx source");
		sfx2DSource = newSfx2Dsource.AddComponent<AudioSource>();
		newSfx2Dsource.transform.parent = transform;

		audioListener = FindObjectOfType<AudioListener>().transform;
		playerT = FindObjectOfType<Player>().transform;

		//masterVolumePercent = PlayerPrefs.GetFloat("master vol");
		//sfxVolumePercent = PlayerPrefs.GetFloat("sfx vol");
		//musicVolumePercent = PlayerPrefs.GetFloat("music vol");
	}

	private void Update()
	{
		if (playerT != null)
		{
			audioListener.position = playerT.position;
		}
	}

	public void SetVolume(float volumePercent, AudioChannel channel)
	{
		switch(channel)
		{
			case AudioChannel.Master:
				masterVolumePercent = volumePercent;
				break;
			case AudioChannel.Sfx:
				sfxVolumePercent = volumePercent;
				break;
			case AudioChannel.Music:
				musicVolumePercent = volumePercent;
				break;
		}

		musicSources[0].volume = musicVolumePercent * masterVolumePercent;
		musicSources[1].volume = musicVolumePercent * masterVolumePercent;

		PlayerPrefs.SetFloat("master vol", masterVolumePercent);
		PlayerPrefs.SetFloat("sfx vol", sfxVolumePercent);
		PlayerPrefs.SetFloat("music vol", musicVolumePercent);
	}

	public void PlayMusic(AudioClip clip, float fadeDuration = 1)
	{
		activeMusicSourceIndex = 1 - activeMusicSourceIndex;
		musicSources[activeMusicSourceIndex].clip = clip;
		musicSources[activeMusicSourceIndex].Play();

		StartCoroutine(AnimateMusicCrossfade(fadeDuration));
	}

	public void PlaySound(AudioClip clip, Vector3 position)
	{
		if (clip != null)
			AudioSource.PlayClipAtPoint(clip, position, sfxVolumePercent * masterVolumePercent);
	}

	public void PlaySound(string soundName, Vector3 position)
	{
		PlaySound(library.GetClipFromName(soundName), position);
	}

	public void PlaySound2D(string soundName)
	{
		sfx2DSource.PlayOneShot(library.GetClipFromName(soundName), sfxVolumePercent * masterVolumePercent);
	}

	IEnumerator AnimateMusicCrossfade(float duration)
	{
		float percent = 0;

		while(percent < 1)
		{
			percent += Time.deltaTime * 1 / duration;
			musicSources[activeMusicSourceIndex].volume = Mathf.Lerp(0, musicVolumePercent * masterVolumePercent, percent);
			musicSources[1 - activeMusicSourceIndex].volume = Mathf.Lerp(0, musicVolumePercent * masterVolumePercent, percent);
			yield return null;
		}
	}
}
