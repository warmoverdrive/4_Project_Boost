using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FuelSliderDisplay : MonoBehaviour
{
	Slider slider;

	private void Start()
	{
		StartCoroutine(InitializeSlider());

		ShipStatus.ShipFuelUpdated += OnFuelUpdated;
	}

	private void OnDestroy()
	{
		ShipStatus.ShipFuelUpdated -= OnFuelUpdated;
	}

	// This is called to stop the race condition of trying to pull data that hasnt
	// been instantiated yet.
	private IEnumerator InitializeSlider()
	{
		slider = GetComponent<Slider>();
		ShipStatus shipStatus = FindObjectOfType<ShipStatus>();
		while (shipStatus == null)
		{
			shipStatus = FindObjectOfType<ShipStatus>();
			yield return new WaitForEndOfFrame();
		}

		slider.maxValue = shipStatus.GetMaxFuel();
	}

	private void OnFuelUpdated(float fuel)
	{
		slider.value = fuel;
	}
}