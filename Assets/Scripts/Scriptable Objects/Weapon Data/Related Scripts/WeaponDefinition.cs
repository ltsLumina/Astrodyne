using UnityEngine;

[CreateAssetMenu(fileName = "Weapon Data", menuName = "Weapons/Create Weapon", order = 0)]
public class WeaponDefinition : ScriptableObject
{
    public enum WeaponClass { Melee, Ranged, }
    public WeaponClass weaponClass;

    [Header("Common Weapon Traits")]
    public GameObject weaponPrefab;
    public int damage;
    public float attackDelay;
    public float lifetime;
    public float knockbackForce;

    [Header("Slashing Weapon Traits")]
    [HideInInspector] public float animationSpeedScalar;
    [HideInInspector] public float size;
    [HideInInspector] public float stepDistance;

    [Header("Ranged Weapon Traits")]
    [HideInInspector] public float bulletSpeed;
    [HideInInspector] public float bulletSize;
}