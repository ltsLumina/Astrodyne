using UnityEngine;
using static Essentials.Attributes;

public class CombatManager : SingletonPersistent<CombatManager>
{
    [SerializeField, ReadOnly]
    float combatTime;
    public float CombatTime
    {
        get => combatTime;
        private set => combatTime = value;
    }

    public void EnterCombat(float combatDuration = 10) => CombatTime = combatDuration;

    public bool IsInCombat()
    {
        if (CombatTime > 0)
            CombatTime -= Time.deltaTime;

        return CombatTime > 0;
    }
}