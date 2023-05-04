using UnityEngine;

[CreateAssetMenu(fileName = "Slash Parameter", menuName = "Slash Attack/Create Slash Parameters", order = 0)]
public class SlashParameters : ScriptableObject
{
    public float slashSize;
    public float dashAttackSlashSize;
    public float knockbackForce;
    public float dashAttackKnockbackForce;
}