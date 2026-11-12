using System.Collections;
using UnityEngine;
using AdixBattleRoyale.Combat;
using AdixBattleRoyale.Emotes;

namespace AdixBattleRoyale.Gameplay
{
    public sealed class PlayerUtilityActions : MonoBehaviour
    {
        [SerializeField] private PlayerHealth health;
        [SerializeField] private Transform throwOrigin;
        [SerializeField] private EmoteController emoteController;
        [SerializeField] private EmoteDefinition[] healingEmoteSlots = new EmoteDefinition[8];
        [SerializeField, Range(0, 7)] private int selectedHealingEmoteSlot;
        [SerializeField, Min(0.1f)] private float medkitUseDuration = 2.5f;
        [SerializeField, Min(1f)] private float grenadeDamage = 85f;
        [SerializeField, Min(1f)] private float grenadeRadius = 7f;
        [SerializeField, Min(0f)] private float grenadeFuse = 2.2f;
        [SerializeField, Min(0f)] private float throwForce = 12f;
        [SerializeField] private int grenades = 2;
        [SerializeField] private int medkits = 2;

        private Coroutine medkitRoutine;
        private bool isUsingMedkit;

        public int Grenades => grenades;
        public int Medkits => medkits;
        public bool IsUsingMedkit => isUsingMedkit;
        public int SelectedHealingEmoteSlot => selectedHealingEmoteSlot;
        public EmoteDefinition SelectedHealingEmote => healingEmoteSlots != null && selectedHealingEmoteSlot >= 0 && selectedHealingEmoteSlot < healingEmoteSlots.Length ? healingEmoteSlots[selectedHealingEmoteSlot] : null;

        public bool SelectHealingEmote(int slot)
        {
            if (slot < 0 || slot >= 8 || healingEmoteSlots == null || slot >= healingEmoteSlots.Length || healingEmoteSlots[slot] == null) return false;
            if (healingEmoteSlots[slot].Category != EmoteCategory.Healing) return false;
            selectedHealingEmoteSlot = slot;
            return true;
        }

        public void SetHealingEmote(int slot, EmoteDefinition emote)
        {
            if (slot < 0 || slot >= 8 || healingEmoteSlots == null || slot >= healingEmoteSlots.Length) return;
            if (emote != null && emote.Category != EmoteCategory.Healing) return;
            healingEmoteSlots[slot] = emote;
        }

        private void Awake()
        {
            if (health == null) health = GetComponent<PlayerHealth>();
            if (emoteController == null) emoteController = GetComponent<EmoteController>();
            if (throwOrigin == null && Camera.main != null) throwOrigin = Camera.main.transform;
        }

        public void ThrowGrenade()
        {
            if (grenades <= 0 || throwOrigin == null || isUsingMedkit) return;
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
            if (isUsingMedkit || health == null || medkits <= 0 || !health.IsAlive || health.Health >= health.MaxHealth)
                return;

            medkits--;
            isUsingMedkit = true;
            if (emoteController != null)
            {
                emoteController.SetHealingState(true);
                EmoteDefinition chosenEmote = SelectedHealingEmote;
                if (chosenEmote != null)
                    emoteController.Play(chosenEmote);
            }
            medkitRoutine = StartCoroutine(CompleteMedkitUse());
        }

        private IEnumerator CompleteMedkitUse()
        {
            yield return new WaitForSeconds(medkitUseDuration);
            if (health != null && health.IsAlive)
                health.Heal(45f);

            if (emoteController != null)
            {
                emoteController.Stop();
                emoteController.SetHealingState(false);
            }
            isUsingMedkit = false;
            medkitRoutine = null;
        }

        public void AddGrenades(int amount) => grenades = Mathf.Clamp(grenades + amount, 0, 6);
        public void AddMedkits(int amount) => medkits = Mathf.Clamp(medkits + amount, 0, 5);

        private void OnDisable()
        {
            if (medkitRoutine != null)
            {
                StopCoroutine(medkitRoutine);
                medkitRoutine = null;
            }
            isUsingMedkit = false;
            if (emoteController != null)
            {
                emoteController.Stop();
                emoteController.SetHealingState(false);
            }
        }
    }
}
