using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FuelSliderDisplay : MonoBehaviour
{
	RectTransform rt;
	Slider slider;

	private void Start()
	{
		StartCoroutine(InitializeSlider());

		ShipStatus.ShipFuelUpdated += OnFuelUpdated;
		ShipUpgradeManager.FuelCapUpgraded += OnFuelCapUpdated;
	}

	private void OnDestroy()
	{
		ShipStatus.ShipFuelUpdated -= OnFuelUpdated;
		ShipUpgradeManager.FuelCapUpgraded -= OnFuelCapUpdated;
	}

	// This is called to stop the race condition of trying to pull data that hasnt
	// been instantiated yet.
	private IEnumerator InitializeSlider()
	{
		slider = GetComponent<Slider>();
		rt = GetComponent<RectTransform>();
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

	private void OnFuelCapUpdated(int capIncrease)
	{
		rt.sizeDelta = new Vector2(rt.sizeDelta.x + capIncrease, rt.sizeDelta.y);
		slider.maxValue += capIncrease;
	}
}