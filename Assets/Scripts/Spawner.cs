using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
	public bool devMode;

	public Wave[] waves;
	public Enemy enemy;

	LivingEntity playerEntity;
	Transform playerT;

	Wave currentWave;
	int currentWaveNumber;
	int enemiesRemainingToSpawn;
	int enemiesRemainingAlive;
	float nextSpawnTime;

	MapGenerator map;

	float timeBetweenCampingChecks = 2;
	float campThresholdDistance = 1.5f;
	float nextCampCheckTime;
	Vector3 campPositionOld;
	bool isCamping;

	bool isDisabled;

	public event System.Action<int> OnNewWave;

	private void Start()
	{
		playerEntity = FindObjectOfType<Player>();
		playerT = playerEntity.transform;

		nextCampCheckTime = timeBetweenCampingChecks + Time.time;
		campPositionOld = playerT.position;
		playerEntity.OnDeath += OnPlayerDeath;

		map = FindObjectOfType<MapGenerator>();
		NextWave();

		// highlight player position - debug
		//StartCoroutine(HighLightPlayerTile());
	}

	private void Update()
	{
		if (isDisabled)
			return;

		if (Time.time > nextCampCheckTime)
		{
			nextCampCheckTime = Time.time + timeBetweenCampingChecks;
		
			isCamping = (Vector3.Distance(playerT.position, campPositionOld) < campThresholdDistance);
			campPositionOld = playerT.position;
		}

		if ((enemiesRemainingToSpawn > 0 || currentWave.infinite) && Time.time > nextSpawnTime)
		{
			enemiesRemainingToSpawn--;
			nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;
		
			StartCoroutine("SpawnEnemy");
		}

		if (devMode)
		{
			if (Input.GetKeyDown(KeyCode.Return))
			{
				foreach(Enemy enemy in FindObjectsOfType<Enemy>())
				{
					Destroy(enemy.gameObject);
				}
				StopCoroutine("SpawnEnemy");
				NextWave();
			}
		}
	}

	IEnumerator SpawnEnemy()
	{
		float spawnDelay = 1;
		float tileFlashSpeed = 8;

		Transform spawnTile = map.GetRandomOpenTile();
		if (isCamping)
		{
			spawnTile = map.GetTileFromPosition(playerT.position);
		}
		Material tileMat = spawnTile.GetComponent<Renderer>().material;
		Color originalColor = tileMat.color;
		Color flashColor = Color.red;
		float spawnTimer = 0;

		while(spawnTimer < spawnDelay)
		{
			tileMat.color = Color.Lerp(originalColor, flashColor, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1));
			spawnTimer += Time.deltaTime;
			yield return null;
		}

		Enemy spawnedEnemy = Instantiate(enemy, spawnTile.position + Vector3.up, Quaternion.identity) as Enemy;
		spawnedEnemy.SetCharacteristics(currentWave.moveSpeed, currentWave.hitsToKillPlayer, currentWave.enemyHealth, currentWave.skinColor);
		spawnedEnemy.OnDeath += OnEnemyDeath;
	}

	void OnPlayerDeath()
	{
		isDisabled = true;
	}

	void OnEnemyDeath()
	{
		enemiesRemainingAlive--;

		if (enemiesRemainingAlive == 0)
		{
			NextWave();
		}
	}

	void ResetPlayerPosition()
	{
		playerT.position = map.GetTileFromPosition(Vector3.zero).position + Vector3.up * 3;
	}

	void NextWave()
	{
		currentWaveNumber++;

		if (currentWaveNumber - 1 < waves.Length)
		{
			currentWave = waves[currentWaveNumber - 1];
			enemiesRemainingToSpawn = currentWave.enemyCount;
			enemiesRemainingAlive = enemiesRemainingToSpawn;

			OnNewWave?.Invoke(currentWaveNumber);
			ResetPlayerPosition();
		}
	}

	IEnumerator HighLightPlayerTile()
	{
		while (true)
		{
			float tileFlashSpeed = 8;
			float flashDelay = 1;
			Transform playerTile = map.GetTileFromPosition(playerT.position);
			Material tileMat = playerTile.GetComponent<Renderer>().material;
			Color originalColor = tileMat.color;
			Color flashColor = Color.green;
			float flashTimer = 0;

			while (flashTimer < flashDelay)
			{
				tileMat.color = Color.Lerp(originalColor, flashColor, Mathf.PingPong(flashTimer * tileFlashSpeed, 1));
				flashTimer += Time.deltaTime;
				yield return null;
			}
		}
	}

	[System.Serializable]
    public class Wave
	{
		public bool infinite;
		public int enemyCount;
		public float timeBetweenSpawns;

		public float moveSpeed;
		public int hitsToKillPlayer;
		public float enemyHealth;
		public Color skinColor;
	}
}
