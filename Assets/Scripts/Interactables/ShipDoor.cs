using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipDoor : MonoBehaviour, IInteractable
{
	[SerializeField] PlayerControlStateManager controlStateManager;
	[SerializeField] ShipUpgradeManager upgradeManager;

	Canvas interactableUI;

	private void Start()
	{
		interactableUI = GetComponentInChildren<Canvas>();
		interactableUI.gameObject.SetActive(false);
	}

	public bool Interact(GameObject player)
	{
		controlStateManager.OnEmbark();
		player.GetComponentInChildren<CharacterInteractionsTracker>().ProcessEmbark(upgradeManager);
		DisableInteractableUI();
		return true;
	}

	public void EnableInteractableUI()
	{
		interactableUI.gameObject.SetActive(true);
	}

	public void DisableInteractableUI()
	{
		interactableUI.gameObject.SetActive(false);
	}

	public void ResetInteractable() { return; }
}
