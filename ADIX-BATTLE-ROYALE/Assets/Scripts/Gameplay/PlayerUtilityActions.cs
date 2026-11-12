using System.Collections;
using UnityEngine;
using AdixBattleRoyale.Combat;

namespace AdixBattleRoyale.Gameplay
{
    public sealed class PlayerUtilityActions : MonoBehaviour
    {
        [SerializeField] private PlayerHealth health;
        [SerializeField] private Transform throwOrigin;
        [SerializeField, Min(1f)] private float grenadeDamage = 85f;
        [SerializeField, Min(1f)] private float grenadeRadius = 7f;
        [SerializeField, Min(0f)] private float grenadeFuse = 2.2f;
        [SerializeField, Min(0f)] private float throwForce = 12f;
        [SerializeField] private int grenades = 2;
        [SerializeField] private int medkits = 2;
        public int Grenades => grenades;
        public int Medkits => medkits;

        private void Awake()
        {
            if (health == null) health = GetComponent<PlayerHealth>();
            if (throwOrigin == null && Camera.main != null) throwOrigin = Camera.main.transform;
        }

        public void ThrowGrenade()
        {
            if (grenades <= 0 || throwOrigin == null) return;
            grenades--;
            var grenade = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            grenade.name = "ADIX Grenade";
            grenade.transform.position = throwOrigin.position + throwOrigin.forward * 0.8f;
            grenade.transform.localScale = Vector3.one * 0.22f;
            grenade.GetComponent<Renderer>().material.color = new Color(0.16f, 0.2f, 0.18f);
            var body = grenade.AddComponent<Rigidbody>();
            body.mass = 0.45f;
            body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            body.AddForce(throwOrigin.forward * throwForce + Vector3.up * 3f, ForceMode.VelocityChange);
            StartCoroutine(Detonate(grenade));
        }

        private IEnumerator Detonate(GameObject grenade)
        {
            yield return new WaitForSeconds(grenadeFuse);
            if (grenade == null) yield break;
            Vector3 center = grenade.transform.position;
            Collider[] hits = Physics.OverlapSphere(center, grenadeRadius);
            foreach (Collider hit in hits)
            {
                IDamageable target = hit.GetComponentInParent<IDamageable>();
                if (target == null || hit.transform.root == transform.root) continue;
                float distance = Vector3.Distance(center, hit.ClosestPoint(center));
                target.TakeDamage(grenadeDamage * Mathf.Clamp01(1f - distance / grenadeRadius));
            }
            Destroy(grenade);
        }

        public void UseMedkit()
        {
            if (health == null || medkits <= 0 || !health.IsAlive || health.Health >= health.MaxHealth) return;
            medkits--;
            health.Heal(45f);
        }

        public void AddGrenades(int amount) => grenades = Mathf.Clamp(grenades + amount, 0, 6);
        public void AddMedkits(int amount) => medkits = Mathf.Clamp(medkits + amount, 0, 5);
    }
}