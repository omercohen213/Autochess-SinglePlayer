using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace Assets.HeroEditor.Common.Scripts.ExampleScripts
{
    /// <summary>
    /// General behaviour for projectiles: bullets, rockets and other.
    /// </summary>
    public class Projectile : MonoBehaviour
    {
        public List<Renderer> Renderers;
        public GameObject Trail;
        public GameObject Impact;

        private readonly float PROJECTILE_SPEED = 5f;

        public void Start()
        {
            Destroy(gameObject, 2);
        }

        private void Bang(GameObject other)
        {
            ReplaceImpactSound(other);
            Impact.SetActive(true);
            Destroy(GetComponent<SpriteRenderer>());
            Destroy(GetComponent<Rigidbody>());
            Destroy(GetComponent<Collider>());
            Destroy(gameObject, 1);

            foreach (var ps in Trail.GetComponentsInChildren<ParticleSystem>())
            {
                ps.Stop();
            }

            foreach (var tr in Trail.GetComponentsInChildren<TrailRenderer>())
            {
                tr.enabled = false;
            }
        }

        private void ReplaceImpactSound(GameObject other)
        {
            var sound = other.GetComponent<AudioSource>();

            if (sound != null && sound.clip != null)
            {
                Impact.GetComponent<AudioSource>().clip = sound.clip;
            }
        }

        public void MoveProjectile(GameUnit target, int damage, bool isCritical)
        {
            StartCoroutine(MoveProjectileCoroutine(target, damage, isCritical));
        }
        private IEnumerator MoveProjectileCoroutine(GameUnit target, int damage, bool isCritical)
        {
            if (target == null)
            {
                Debug.Log("attacked when target is null " + transform.parent.name);
                Destroy(gameObject);

                yield break;
            }

            Vector3 targetPosition = target.transform.position;

            while (Vector3.Distance(transform.position, targetPosition) > 0.05f)
            {
                if (target == null)
                {
                    Destroy(gameObject);
                    yield break;
                }

                targetPosition = target.transform.position + new Vector3(0, 0.5f);
                Vector3 newPosition = Vector3.MoveTowards(transform.position, targetPosition, PROJECTILE_SPEED * Time.deltaTime);
                transform.position = newPosition;
                // Calculate the rotation angle towards the target
                Vector3 direction = (targetPosition - transform.position).normalized;

                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                // Set the rotation of the projectile towards the target
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                //transform.rotation = Quaternion.LookRotation(direction);
                yield return null;
            }

            // Ensure the projectile reaches the target's position precisely
            transform.position = targetPosition;

            target.OnDamageTaken(damage, isCritical);

            // Perform the impact actions
            Bang(target.gameObject);
        }
    }
}