using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShipMovement : MonoBehaviour
{
    [SerializeField] Rigidbody rigidBody;
	[SerializeField] float mainThrustForce = 15f;
	[SerializeField] float pivotForce = 10f;
	[SerializeField] Transform mainThrustTransform;

	Coroutine pivotAction;
	bool isDead = false;

	private void Start()
	{
		ShipHealth.PlayerHasDied += OnPlayerHasDied;
	}

	private void OnDestroy()
	{
		ShipHealth.PlayerHasDied += OnPlayerHasDied;
	}

	private bool DeathCheck()
	{
		if (isDead)
		{
			StopAllCoroutines();
			return isDead;
		}
		else return isDead;
	}

	public void OnThrust(InputAction.CallbackContext context)
	{
		if (DeathCheck()) return;

		if (context.started)
			StartCoroutine(nameof(Thrust));
		if (context.canceled)
			StopCoroutine(nameof(Thrust));
	}

	public void OnPivot(InputAction.CallbackContext context)
	{
		DeathCheck();

		if (context.started)
		{
			if (pivotAction == null)
				pivotAction = StartCoroutine(Pivot(context.action.ReadValue<float>()));
			else
			{
				StopCoroutine(pivotAction);
				pivotAction = StartCoroutine(Pivot(context.action.ReadValue<float>()));
			}
		}
			
		if (context.canceled)
			StopCoroutine(pivotAction);
	}

	private IEnumerator Pivot(float direction)
	{
		while (true)
		{
			rigidBody.AddTorque(-transform.forward * direction * pivotForce, ForceMode.Force);
			yield return new WaitForFixedUpdate();
		}
	}

	private IEnumerator Thrust()
	{
		while (true)
		{
			rigidBody.AddForceAtPosition(transform.up * mainThrustForce, mainThrustTransform.position, ForceMode.Force);
			yield return new WaitForFixedUpdate();
		}
	}

	private void OnPlayerHasDied(bool hasDied) { isDead = hasDied; }
}