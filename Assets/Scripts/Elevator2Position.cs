using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Elevator2Position : MonoBehaviour, IInteractable
{
	[SerializeField] Transform elevatorPlatform;
    [SerializeField] Transform position1;
    [SerializeField] Transform position2;
	[SerializeField] float transitionTime;
	[SerializeField] Light indicatorLight;
    [SerializeField] Color waitingColor = Color.green;
    [SerializeField] Color busyColor = Color.red;
	[SerializeField] string pos1UIText = "UP";
	[SerializeField] string pos2UIText = "DOWN";

	Canvas interactUI;
	Transform target;
	bool isBusy = false;
	bool isPosition1 = true;

	private void Start()
	{
		interactUI = GetComponentInChildren<Canvas>();
		interactUI.gameObject.SetActive(false);
		indicatorLight.color = waitingColor;
	}
	public void EnableInteractableUI()
	{
		if (isBusy) return;
		interactUI.gameObject.SetActive(true);
		interactUI.GetComponentInChildren<TMP_Text>().text = isPosition1 ? pos1UIText : pos2UIText;
	}

	public void DisableInteractableUI()
	{
		if (isBusy) return;
		interactUI.gameObject.SetActive(false);
	}


	public void Interact(GameObject player)
	{
		if (isBusy)
			return;

		if (isPosition1)
			target = position2;
		else
			target = position1;

		StartCoroutine(ElevatorMove(player));
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
			elevatorPlatform.position = Vector3.Lerp(startPos, target.position, currentTime / transitionTime);
			currentTime += Time.deltaTime;
			currentTime = Mathf.Clamp(currentTime, 0, transitionTime);
			yield return new WaitForEndOfFrame();
		}

		player.transform.parent = null;
		isPosition1 = !isPosition1;
		isBusy = false;
		indicatorLight.color = waitingColor;
		EnableInteractableUI();
	}

}
