using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AccessComputer : MonoBehaviour, IInteractable
{
	[SerializeField] AccessCodeManager.AccessCodes accessLevel = AccessCodeManager.AccessCodes.Red;
	[SerializeField] Light indicatorLight;
	[SerializeField] Color accessedColor = Color.white;

	AccessCode code;
	Canvas UIcanvas;
	TMP_Text textbox;

	bool accessGranted = false;

	private void Start()
	{
		UIcanvas = GetComponentInChildren<Canvas>();
		textbox = GetComponentInChildren<TMP_Text>();
		AccessCodeManager.accessCodes.TryGetValue(accessLevel, out code);

		indicatorLight.color = code.Color;
		textbox.text = $"DOWNLOAD\n{code.Name} ACCESS CODE";
		DisableInteractableUI();
	}

	public void EnableInteractableUI()
	{
		UIcanvas.gameObject.SetActive(true);
	}

	public void DisableInteractableUI()
	{
		if (accessGranted)
			Destroy(gameObject);

		UIcanvas.gameObject.SetActive(false);
	}


	public void Interact(GameObject player)
	{
		FindObjectOfType<AccessCodeManager>().ModifyAccess(accessLevel, true);
		textbox.text = $"{code.Name} LEVEL ACCESS GRANTED";
		indicatorLight.color = accessedColor;
		accessGranted = true;
	}
}
