using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeZone : MonoBehaviour
{
	[SerializeField] Transform spawnPoint;
	bool isDead = false;
	Coroutine RandR;

	private void Start()
	{
		ShipStatus.ShipHasDied += OnPlayerDiedUpdated;
		ShipStatus.ShipHasRespawned += OnRespawned;
	}

	private void OnDestroy()
	{
		ShipStatus.ShipHasDied -= OnPlayerDiedUpdated;
		ShipStatus.ShipHasRespawned -= OnRespawned;
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (isDead) return;
		if (RandR != null) return;
		// isolate the first collider contact's tag
		if (collision.GetContact(0).otherCollider.tag == "LandingGear")
		{
			var status = collision.gameObject.GetComponentInChildren<ShipStatus>();
			if (status == null)
				return;
			
			RandR = StartCoroutine(RepairAndRefuel(status));
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		if (RandR != null)
		{
			StopAllCoroutines();
			RandR = null;
		}
	}

	private IEnumerator RepairAndRefuel(ShipStatus status)
	{
		Transform parentTransform = status.transform.parent.transform;
		var shipRB = status.GetComponentInParent<Rigidbody>();

		while (true)
		{
			yield return new WaitForSeconds(status.GetSecondsPerTick());

			// checks to make sure the ship is stationary before r&r
			// includes wobbles or change in angle - must be totally
			// landed to use the platform.
			if (shipRB.velocity.magnitude > 0)
				continue;
			// ensure the ship is relatively level
			if (parentTransform.rotation.eulerAngles.z > 5 || parentTransform.rotation.eulerAngles.z < -5)
				continue;

			status.SetShipSpawnPoint(spawnPoint);

			if (status.RepairAndRefuel() == true)
				break;
		}
	}

	private void OnPlayerDiedUpdated(bool hasDied) => isDead = hasDied;
	private void OnRespawned() => isDead = false;
}
