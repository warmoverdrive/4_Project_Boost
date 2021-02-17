using System.Collections;
using UnityEngine;
using TMPro;

public class HealthDisplay : MonoBehaviour
{
	TMP_Text textbox;

	void Start()
	{
		ShipHealth.PlayerHealthUpdated += OnPlayerHealthUpdated;
		textbox = GetComponent<TMP_Text>();

		OnPlayerHealthUpdated(100); // initialize with starting health
	}

	private void OnDestroy()
	{
		ShipHealth.PlayerHealthUpdated -= OnPlayerHealthUpdated;
	}

	private void OnPlayerHealthUpdated(int health)
	{
		textbox.text = $"Hull Integrity:\n{health}%";
	}
}