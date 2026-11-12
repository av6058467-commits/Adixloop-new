using UnityEngine;
using AdixBattleRoyale.Combat;

namespace AdixBattleRoyale.World
{
    public enum LootType { Health, Armor, Ammo }

    /// <summary>Simple proximity pickup; place trigger collider on the pickup prefab.</summary>
    [RequireComponent(typeof(Collider))]
    public sealed class LootPickup : MonoBehaviour
    {
        [SerializeField] private LootType lootType;
        [SerializeField, Min(1)] private int amount = 25;
        [SerializeField, Min(0.1f)] private float despawnSeconds = 60f;
        private bool collected;

        private void Awake()
        {
            GetComponent<Collider>().isTrigger = true;
            if (despawnSeconds > 0f) Destroy(gameObject, despawnSeconds);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (collected) return;
            PlayerHealth health = other.GetComponentInParent<PlayerHealth>();
            PlayerWeapon weapon = other.GetComponentInParent<PlayerWeapon>();
            bool applied = false;

            switch (lootType)
            {
                case LootType.Health:
                    if (health != null && health.IsAlive && health.Health < 100f)
                    {
                        health.Heal(amount);
                        applied = true;
                    }
                    break;
                case LootType.Armor:
                    if (health != null && health.IsAlive)
                    {
                        health.AddArmor(amount);
                        applied = true;
                    }
                    break;
                case LootType.Ammo:
                    if (weapon != null)
                    {
                        weapon.AddReserveAmmo(amount);
                        applied = true;
                    }
                    break;
            }

            if (!applied) return;
            collected = true;
            Destroy(gameObject);
        }
    }
}