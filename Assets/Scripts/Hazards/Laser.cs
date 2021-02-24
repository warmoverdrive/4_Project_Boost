using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [Header("Design Levers")]
    [SerializeField] float startupTime = 0.5f;
    [SerializeField] float activeTime = 3f;
    [SerializeField] float inactiveTime = 3f;
    [SerializeField] float laserRadius = 2.5f;
    [SerializeField] float laserDamage = 5;
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

        float startupTimer = 0f;
        while (startupTimer < startupTime)
		{
            yield return new WaitForEndOfFrame();
            startupTimer = Mathf.Clamp(startupTimer + Time.deltaTime, 0, startupTime);
            beamRenderer.SetPosition(1, Vector3.Lerp(
                Vector3.zero, targetPos.localPosition, startupTimer / startupTime));
		}

        // Active Loop (collision checking)
        GameObject impactParticles = Instantiate(
            hitParticlesPrefab, targetPos.position, Quaternion.identity, transform);

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

        while (startupTimer > 0)
        {
            yield return new WaitForEndOfFrame();
            startupTimer = Mathf.Clamp(startupTimer - Time.deltaTime, 0, startupTime);
            beamRenderer.SetPosition(1, Vector3.Lerp(
                Vector3.zero, targetPos.localPosition, startupTimer / startupTime));
        }

        foreach (var particleSys in particleSystems)
            particleSys.Stop();
    }

    private void CheckCollisions(GameObject impactParticles)
	{
        if (Physics.SphereCast(
            transform.position, 
            laserRadius,
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
                if (hit.rigidbody.GetComponentInChildren<ShipStatus>())
			    {
                    hit.rigidbody.GetComponentInChildren<ShipStatus>().TakeDamage(laserDamage);
			    }
                else if (hit.rigidbody.GetComponentInChildren<CharacterStatus>())
                {
                    hit.rigidbody.GetComponentInChildren<CharacterStatus>().Damage(laserDamage);
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
