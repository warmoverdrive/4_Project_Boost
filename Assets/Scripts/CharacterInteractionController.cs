using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterInteractionController : MonoBehaviour
{
	[SerializeField] float enabledInteractionCooldown = 3f;
    [SerializeField] List<IInteractable> interactableObjects = new List<IInteractable>();

	bool interactionEnabled = false;

	private void OnEnable()
	{
		// on enable, stop character from interacting with objects for a short time
		// basically just to stop UI elements from popping in immediately on disembark
		StartCoroutine(EnabledInteractionCooldown());
	}

	private void OnTriggerEnter(Collider other)
	{
		if (interactionEnabled == false)
			return;

		Debug.Log(other);
		var interactable = other.GetComponent<IInteractable>();
		if (interactable == null) 
			return;
		interactableObjects.Add(interactable);

		interactable.EnableInteractableUI();
	}

	private void OnTriggerExit(Collider other)
	{
		if (interactionEnabled == false)
			return;

		var interactable = other.GetComponent<IInteractable>();
		if (interactable == null) 
			return;

		// remove from list
		if (interactableObjects.Contains(interactable))
			interactableObjects.Remove(interactable);

		// turn off UI tooltip
		interactable.DisableInteractableUI();
	}

	public void OnInteract(InputAction.CallbackContext context)
	{
		if (!context.started)
			return;
		foreach (var interactable in interactableObjects)
			interactable.Interact();
	}

	private IEnumerator EnabledInteractionCooldown()
	{
		interactionEnabled = false;
		yield return new WaitForSeconds(enabledInteractionCooldown);
		interactionEnabled = true;
	}
}
