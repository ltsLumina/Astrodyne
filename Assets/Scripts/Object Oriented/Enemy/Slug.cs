using System.Collections;
using UnityEngine;
using static Essentials.Attributes;

public class Slug : Enemy
{
    [SerializeField, ReadOnly] float moveSpeed;

    [Header("Slug Specific Parameters")]
    [SerializeField] float stealDuration;
    [Tooltip("Use a number such as 0.25x, to steal 25% of the player's speed."),
    SerializeField] float stealAmountPercentage;

    protected override bool CanMove() => moveSpeed > 0;

    protected override void Start()
    {
        base.Start();
        moveSpeed = EnemyData.MoveSpeed;
    }

    protected override void Update()
    {
        base.Update();

        if (CanMove()) SlugMovement();
    }

    void SlugMovement()
    { // move towards player
        Vector2 playerPos = FindObjectOfType<Player>().transform.position;
        transform.position = Vector2.MoveTowards(transform.position, playerPos, moveSpeed * Time.deltaTime);
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);

        if (!other.gameObject.CompareTag("Player")) return;

        GameObject player = other.gameObject;
        float playerSpeed = player.GetComponent<Player>().MoveSpeed;

        StartCoroutine(StealMoveSpeed(playerSpeed));
    }

    IEnumerator StealMoveSpeed(float playerSpeed)
    {
        // Steal player moveSpeed for a short time.
        moveSpeed += playerSpeed * 0.25f;
        Debug.Log("Slug speed increased.");

        // Wait for a short time.
        yield return new WaitForSeconds(stealDuration);

        // Return to normal speed.
        moveSpeed -= playerSpeed * stealAmountPercentage;
        Debug.Log("Slug speed returned to normal.");
    }
}