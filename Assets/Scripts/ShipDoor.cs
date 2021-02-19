using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipDoor : MonoBehaviour, IInteractable
{
	[SerializeField] PlayerControlStateManager controlStateManager;

	Canvas interactableUI;

	private void Start()
	{
		interactableUI = GetComponentInChildren<Canvas>();
		interactableUI.enabled = false;
	}

	public void Interact()
	{
		controlStateManager.OnEmbark();
		DisableInteractableUI();
	}

	public void EnableInteractableUI()
	{
		interactableUI.enabled = true;
	}

	public void DisableInteractableUI()
	{
		interactableUI.enabled = false;
	}
}
