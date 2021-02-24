using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCollisionParser : MonoBehaviour
{
    [SerializeField] CharacterStatus characterStatus;

	bool isDead = false;

	private void Start()
	{
		CharacterStatus.CharacterDied += OnCharacterDeath;
		CharacterStatus.CharacterRespawned += OnCharacterRespawn;
	}

	private void OnDestroy()
	{
		CharacterStatus.CharacterDied -= OnCharacterDeath;
		CharacterStatus.CharacterRespawned -= OnCharacterRespawn;
	}

	private void OnCharacterDeath() => isDead = true;
	private void OnCharacterRespawn() => isDead = false;

	private void OnCollisionEnter(Collision collision)
	{
		if (isDead)
			return;
		float rawMagnitude = collision.relativeVelocity.magnitude;
		if (rawMagnitude < characterStatus.GetMagnitudeTolerance())
			return;
		else
			characterStatus.TakeDamage(rawMagnitude);
	}
}
