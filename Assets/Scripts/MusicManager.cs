using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
	public AudioClip mainTheme;
	public AudioClip menuTheme;

	string sceneName;

	private void Start()
	{
		AudioManager.instance.PlayMusic(menuTheme, 2);
		OnLevelWasLoaded(0);
	}

	private void OnLevelWasLoaded(int level)
	{
		string newSceneName = SceneManager.GetActiveScene().name;
		if (newSceneName != sceneName)
		{
			sceneName = newSceneName;
			Invoke("PlayMusic", .2f);
		}
	}

	void PlayMusic()
	{
		AudioClip clipToPlay = null;

		if (sceneName == "Menu")
		{
			clipToPlay = menuTheme;
		}
		else
		{
			clipToPlay = mainTheme;
		}

		AudioManager.instance.PlayMusic(clipToPlay, 2);
		Invoke("PlayMusic", clipToPlay.length);
	}
}
