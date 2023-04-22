using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    int health = 10;

    void Update()
    {
        // move towards player
        Vector2 playerPos = GameManager.Instance.Player.transform.position;
        transform.position = Vector2.MoveTowards(transform.position, playerPos, 1f * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("HIT PLAYER");
        }

        if (other.gameObject.CompareTag("Weapon"))
        {
            Debug.Log("HIT WEAPON, BOUNCING OFF");
            Vector2 direction = (other.transform.position - transform.position).normalized;
            transform.Translate(-direction * 25 * Time.deltaTime);

            // take damage
            health--;
            StartCoroutine(PlayerAnimationManager.SpriteRoutine(0.5f, GetComponent<SpriteRenderer>()));

            if (health <= 0) Destroy(gameObject);
        }
    }
}