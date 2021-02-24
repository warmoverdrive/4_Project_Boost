using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [Header("Design Levers")]
    [SerializeField] float startupTime = 0.5f;
    [SerializeField] float activeTime = 3f;
    [SerializeField] float inactiveTime = 3f;
    [SerializeField] float laserDamagePerSecond = 25;
    [Header("Component References")]
    [SerializeField] Transform targetPos;
    [SerializeField] LayerMask interactionLayers;
    [SerializeField] GameObject hitParticlesPrefab;

    LineRenderer beamRenderer;
    ParticleSystem[] particleSystems;

	private void Start()
	{
        beamRenderer = GetComponentInChildren<LineRenderer>();
        particleSystems = GetComponentsInChildren<ParticleSystem>();
        beamRenderer.SetPosition(1, Vector3.zero);
        foreach (var particleSys in particleSystems)
		{
            particleSys.Stop();
		}

        StartCoroutine(LaserHandler());
	}

    private IEnumerator LaserHandler()
	{
        yield return new WaitForSeconds(Random.Range(0f, 3f));
        while (true)
		{
            yield return new WaitForSeconds(inactiveTime);
            yield return LaserActiveRoutine();
		}
	}

    private IEnumerator LaserActiveRoutine()
	{
        // Startup loop
        foreach (var particleSys in particleSystems)
            particleSys.Play();
        GameObject impactParticles = Instantiate(
            hitParticlesPrefab, transform.position, Quaternion.identity, transform);

        // This confusing looking "line" determines if the targetPos is obstructed at all
        // and resolves a collision by setting the starting target either to the collision point
        // or the target point
        Vector3 startingTarget = Physics.Raycast(
            transform.position,
            targetPos.localPosition.normalized,
            out RaycastHit hit,
            Vector3.Distance(transform.position, targetPos.position),
            interactionLayers) ? transform.InverseTransformPoint(hit.point) : targetPos.position;

        float startupTimer = 0f;
        while (startupTimer < startupTime)
		{
            yield return new WaitForEndOfFrame();
            startupTimer = Mathf.Clamp(startupTimer + Time.deltaTime, 0, startupTime);
            beamRenderer.SetPosition(1, Vector3.Lerp(
                Vector3.zero, startingTarget, startupTimer / startupTime));
		}

        // Active Loop (collision checking)
        float activeTimer = 0f;
        while (activeTimer < activeTime)
		{
            yield return new WaitForEndOfFrame();
            activeTimer = Mathf.Clamp(activeTimer + Time.deltaTime, 0, activeTime);
            CheckCollisions(impactParticles);
		}

        // Deactivate loop
        impactParticles.GetComponent<ParticleSystem>().Stop(true);
        Destroy(impactParticles, 1f);

        // Get the current position of the beam as the starting position for the
        // deactivation
        Vector3 endingTarget = beamRenderer.GetPosition(1);

        while (startupTimer > 0)
        {
            yield return new WaitForEndOfFrame();
            startupTimer = Mathf.Clamp(startupTimer - Time.deltaTime, 0, startupTime);
            beamRenderer.SetPosition(1, Vector3.Lerp(
                Vector3.zero, endingTarget, startupTimer / startupTime));
        }

        foreach (var particleSys in particleSystems)
            particleSys.Stop();
    }

    private void CheckCollisions(GameObject impactParticles)
	{
        if (Physics.Raycast(
            transform.position,
            targetPos.localPosition.normalized, 
            out RaycastHit hit,
            Vector3.Distance(transform.position, targetPos.position),
            interactionLayers))
		{
            impactParticles.transform.position = hit.point;
            beamRenderer.SetPosition(1, transform.InverseTransformPoint(hit.point));

            // check for damagables
            if (hit.rigidbody)
			{
                var damageable = hit.rigidbody.GetComponentInChildren<IDamageable>();
                if (damageable != null)
			    {
                    damageable.TakeDamage(laserDamagePerSecond * Time.deltaTime);
			    }
			}
        }
		else
		{
            impactParticles.transform.position = targetPos.position;
            beamRenderer.SetPosition(1, targetPos.localPosition);
		}
	}
}
