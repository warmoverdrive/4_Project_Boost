using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipUpgradeManager : MonoBehaviour
{
    [Header("Upgrade Influence Values")]
    [SerializeField] int fuelCapacityIncrease = 50;
    [SerializeField] float fuelDrainReduction = 0.5f;
    [SerializeField] float damageMultiplierReduction = 0.5f;
    [SerializeField] int thrustPowerIncrease = 25000;
    [SerializeField] int upgradeCountMax = 3;

    public enum UpgradeType { None, FuelCapIncrease, FuelDrainReduction, DamageMultReduction, ThrustPowIncrease }

    private Dictionary<UpgradeType, int> UpgradeTracker = new Dictionary<UpgradeType, int> {
        { UpgradeType.FuelCapIncrease, 0 },
        { UpgradeType.FuelDrainReduction, 0 },
        { UpgradeType.DamageMultReduction, 0 },
        { UpgradeType.ThrustPowIncrease, 0 } };

    public static event Action<int> FuelCapUpgraded;
    public static event Action<float> FuelDrainUpgraded;
    public static event Action<float> DamageMultiplierUpgraded;
    public static event Action<int> ThrustPowerUpgraded;

    public void UpgradeShip(UpgradeType type)
	{
        if (UpgradeTracker[type] == upgradeCountMax)
            return;

        UpgradeTracker[type]++;

		switch(type)
		{
            case UpgradeType.FuelCapIncrease:
                FuelCapUpgraded?.Invoke(fuelCapacityIncrease);
                Debug.Log($"Fuel Cap Upgrade {UpgradeTracker[type]}");
                break;
            case UpgradeType.FuelDrainReduction:
                FuelDrainUpgraded?.Invoke(fuelDrainReduction);
                Debug.Log($"Fuel Drain Upgrade {UpgradeTracker[type]}");
                break;
            case UpgradeType.DamageMultReduction:
                DamageMultiplierUpgraded?.Invoke(damageMultiplierReduction);
                Debug.Log($"DamageMult Upgrade {UpgradeTracker[type]}");
                break;
            case UpgradeType.ThrustPowIncrease:
                ThrustPowerUpgraded?.Invoke(thrustPowerIncrease);
                Debug.Log($"ThrustPow Upgrade {UpgradeTracker[type]}");
                break;
        }
	}
}
