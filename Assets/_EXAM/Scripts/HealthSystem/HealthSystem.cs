using System;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public Action OnDeath;
    [SerializeField]
    public HealthUI healthUI;

    [field: SerializeField]
    public int MaxHealth { get; private set; } = 3;
    public int CurrentHealth { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CurrentHealth = MaxHealth;
        healthUI.SetMaxHealth(MaxHealth);
    }

    public void TakeDamage(int amount = 1)
    {
        CurrentHealth -= amount;
        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            if(OnDeath != null) OnDeath();
        }
        healthUI.UpdateCurrentHealth(CurrentHealth);
    }
}
