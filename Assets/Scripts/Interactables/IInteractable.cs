using UnityEngine;

public interface IInteractable
{
    public void Interact(GameObject player);
    public void EnableInteractableUI();
    public void DisableInteractableUI();
}