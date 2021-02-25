using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class CharacterStatus : MonoBehaviour, IDamageable
{
    [SerializeField] int maxHealth = 100;
    [SerializeField] float damageMultiplier = 5f;
    [SerializeField] float magnatudeTolerance = 4f;
    [SerializeField] float respawnTime = 2f;
    [SerializeField] Volume ppVolume;
    [SerializeField] CharacterInteractionsTracker tracker;
    [SerializeField] Transform spawnPos;
    [SerializeField] int health;

    Vignette vignette;

    public static event Action CharacterDied;
    public static event Action CharacterRespawned;

    Coroutine respawnRoutine;

    public float GetMagnitudeTolerance() => magnatudeTolerance;

	private void Start()
	{
        health = maxHealth;
        ppVolume.profile.TryGet(out vignette);
	}

    public void TakeDamage(float magnitude)
	{
        health -= Mathf.RoundToInt(magnitude * damageMultiplier);
        health = Mathf.Clamp(health, 0, maxHealth);

        vignette.intensity.value = Mathf.Lerp(1, 0, (float)health / maxHealth);

        if (health == 0)
        {
            CharacterDied?.Invoke();
            if (respawnRoutine == null)
                respawnRoutine = StartCoroutine(Respawn(respawnTime));
        }
    }

    public void OnReset(InputAction.CallbackContext context)
	{
        if (respawnRoutine == null && context.started)
            respawnRoutine = StartCoroutine(Respawn(0.25f));
	}

    private IEnumerator Respawn(float time)
	{
        tracker.ProcessReset();
        yield return new WaitForSeconds(time);
        health = maxHealth;
        vignette.intensity.value = 0;
        transform.parent.position = spawnPos.position;
        GetComponentInParent<Rigidbody>().velocity = Vector3.zero;
        CharacterRespawned?.Invoke();
        respawnRoutine = null;
	}
}
