using UnityEngine;

public interface IInteractable
{
    public bool Interact(GameObject player);
    public void EnableInteractableUI();
    public void DisableInteractableUI();
    public void ResetInteractable();
}