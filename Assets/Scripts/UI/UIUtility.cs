using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIUtility
{
    public enum UIWipeStyle { Horizontal, Vertical };

	public IEnumerator UIWipeIn(RectTransform targetUI, UIWipeStyle wipeStyle, float wipeTime)
	{
		// this assumes the parent is a canvas... might cause issues
		targetUI.parent.gameObject.SetActive(true);
		Vector3 startScale = Vector3.zero;

		if (wipeStyle == UIWipeStyle.Horizontal)
			startScale = new Vector3(0, 1, 1);
		else if (wipeStyle == UIWipeStyle.Vertical)
			startScale = new Vector3(1, 0, 1);

		targetUI.localScale = startScale;
		float timer = 0;

		while (timer < wipeTime)
		{
			yield return new WaitForEndOfFrame();
			timer = Mathf.Clamp(timer + Time.unscaledDeltaTime, 0, wipeTime);
			targetUI.localScale = Vector3.Lerp(startScale, Vector3.one, timer / wipeTime);
		}
	}

    public IEnumerator UIWipeOut (RectTransform targetUI, UIWipeStyle wipeStyle, float wipeTime)
	{
		Vector3 targetScale = Vector3.zero;

		if (wipeStyle == UIWipeStyle.Horizontal)
			targetScale = new Vector3(0, 1, 1);
		else if (wipeStyle == UIWipeStyle.Vertical)
			targetScale = new Vector3(1, 0, 1);

		float timer = 0;

		while (timer < wipeTime)
		{
			yield return new WaitForEndOfFrame();
			timer = Mathf.Clamp(timer + Time.unscaledDeltaTime, 0, wipeTime);
			targetUI.localScale = Vector3.Lerp(Vector3.one, targetScale, timer / wipeTime);
		}
		// this assumes the parent is a canvas... might cause issues
		targetUI.parent.gameObject.SetActive(false);
	}
}
