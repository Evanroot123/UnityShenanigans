using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
	public enum FireMode { Auto, Burst, SemiAuto};
	public FireMode fireMode;

	public Transform[] projectileSpawn;
	public Projectile projectile;
	public float msBetweenShots = 100;
	public float muzzleVelocity = 35;
	public int burstCount;
	public int magCapacity;
	public float reloadTime = .3f;

	[Header("Recoil")]
	public Vector2 kickMinMax = new Vector2(.05f, .2f);
	public Vector2 recoilAngleMinMax = new Vector2(3, 5);
	public float recoilMoveSettleTime = .1f;
	public float recoilRotationSettleTime = .1f;

	[Header("Effects")]
	public Transform shell;
	public Transform shellEjection;
	MuzzleFlash muzzleFlash;
	float nextShotTime;
	public AudioClip shootAudio;
	public AudioClip reloadAudio;

	bool triggerReleasedSinceLastShot;
	int shotsRemainingInBurst;
	int currentMagCount;
	bool isReloading;

	Vector3 recoilSmoothDampVelocity;
	float recoilAngle;
	float recoilRotSmoothDampVelocity;

	void Start()
	{
		muzzleFlash = GetComponent<MuzzleFlash>();
		shotsRemainingInBurst = burstCount;
		currentMagCount = magCapacity;
	}

	private void LateUpdate()
	{
		transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref recoilSmoothDampVelocity, recoilMoveSettleTime);
		recoilAngle = Mathf.SmoothDamp(recoilAngle, 0, ref recoilRotSmoothDampVelocity, recoilRotationSettleTime);
		transform.localEulerAngles = transform.localEulerAngles + Vector3.left * recoilAngle;
	}

	void Shoot()
	{
		if (!isReloading && currentMagCount <= 0)
		{
			Reload();
		}

		if (!isReloading && Time.time > nextShotTime && currentMagCount > 0)
		{
			if (fireMode == FireMode.Burst)
			{
				if (shotsRemainingInBurst <= 0)
					return;

				shotsRemainingInBurst--;
			}

			else if (fireMode == FireMode.SemiAuto)
			{
				if (!triggerReleasedSinceLastShot)
					return;
			}

			for (int i = 0; i < projectileSpawn.Length; i++)
			{
				if (currentMagCount <= 0)
					break;

				currentMagCount--;
				nextShotTime = Time.time + (msBetweenShots / 1000);
				Projectile newProjectile = Instantiate(projectile, projectileSpawn[i].position, projectileSpawn[i].rotation) as Projectile;
				newProjectile.speed = muzzleVelocity;
			}

			Instantiate(shell, shellEjection.position, shellEjection.rotation);
			muzzleFlash.Activate();
			transform.localPosition -= Vector3.forward * Random.Range(kickMinMax.x, kickMinMax.y);
			recoilAngle += Random.Range(recoilAngleMinMax.x, recoilAngleMinMax.y);
			recoilAngle = Mathf.Clamp(recoilAngle, 0, 30);
			AudioManager.instance.PlaySound(shootAudio, transform.position);
		}
	}

	public void Reload()
	{
		if (!isReloading && currentMagCount != magCapacity)
		{
			StartCoroutine(AnimateReload());
			AudioManager.instance.PlaySound(reloadAudio, transform.position);
		}
	}

	IEnumerator AnimateReload()
	{
		isReloading = true;
		yield return new WaitForSeconds(.2f);

		float percent = 0;
		float reloadSpeed = 1 / reloadTime;
		Vector3 initialRot = transform.localEulerAngles;
		float maxReloadAngle = 30;

		while(percent < 1)
		{
			percent += Time.deltaTime * reloadSpeed;
			float interpolation = (-percent * percent + percent) * 4;
			float reloadAngle = Mathf.Lerp(0, maxReloadAngle, interpolation);
			transform.localEulerAngles = initialRot + Vector3.left * reloadAngle;

			yield return null;
		}

		isReloading = false;
		currentMagCount = magCapacity;
	}

	public void Aim(Vector3 aimPoint)
	{
		if (!isReloading)
			transform.LookAt(aimPoint);
	}

	public void OnTriggerHold()
	{
		Shoot();
		triggerReleasedSinceLastShot = false;
	}

	public void OnTriggerRelease()
	{
		triggerReleasedSinceLastShot = true;
		shotsRemainingInBurst = burstCount;
	}
}
