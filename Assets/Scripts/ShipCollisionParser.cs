using System.Collections;
using System;
using UnityEngine;

public class ShipCollisionParser : MonoBehaviour
{
	[SerializeField] ShipStatus shipStatusController;
	[SerializeField] MeshCollider landingGearCollider;

	float damageCooldown;
	bool inDamageCooldown = false;
	bool isDead = false;

	private void Start()
	{
		ShipStatus.PlayerHasDied += OnUpdateHasDied;

		damageCooldown = shipStatusController.GetDamageCooldown();
	}

	private void OnDestroy()
	{
		ShipStatus.PlayerHasDied -= OnUpdateHasDied;
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (isDead || inDamageCooldown) return;

		Collider contact = collision.GetContact(0).thisCollider;

		var damage = collision.relativeVelocity.magnitude;

		if (contact == landingGearCollider && damage <= 10)
			return;

		shipStatusController.TakeDamage(damage);
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