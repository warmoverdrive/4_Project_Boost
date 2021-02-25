using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DialogueInteractableHandler : MonoBehaviour, ISubmitHandler, ICancelHandler, IInteractable
{
	[Header("Design Levers")]
	[SerializeField] float wipeSpeed = 1f;
	[SerializeField] float charactersPerSecond = 25f;
	[TextArea(2,3)]
	[SerializeField] string unreadMessage = "-- Unread\nMessage --";
	[TextArea(2, 3)]
	[SerializeField] string readMessage = "-- Message\nRead --";
	[SerializeField] DialogueScriptableObject dialogueObject;
	[Header("Component References")]
	[SerializeField] Canvas interactUICanvas;
	[SerializeField] TMP_Text interactUIText;
	[SerializeField] RectTransform dialogueUIParent;
	[SerializeField] TMP_Text dialogueTextBox;

	public static event Action<bool> PlayerInDialogue;

	private Queue<string> sentences = new Queue<string>();
	UIUtility uiUtility = new UIUtility();
	EventSystem eventSystem;

	bool isInDialogue = false;
	bool hasRead = false;

	Coroutine currentSentenceCoroutine;
	string currentSentence;

	private void Start()
	{
		interactUICanvas.gameObject.SetActive(false);
		dialogueUIParent.parent.gameObject.SetActive(false);
		eventSystem = FindObjectOfType<EventSystem>();
	}

	public void OnSubmit(BaseEventData eventData)
	{
		if (!isInDialogue)
			return;
		if (currentSentenceCoroutine == null)
		{
			if (sentences.Count == 0)
			{
				ExitDialogue();
			}
			currentSentenceCoroutine = StartCoroutine(DisplaySentence());
		}
		else
		{
			StopCoroutine(currentSentenceCoroutine);
			currentSentenceCoroutine = null;
			dialogueTextBox.text = currentSentence;
		}
	}

	public void OnCancel(BaseEventData eventData)
	{
		if (!isInDialogue)
			return;
		ExitDialogue();
	}

	private void ExitDialogue()
	{
		if (currentSentenceCoroutine != null)
		{
			StopCoroutine(currentSentenceCoroutine);
			currentSentenceCoroutine = null;
		}
		currentSentence = null;
		isInDialogue = false;
		hasRead = true;
		dialogueTextBox.text = "";
		StartCoroutine(uiUtility.UIWipeOut(dialogueUIParent, UIUtility.UIWipeStyle.Vertical, wipeSpeed));
		Time.timeScale = 1;
		PlayerInDialogue?.Invoke(false);
		EnableInteractableUI();
	}

	public bool Interact(GameObject player)
	{
		eventSystem.SetSelectedGameObject(gameObject);
		PlayerInDialogue?.Invoke(true);
		Time.timeScale = 0f;
		StartCoroutine(StartDialogue());
		return true;
	}

	private IEnumerator StartDialogue()
	{
		dialogueTextBox.text = "";
		foreach (var sentence in dialogueObject.sentences)
			sentences.Enqueue(sentence);
		yield return StartCoroutine(uiUtility.UIWipeIn(dialogueUIParent, UIUtility.UIWipeStyle.Vertical, wipeSpeed));
		isInDialogue = true;
		currentSentenceCoroutine = StartCoroutine(DisplaySentence());
	}

	private IEnumerator DisplaySentence()
	{
		dialogueTextBox.text = "";
		if (sentences.Count > 0)
		{
			currentSentence = sentences.Dequeue();

			foreach (var character in currentSentence.ToCharArray())
			{
				dialogueTextBox.text += character;
				yield return new WaitForSecondsRealtime(1/charactersPerSecond);
			}
		}
		currentSentenceCoroutine = null;
	}

	public void EnableInteractableUI()
	{
		interactUICanvas.gameObject.SetActive(true);
		interactUIText.text = hasRead ? readMessage : unreadMessage;
	}

	public void DisableInteractableUI()
	{
		interactUICanvas.gameObject.SetActive(false);
	}

	public void ResetInteractable()
	{
		hasRead = false;
	}
}
