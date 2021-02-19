using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterInteractionController : MonoBehaviour
{
    [SerializeField] List<IInteractable> interactableObjects = new List<IInteractable>();

	private void OnTriggerEnter(Collider other)
	{
		Debug.Log(other);
		var interactable = other.GetComponent<IInteractable>();
		if (interactable == null) 
			return;
		interactableObjects.Add(interactable);

		interactable.EnableInteractableUI();
	}

	private void OnTriggerExit(Collider other)
	{
		var interactable = other.GetComponent<IInteractable>();
		if (interactable == null) 
			return;
		if (interactableObjects.Contains(interactable))
			interactableObjects.Remove(interactable);
	}

	public void OnInteract(InputAction.CallbackContext context)
	{
		if (!context.started)
			return;
		foreach (var interactable in interactableObjects)
			interactable.Interact();
	}
}
