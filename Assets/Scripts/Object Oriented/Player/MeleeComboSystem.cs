using System;
using System.Collections;
using Essentials;
using UnityEngine;

public class MeleeComboSystem : MonoBehaviour
{
    [SerializeField, ReadOnly] float timeSinceLastMelee;
    [SerializeField] float attackForce;
    [SerializeField] float recoilForce;

    Weapon weapon;

    // Delegate for when the player melee attacks.
    public delegate void OnMeleeAttack();
    public event OnMeleeAttack onMeleeAttack;

    void Start() => weapon = GetComponent<Weapon>();

    public void MeleeCombat()
    {
        timeSinceLastMelee += Time.deltaTime;

        //TODO: MAKE COMBO SCRIPT
        //USE THIS: https://www.youtube.com/watch?v=Jm0mbHEFPfE
        if (Input.GetMouseButton(1) && timeSinceLastMelee > 0.45f)
        {
            onMeleeAttack?.Invoke();
            weapon.CombatTime = 10;
            GetComponent<Animator>().SetTrigger("doAttack");
            Vector3 direction = (Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position)).normalized;
            FindObjectOfType<Player>().GetComponent<Rigidbody2D>().AddForce(direction * attackForce, ForceMode2D.Impulse);

            timeSinceLastMelee = 0;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("HIT ENEMY");
            Vector3 direction = (Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position)).normalized;
            FindObjectOfType<Player>().GetComponent<Rigidbody2D>().AddForce(-direction * recoilForce, ForceMode2D.Impulse);
        }
    }
}