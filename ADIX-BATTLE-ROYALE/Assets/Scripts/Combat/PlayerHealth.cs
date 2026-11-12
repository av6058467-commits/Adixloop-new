using System;
using UnityEngine;

public sealed class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField, Min(1f)] private float maxHealth = 100f;
    [SerializeField, Min(0f)] private float maxArmor = 100f;
    public float Health { get; private set; }
    public float Armor { get; private set; }
    public bool IsAlive => Health > 0f;
    public event Action<float, float> HealthChanged;
    public event Action Eliminated;

    private void Awake()
    {
        Health = maxHealth;
        Armor = maxArmor;
        HealthChanged?.Invoke(Health, Armor);
    }

    public void TakeDamage(float amount)
    {
        if (!IsAlive || amount <= 0f) return;
        float absorbed = Mathf.Min(Armor, amount * 0.65f);
        Armor -= absorbed;
        Health = Mathf.Max(0f, Health - (amount - absorbed));
        HealthChanged?.Invoke(Health, Armor);
        if (Health <= 0f)
        {
            Eliminated?.Invoke();
            enabled = false;
        }
    }

    public void Heal(float amount)
    {
        if (!IsAlive || amount <= 0f) return;
        Health = Mathf.Min(maxHealth, Health + amount);
        HealthChanged?.Invoke(Health, Armor);
    }

    public void AddArmor(float amount)
    {
        if (!IsAlive || amount <= 0f) return;
        Armor = Mathf.Min(maxArmor, Armor + amount);
        HealthChanged?.Invoke(Health, Armor);
    }
}