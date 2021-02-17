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
}
