using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Upgrade : MonoBehaviour, IInteractable
{
	[SerializeField] ShipUpgradeManager.UpgradeType upgradeType;
	[SerializeField] GameObject upgradeModel;
	[SerializeField] string UIText;

	Canvas UICanvas;
	TMP_Text textbox;

	bool isCollected = false;

	private void Start()
	{
		UICanvas = GetComponentInChildren<Canvas>();
		textbox = GetComponentInChildren<TMP_Text>();

		Initialize();
	}

	private void Initialize()
	{
		isCollected = false;
		textbox.text = UIText;
		UICanvas.gameObject.SetActive(false);
		upgradeModel.SetActive(true);
	}

	public void EnableInteractableUI()
	{
		if (isCollected)
			return;
		textbox.text = UIText;
		UICanvas.gameObject.SetActive(true);
	}

	public void DisableInteractableUI()
	{
		UICanvas.gameObject.SetActive(false);
	}

	public bool Interact(GameObject player)
	{
		if (isCollected)
			return false;
		var tracker = player.GetComponentInChildren<CharacterInteractionsTracker>();
		if (tracker.CharacterHasUpgrade())
		{
			textbox.text = "Inventory Full";
			return false;
		}

		tracker.SetUpgradeSlot(upgradeType);
		upgradeModel.SetActive(false);
		isCollected = true;
		DisableInteractableUI();
		return true;
	}

	public void ResetInteractable()
	{
		Initialize();
	}
}
