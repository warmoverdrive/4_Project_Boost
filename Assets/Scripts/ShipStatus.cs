using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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


	int health;
	float fuel;
	bool isDead = false;
	bool isEmpty = false;

	public static event Action<int> PlayerHealthUpdated;
	public static event Action<float> PlayerFuelUpdated;
	public static event Action<bool> PlayerHasDied;
	public static event Action<bool> PlayerOutOfFuel;

	public float GetDamageCooldown() => damageCooldown;
	public int GetMaxHealth() => maxHealth;
	public float GetMaxFuel() => maxFuel;
	public float GetFuelDrainPerSecond() => fuelDrainPerSecond;
	public float GetSecondsPerTick() => secondsPerTick;

	private void Start()
	{
		health = maxHealth;
		fuel = maxFuel;

		PlayerHealthUpdated?.Invoke(health);
		PlayerFuelUpdated?.Invoke(fuel);
	}

	public void TakeDamage(float rawDamage)
	{
		health -= Mathf.RoundToInt(rawDamage * damageMultiplier);

		if (health <= 0)
		{
			health = 0;
			Debug.LogError("Dead");
			isDead = true;
			PlayerHasDied?.Invoke(isDead);
		}

		PlayerHealthUpdated?.Invoke(health);
	}

	public void DrainFuel(float fuelUsed)
	{
		fuel -= fuelUsed;

		if (fuel <= 0)
		{
			fuel = 0;
			isEmpty = true;
			PlayerOutOfFuel?.Invoke(isEmpty);
		}

		PlayerFuelUpdated?.Invoke(fuel);
	}

	// Repairs and Refuels the ship each call. Checks if both fuel and health are
	// full, and returns true if R&R is complete, and false if R&R needs to continue.
	public bool RepairAndRefuel()
	{
		health = Mathf.Clamp(health + repairPerTick, 0, maxHealth);
		fuel = Mathf.Clamp(fuel + refuelPerTick, 0, maxFuel);
		PlayerFuelUpdated?.Invoke(fuel);
		PlayerHealthUpdated?.Invoke(health);

		if (health == maxHealth && fuel == maxFuel)
			return true;
		else
			return false;
	}
}
