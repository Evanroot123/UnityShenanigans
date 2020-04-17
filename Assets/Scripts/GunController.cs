using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
	public Transform weaponHold;
	public Gun[] allGuns;
	Gun equippedGun;

	void Start()
	{

	}

	public void OnTriggerHold()
	{
		if (equippedGun != null)
		{
			equippedGun.OnTriggerHold();
		}
	}

	public void OnTriggerRelease()
	{
		if (equippedGun != null)
		{
			equippedGun.OnTriggerRelease();
		}
	}

	public float GunHeight
	{
		get { return weaponHold.position.y; }
	}

	public void EquipGun(int weaponIndex)
	{
		EquipGun(allGuns[2]);
	}

	public void EquipGun(Gun gunToEquip)
	{
		if (equippedGun != null)
		{
			Destroy(equippedGun.gameObject);
		}

		equippedGun = Instantiate(gunToEquip, weaponHold.position, weaponHold.rotation) as Gun;
		equippedGun.transform.parent = weaponHold;
	}

	public void Aim(Vector3 aimPoint)
	{
		equippedGun.Aim(aimPoint);
	}

	public void Reload()
	{
		equippedGun.Reload();
	}
}
