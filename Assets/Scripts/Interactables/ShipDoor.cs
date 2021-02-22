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
		interactableUI.gameObject.SetActive(false);
	}

	public void Interact(GameObject player)
	{
		controlStateManager.OnEmbark();
		DisableInteractableUI();
	}

	public void EnableInteractableUI()
	{
		interactableUI.gameObject.SetActive(true);
	}

	public void DisableInteractableUI()
	{
		interactableUI.gameObject.SetActive(false);
	}
}
