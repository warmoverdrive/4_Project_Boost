using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObstacleTogglePosition : MonoBehaviour, IInteractable
{
	[SerializeField] AccessCodeManager.AccessCodes accessCode = AccessCodeManager.AccessCodes.None;
	[SerializeField] Transform obstacleObject;
    [SerializeField] Transform position1;
    [SerializeField] Transform position2;
	[SerializeField] float transitionTime;
	[SerializeField] Light indicatorLight;
    [SerializeField] Color waitingColor = Color.white;
    [SerializeField] Color busyColor = Color.magenta;
	[SerializeField] string pos1UIText = "UP";
	[SerializeField] string pos2UIText = "DOWN";

	Canvas interactUI;
	TMP_Text textbox;
	Transform target;
	bool isBusy = false;
	bool isPosition1 = true;
	bool isLocked = false;
	bool playerHasLeft = false;

	private void Start()
	{
		interactUI = GetComponentInChildren<Canvas>();
		textbox = interactUI.GetComponentInChildren<TMP_Text>();
		Initialize();
	}

	private void Initialize()
	{
		if (accessCode == AccessCodeManager.AccessCodes.None)
			indicatorLight.color = waitingColor;
		else
		{
			AccessCodeManager.accessCodes.TryGetValue(accessCode, out AccessCode code);
			indicatorLight.color = code.Color;
			isLocked = true;
		}
		obstacleObject.position = position1.position;
		isPosition1 = true;
		interactUI.gameObject.SetActive(false);
	}

	public void EnableInteractableUI()
	{
		if (isBusy) return;
		interactUI.gameObject.SetActive(true);
		textbox.text = isPosition1 ? pos1UIText : pos2UIText;
	}

	public void DisableInteractableUI()
	{
		if (isBusy) playerHasLeft = true;
		interactUI.gameObject.SetActive(false);
	}


	public bool Interact(GameObject player)
	{
		if (isBusy)
			return false;

		if (isLocked)
			if (ChallengeAccess() == false)
				return false;

		if (isPosition1)
			target = position2;
		else
			target = position1;

		StartCoroutine(ElevatorMove(player));
		return true;
	}

	private bool ChallengeAccess()
	{
		AccessCodeManager.accessCodes.TryGetValue(accessCode, out AccessCode code);

		if (code.Access)
		{
			isLocked = false;
			return true;
		}
		else
		{
			textbox.text = $"ACCESS DENIED:\n{code.Name} CODE";
			return false;
		}
	}

	private IEnumerator ElevatorMove(GameObject player)
	{
		player.transform.parent = gameObject.transform;
		DisableInteractableUI();
		isBusy = true;
		indicatorLight.color = busyColor;
		float currentTime = 0f;
		Vector3 startPos = isPosition1 ? position1.position : position2.position;

		while (currentTime < transitionTime)
		{
			obstacleObject.position = Vector3.Lerp(startPos, target.position, currentTime / transitionTime);
			currentTime += Time.deltaTime;
			currentTime = Mathf.Clamp(currentTime, 0, transitionTime);
			yield return new WaitForEndOfFrame();
		}

		player.transform.parent = null;
		isPosition1 = !isPosition1;
		isBusy = false;
		indicatorLight.color = waitingColor;
		// check if the player has left the interactable UI area
		if (playerHasLeft)
			playerHasLeft = false;
		else
			EnableInteractableUI();
	}

	public void ResetInteractable()
	{
		Initialize();
	}
}
