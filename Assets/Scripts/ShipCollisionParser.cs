using System.Collections;
using System;
using UnityEngine;

public class ShipCollisionParser : MonoBehaviour
{
	[SerializeField] ShipStatus shipStatusController;
	[SerializeField] MeshCollider landingGearCollider;
	[SerializeField] LayerMask damagerLayers;

	float damageCooldown;
	bool inDamageCooldown = false;
	bool isDead = false;

	private void Start()
	{
		ShipStatus.ShipHasDied += OnUpdateHasDied;
		ShipStatus.ShipHasRespawned += OnRespawned;

		damageCooldown = shipStatusController.GetDamageCooldown();
	}

	private void OnDestroy()
	{
		ShipStatus.ShipHasDied -= OnUpdateHasDied;
		ShipStatus.ShipHasRespawned -= OnRespawned;
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (isDead || inDamageCooldown) return;

		Collider contact = collision.GetContact(0).thisCollider;

		// check for collision layer in layermask using bitwise comparison
		if ((damagerLayers & 1 << collision.gameObject.layer) != 1 << collision.gameObject.layer)
			return;

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
	private void OnRespawned() => isDead = false;
}