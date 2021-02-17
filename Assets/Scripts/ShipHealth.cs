using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ShipHealth : MonoBehaviour
{
    [SerializeField] int health = 100;
	[SerializeField] float damageMultiplier = 1.5f;
	[SerializeField] float damageCooldown = 0.5f;

	bool isDead = false;

	public static event Action<int> PlayerHealthUpdated;
	public static event Action<bool> PlayerHasDied;

	public float GetDamageCooldown() => damageCooldown;

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
}
