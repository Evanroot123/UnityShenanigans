using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreKeeper : MonoBehaviour
{
	public static int score { get; private set; }
	float lastEnemyKillTime;
	int streakCount;
	float streakExpiryTime = 1;


	private void Start()
	{
		Enemy.OnDeathStatic -= OnEnemyKilled;
		Enemy.OnDeathStatic += OnEnemyKilled;
	}

	void OnEnemyKilled()
	{
		if (Time.time < lastEnemyKillTime + streakExpiryTime)
		{
			streakCount++;
		}
		else
		{
			streakCount = 0;
		}

		lastEnemyKillTime = Time.time;

		score += 5 + streakCount * streakCount;
	}
}
