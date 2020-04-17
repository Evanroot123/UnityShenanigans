using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
	public Image fadePlane;
	public GameObject gameOverUI;

	public RectTransform newWaveBanner;
	public Text newWaveTitle;
	public Text newWaveEnemyCount;

	Spawner spawner;

	private void Awake()
	{
		spawner = FindObjectOfType<Spawner>();
		spawner.OnNewWave += OnNewWave;
	}

	void Start()
    {
		FindObjectOfType<Player>().OnDeath += OnGameOver;
    }

	void OnNewWave(int waveNumber)
	{
		string[] numbers = { "One", "Two", "Three", "Four", "Five" };
		newWaveTitle.text = "- Wave " + numbers[waveNumber - 1] + " -";
		string enemyCountString = spawner.waves[waveNumber - 1].infinite ? "Infinite" : spawner.waves[waveNumber - 1].enemyCount + "";
		newWaveEnemyCount.text = "Enemies: " + spawner.waves[waveNumber - 1].enemyCount;

		StopCoroutine("AnimateNewWaveBanner");
		StartCoroutine("AnimateNewWaveBanner");
	}

	IEnumerator AnimateNewWaveBanner()
	{
		float animatePercent = 0;
		float speed = 2.5f;
		float delayTime = 2f;
		int dir = 1;
		float endDelayTime = Time.time + 1 / speed + delayTime;

		while(animatePercent >= 0)
		{
			animatePercent += Time.deltaTime * speed * dir;

			if (animatePercent >= 1)
			{
				animatePercent = 1;
				if (Time.time > endDelayTime)
				{
					dir = -1;
				}
			}

			newWaveBanner.anchoredPosition = Vector2.up * Mathf.Lerp(-310, 0, animatePercent);
			yield return null;
		}
	}

	void OnGameOver()
	{
		StartCoroutine(Fade(Color.red, Color.black, 1));
		gameOverUI.SetActive(true);
	}

	IEnumerator Fade(Color from, Color to, float time)
	{
		float speed = 1 / time;
		float percent = 0;

		while(percent < 1)
		{
			percent += Time.deltaTime * speed;
			fadePlane.color = Color.Lerp(from, to, percent);
			yield return null;
		}
	}

	public void StartNewGame()
	{
		SceneManager.LoadScene("Scene1");
		//Application.LoadLevel("Scene1");
	}
}
