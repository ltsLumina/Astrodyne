using UnityEngine;
using static GameManager;

/// <summary>
/// TODO: REWORK THE ENEMY SCRIPT
/// </summary>
public class Enemy : MonoBehaviour
{
    [SerializeField] int health = 100;

    void Update()
    {
        // move towards player
        Vector2 playerPos = FindObjectOfType<Player>().transform.position;
        //transform.position = Vector2.MoveTowards(transform.position, playerPos, 1f * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;

        Debug.Log("HIT PLAYER");
        var player = other.gameObject;
        player.GetComponent<Player>().CurrentHealth--;
        KnockbackRoutine(player, player.transform.position - transform.position, 25);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        StartCoroutine(PlayerAnimationManager.SpriteRoutine(0.5f, GetComponent<SpriteRenderer>()));

        if (health <= 0) Destroy(gameObject);
    }
}