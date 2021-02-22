using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class ShipStatus : MonoBehaviour
{
	[Header("Health")]
    [SerializeField] int maxHealth = 100;
	[SerializeField] float damageMultiplier = 1.5f;
	[SerializeField] float damageCooldown = 0.5f;
	[Header("Fuel")]
	[SerializeField] float maxFuel = 100;
	[SerializeField] float fuelDrainPerSecond = 5f;
	[Header("R&R")]
	[SerializeField] float secondsPerTick = 1f;
	[SerializeField] int repairPerTick = 15;
	[SerializeField] float refuelPerTick = 10f;
	[Header("Respawn")]
	[SerializeField] Transform respawnPoint;
	[SerializeField] float respawnTime = 2f;


	int health;
	float fuel;
	bool isDead = false;
	bool isEmpty = false;

	public static event Action<int> ShipHealthUpdated;
	public static event Action<float> ShipFuelUpdated;
	public static event Action<bool> ShipHasDied;
	public static event Action<bool> ShipOutOfFuel;
	public static event Action ShipHasRespawned;

	public float GetDamageCooldown() => damageCooldown;
	public int GetMaxHealth() => maxHealth;
	public float GetMaxFuel() => maxFuel;
	public float GetFuelDrainPerSecond() => fuelDrainPerSecond;
	public float GetSecondsPerTick() => secondsPerTick;

	private void Start()
	{
		health = maxHealth;
		fuel = maxFuel;

		ShipHealthUpdated?.Invoke(health);
		ShipFuelUpdated?.Invoke(fuel);

		ShipUpgradeManager.FuelCapUpgraded += OnUpgradeFuelCap;
		ShipUpgradeManager.FuelDrainUpgraded += OnFuelDrainUpgraded;
		ShipUpgradeManager.DamageMultiplierUpgraded += OnDamageMultiplierUpgraded;
	}

	private void OnDestroy()
	{
		ShipUpgradeManager.FuelCapUpgraded -= OnUpgradeFuelCap;
		ShipUpgradeManager.FuelDrainUpgraded -= OnFuelDrainUpgraded;
		ShipUpgradeManager.DamageMultiplierUpgraded -= OnDamageMultiplierUpgraded;
	}

	public void TakeDamage(float rawDamage)
	{
		health -= Mathf.RoundToInt(rawDamage * damageMultiplier);

		if (health <= 0)
		{
			health = 0;
			isDead = true;
			ShipHasDied?.Invoke(isDead);
			StartCoroutine(HandleRespawn(respawnTime));
		}

		ShipHealthUpdated?.Invoke(health);
	}

	public void DrainFuel(float fuelUsed)
	{
		fuel -= fuelUsed;

		if (fuel <= 0)
		{
			fuel = 0;
			isEmpty = true;
			ShipOutOfFuel?.Invoke(isEmpty);
		}

		ShipFuelUpdated?.Invoke(fuel);
	}

	// Repairs and Refuels the ship each call. Checks if both fuel and health are
	// full, and returns true if R&R is complete, and false if R&R needs to continue.
	public bool RepairAndRefuel()
	{
		health = Mathf.Clamp(health + repairPerTick, 0, maxHealth);
		fuel = Mathf.Clamp(fuel + refuelPerTick, 0, maxFuel);
		ShipFuelUpdated?.Invoke(fuel);
		ShipHealthUpdated?.Invoke(health);

		if (health == maxHealth && fuel == maxFuel)
			return true;
		else
			return false;
	}

	public void SetShipSpawnPoint(Transform spawnpt) { respawnPoint = spawnpt; }

	private IEnumerator HandleRespawn(float timer)
	{
		yield return new WaitForSeconds(timer);
		health = maxHealth;
		fuel = maxFuel;
		transform.parent.position = respawnPoint.position;
		transform.parent.rotation = respawnPoint.rotation;
		isDead = false;

		ShipHealthUpdated?.Invoke(health);
		ShipFuelUpdated?.Invoke(fuel);
		ShipHasRespawned?.Invoke();
	}

	public void OnShipReset(InputAction.CallbackContext context)
	{
		if (isDead) return;
		isDead = true;
		ShipHasDied?.Invoke(isDead);
		StartCoroutine(HandleRespawn(0.5f));
	}

	private void OnUpgradeFuelCap(int capIncrease) => maxFuel += capIncrease;
	private void OnFuelDrainUpgraded(float drainDecrease) => fuelDrainPerSecond -= drainDecrease;
	private void OnDamageMultiplierUpgraded(float damMultDecrease) => damageMultiplier -= damMultDecrease;
}
