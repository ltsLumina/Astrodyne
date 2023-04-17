using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    void Update()
    {
        // move towards player
        Vector2 playerPos = GameManager.Instance.Player.transform.position;
        transform.position = Vector2.MoveTowards(transform.position, playerPos, 1f * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("HIT PLAYER");
    }
}