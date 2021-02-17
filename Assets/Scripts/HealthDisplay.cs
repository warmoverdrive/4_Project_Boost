using System.Collections;
using UnityEngine;
using TMPro;

public class HealthDisplay : MonoBehaviour
{
	TMP_Text textbox;

	void Start()
	{
		ShipStatus.PlayerHealthUpdated += OnPlayerHealthUpdated;

		StartCoroutine(InitializeDisplay());
	}

	private void OnDestroy()
	{
		ShipStatus.PlayerHealthUpdated -= OnPlayerHealthUpdated;
	}

	// This is called to stop the race condition of trying to pull data that hasnt
	// been instantiated yet.
	private IEnumerator InitializeDisplay()
	{
		textbox = GetComponent<TMP_Text>();
		var shipStatus = FindObjectOfType<ShipStatus>();
		while (shipStatus == null)
		{
			shipStatus = FindObjectOfType<ShipStatus>();
			yield return new WaitForEndOfFrame();
		}
		OnPlayerHealthUpdated(shipStatus.GetMaxHealth());
	}

	private void OnPlayerHealthUpdated(int health)
	{
		textbox.text = $"Hull Integrity:\n{health}%";
	}
}