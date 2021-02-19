using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{
	[SerializeField] Rigidbody rb;
	[SerializeField] float moveSpeed = 5f;
	[SerializeField] float acceleration = 25f;
	[SerializeField] float jumpForce = 5f;

	Coroutine movementRoutine;

	public void OnMovement(InputAction.CallbackContext context)
	{
		if (context.started && movementRoutine == null)
			movementRoutine = StartCoroutine(HandleMovement(context.action));
	}

	public void OnJump(InputAction.CallbackContext context)
	{
		if (context.started)
			rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.VelocityChange);
	}

	private IEnumerator HandleMovement(InputAction action)
	{
		float direction = action.ReadValue<float>();
		while (direction != 0)
		{
			rb.AddForce(new Vector3(direction * acceleration, 0, 0), ForceMode.Acceleration);
			rb.velocity = new Vector3(
				Mathf.Clamp(rb.velocity.x, -moveSpeed, moveSpeed), rb.velocity.y, rb.velocity.z);

			yield return new WaitForFixedUpdate();

			direction = action.ReadValue<float>();
		}
		// clear self-reference before exiting method
		movementRoutine = null;
	}
}