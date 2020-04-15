using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(PlayerController))]
[RequireComponent(typeof(GunController))]
public class Player : LivingEntity
{
	public float moveSpeed = 5f;

	Camera viewCamera;
	PlayerController controller;
	GunController gunController;

	public override void Start()
    {
		//startingHealth = 10;
		base.Start();
		controller = GetComponent<PlayerController>();
		gunController = GetComponent<GunController>();
		viewCamera = Camera.main;
    }

    void Update()
    {
		// Movement
		Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
		Vector3 moveVelocity = moveInput.normalized * moveSpeed;
		controller.Move(moveVelocity);

		// look
		Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
		Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
		float rayDistance;

		if (groundPlane.Raycast(ray, out rayDistance))
		{
			Vector3 point = ray.GetPoint(rayDistance);
			//Debug.DrawLine(ray.origin, point, Color.red);
			controller.LookAt(point);
		}

		// weapon
		if (Input.GetMouseButton(0))
		{
			gunController.OnTriggerHold();
		}
		if (Input.GetMouseButtonUp(0))
		{
			gunController.OnTriggerRelease();
		}
	}
}
