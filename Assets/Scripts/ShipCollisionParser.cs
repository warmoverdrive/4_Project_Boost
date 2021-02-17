﻿using System.Collections;
using System;
using UnityEngine;

public class ShipCollisionParser : MonoBehaviour
{
	[SerializeField] ShipHealth shipHealthController;
	[SerializeField] MeshCollider landingGearCollider;

	float damageCooldown;
	bool inDamageCooldown = false;
	bool isDead = false;

	private void Start()
	{
		ShipHealth.PlayerHasDied += OnUpdateHasDied;

		damageCooldown = shipHealthController.GetDamageCooldown();
	}

	private void OnDestroy()
	{
		ShipHealth.PlayerHasDied -= OnUpdateHasDied;
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (isDead || inDamageCooldown) return;

		Collider contact = collision.GetContact(0).thisCollider;

		var damage = collision.relativeVelocity.magnitude;

		if (contact == landingGearCollider && damage <= 10)
			return;

		shipHealthController.TakeDamage(damage);
		StartCoroutine(DamageCooldown());
	}

	private IEnumerator DamageCooldown()
	{
		inDamageCooldown = true;

		yield return new WaitForSecondsRealtime(damageCooldown);

		inDamageCooldown = false;
	}

	private void OnUpdateHasDied(bool hasDied) => isDead = hasDied;
}