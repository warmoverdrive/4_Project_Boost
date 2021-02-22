using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInteractionsTracker : MonoBehaviour
{
    [SerializeField] ShipUpgradeManager.UpgradeType currentUpgrade = ShipUpgradeManager.UpgradeType.None;
    [SerializeField] List<IInteractable> interactedObjects = new List<IInteractable>();
    [SerializeField] List<AccessCodeManager.AccessCodes> collectedCodes = new List<AccessCodeManager.AccessCodes>();

    public bool CharacterHasUpgrade()
	{
        return currentUpgrade != ShipUpgradeManager.UpgradeType.None;
	}

    public void SetUpgradeSlot(ShipUpgradeManager.UpgradeType upgrade) =>
        currentUpgrade = upgrade;
    
    public void TrackInteractable(IInteractable interactable)
	{
        if (interactedObjects.Contains(interactable))
            return;
        else
            interactedObjects.Add(interactable);
	}

    public void TrackAccessCode(AccessCodeManager.AccessCodes code)
	{
        if (collectedCodes.Contains(code))
            return;
        else
            collectedCodes.Add(code);
	}

    public void ProcessEmbark(ShipUpgradeManager upgradeManager)
	{
        if (currentUpgrade != ShipUpgradeManager.UpgradeType.None)
            upgradeManager.UpgradeShip(currentUpgrade);
        currentUpgrade = ShipUpgradeManager.UpgradeType.None;
        interactedObjects.Clear();
        collectedCodes.Clear();
	}

    public void ProcessReset()
	{
        currentUpgrade = ShipUpgradeManager.UpgradeType.None;
        foreach (var interactable in interactedObjects)
            interactable.ResetInteractable();
        foreach (var accessCode in collectedCodes)
            AccessCodeManager.accessCodes[accessCode].Access = false;
        interactedObjects.Clear();
        collectedCodes.Clear();
	}
}
