using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShipMovement : MonoBehaviour
{
	[SerializeField] ShipStatus shipStatusController;
	[SerializeField] Rigidbody rigidBody;
	[SerializeField] float mainThrustForce;
	[SerializeField] float pivotForce;
	[SerializeField] Transform mainThrustTransform;

	float fuelDrainPerSecond;
	Coroutine pivotAction;
	bool isDead = false;
	bool isEmpty = false;

	private void Start()
	{
		ShipStatus.ShipHasDied += OnPlayerHasDied;
		ShipStatus.ShipOutOfFuel += OnPlayerEmpty;
		ShipStatus.ShipHasRespawned += OnRespawned;
		ShipUpgradeManager.FuelDrainUpgraded += OnFuelDrainUpgraded;
		ShipUpgradeManager.ThrustPowerUpgraded += OnThrustUpgraded;

		fuelDrainPerSecond = shipStatusController.GetFuelDrainPerSecond();
	}

	private void OnDestroy()
	{
		ShipStatus.ShipHasDied -= OnPlayerHasDied;
		ShipStatus.ShipOutOfFuel -= OnPlayerEmpty;
		ShipStatus.ShipHasRespawned -= OnRespawned;
		ShipUpgradeManager.FuelDrainUpgraded -= OnFuelDrainUpgraded;
		ShipUpgradeManager.ThrustPowerUpgraded -= OnThrustUpgraded;
	}

	private bool DeathCheck()
	{
		if (isDead)
		{
			StopAllCoroutines();
			return isDead;
		}
		else return isDead;
	}

	public void OnThrust(InputAction.CallbackContext context)
	{
		if (DeathCheck()) return;
		if (isEmpty)
		{
			StopCoroutine(nameof(Thrust));
			return;
		}

		if (context.started)
			StartCoroutine(nameof(Thrust));
		if (context.canceled)
			StopCoroutine(nameof(Thrust));
	}
	
	public void OnPivot(InputAction.CallbackContext context)
	{
		if (DeathCheck()) return;

		if (context.started)
		{
			if (pivotAction == null)
				pivotAction = StartCoroutine(Pivot(context.action));
		}
			
		if (context.canceled)
		{
			if (pivotAction == null)
				return;
			StopCoroutine(pivotAction);
			pivotAction = null;
		}
	}

	private IEnumerator Pivot(InputAction pivotAction)
	{
		
		while (true && !isDead)
		{
			DeathCheck();
			rigidBody.AddTorque(-transform.forward * pivotAction.ReadValue<float>() * pivotForce, ForceMode.Force);
			yield return new WaitForFixedUpdate();
		}
	}

	private IEnumerator Thrust()
	{
		while (true && !isEmpty && !isDead)
		{
			rigidBody.AddForceAtPosition(transform.up * mainThrustForce, mainThrustTransform.position, ForceMode.Force);
			shipStatusController.DrainFuel(fuelDrainPerSecond * Time.deltaTime);
			yield return new WaitForFixedUpdate();
		}
	}

	private void OnPlayerHasDied(bool hasDied) => isDead = hasDied;
	private void OnPlayerEmpty(bool hasEmptied) => isEmpty = hasEmptied;
	private void OnFuelDrainUpgraded(float fuelDrainDecrease) => fuelDrainPerSecond -= fuelDrainDecrease;
	private void OnThrustUpgraded(int thrustIncrease)
	{
		mainThrustForce += thrustIncrease;
		pivotForce += thrustIncrease;
	}

	private void OnRespawned()
	{
		isDead = false;
		isEmpty = false;
		rigidBody.velocity = Vector3.zero;
		rigidBody.angularVelocity = Vector3.zero;
	}
}