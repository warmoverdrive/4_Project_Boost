using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class PlayerControlStateManager : MonoBehaviour
{
    public enum ControlState { Ship, Character, Dialogue };

    [SerializeField] ControlState controlState = ControlState.Ship;
    [SerializeField] PlayerInput inputManager;

    [Header("Ship Objects")]
    [SerializeField] CinemachineVirtualCamera shipCam;
    [SerializeField] Canvas shipUI;
    [SerializeField] Transform shipExit;

    [Header("Character Objects")]
    [SerializeField] GameObject character;
    [SerializeField] CinemachineVirtualCamera charCam;

    ControlState previousControlState = ControlState.Character;

    public ControlState GetControlState() => controlState;

	private void Start()
	{
        if (controlState == ControlState.Character)
            Disembark();
        if (controlState == ControlState.Ship)
            OnEmbark();
        if (controlState == ControlState.Dialogue)
            OnEmbark();

        DialogueInteractableHandler.PlayerInDialogue += OnDialogue;
	}

	private void OnDestroy()
	{
        DialogueInteractableHandler.PlayerInDialogue -= OnDialogue;
    }

	public void OnDisembark(InputAction.CallbackContext context)
	{
        if (!context.started)
            return;

        if (GetComponentInParent<Rigidbody>().velocity.magnitude > 0) // if ship moving
            return;

        Disembark();
	}

    public void OnEmbark()
	{
        controlState = ControlState.Ship;
        shipCam.Priority = 1;
        charCam.Priority = 0;
        inputManager.SwitchCurrentActionMap("Ship");
        shipUI.enabled = true;
        character.SetActive(false);
    }

    private void Disembark()
	{
        controlState = ControlState.Character;
        shipCam.Priority = 0;
        charCam.Priority = 1;
        inputManager.SwitchCurrentActionMap("Character");
        shipUI.enabled = false;
        character.transform.position = shipExit.position;
        character.GetComponent<Rigidbody>().velocity = Vector3.zero;
        character.SetActive(true);
    }

    private void OnDialogue(bool isInDialogue)
	{
        if (isInDialogue)
		{
            previousControlState = controlState;
            controlState = ControlState.Dialogue;
            inputManager.SwitchCurrentActionMap("UI");
		}
		else
		{
            controlState = previousControlState;
            if (controlState == ControlState.Character)
                inputManager.SwitchCurrentActionMap("Character");
            else
                inputManager.SwitchCurrentActionMap("Ship");
        }
	}
}
