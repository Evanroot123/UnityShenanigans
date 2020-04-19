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
	public Text scoreUI;
	public RectTransform healthBar;
	public Text gameOverScoreUI;

	Spawner spawner;
	Player player;

	private void Awake()
	{
		spawner = FindObjectOfType<Spawner>();
		spawner.OnNewWave += OnNewWave;
	}

	void Start()
    {
		player = FindObjectOfType<Player>();
		FindObjectOfType<Player>().OnDeath += OnGameOver;
    }

	private void Update()
	{
		scoreUI.text = ScoreKeeper.score.ToString("D6");
		float healthPercent = 0;
		if (player != null)
		{
			healthPercent = player.health / player.startingHealth;
		}

		healthBar.localScale = new Vector3(healthPercent, 1, 1);
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
		StartCoroutine(Fade(Color.red, new Color(0,0,0,.95f), 1));
		gameOverScoreUI.text = scoreUI.text;
		scoreUI.gameObject.SetActive(false);
		healthBar.transform.parent.gameObject.SetActive(false);
		Cursor.visible = true;
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

	public void ReturnToMainMenu()
	{
		SceneManager.LoadScene("Menu");
	}
}
